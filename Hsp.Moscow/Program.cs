using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Hsp.Moscow.Extensibility;
using Hsp.Moscow.Properties;
using Rug.Osc;

namespace Hsp.Moscow
{

  class Program : ServiceBase, IMoscowHost
  {

    private const string DefaultServiceName = "Moscow";

    private PeriodicTask MidiKeepAliveTask { get; set; }

    private MidiDevice MidiIn { get; set; }

    private PeriodicTask OscInputTask { get; set; }

    private OscSender OscOut { get; set; }
    
    private OscReceiver OscIn { get; set; }

    private bool DebugMode { get; set; }


    private List<IMoscowPlugin> Plugins { get; set; }


    static void Main(string[] args)
    {
      var instance = new Program();

      if (args.Length > 0)
      {
        instance.DebugMode = args[0].Equals("debug", StringComparison.OrdinalIgnoreCase);
        if (args[0].Equals("install", StringComparison.OrdinalIgnoreCase))
        {
          var serviceName = args.Length > 1 ? args[1] : DefaultServiceName;
          Environment.Exit(InstallService(serviceName));
        }
        if (args[0].Equals("uninstall", StringComparison.OrdinalIgnoreCase))
        {
          var serviceName = args.Length > 1 ? args[1] : DefaultServiceName;
          Environment.Exit(UninstallService(serviceName));
        }
      }

      if (instance.DebugMode)
      {
        instance.OnStart(args);
        Console.WriteLine("Moscow is running. Press <ENTER> to exit.");
        Console.ReadLine();
        instance.Stop();
      }
      else
        Run(instance);
    }


    private static int RunSc(params string[] args)
    {
      var proc = new Process
      {
        StartInfo = new ProcessStartInfo
        {
          FileName = "sc.exe",
          Arguments = String.Join(" ", args),
          CreateNoWindow = true,
          WindowStyle = ProcessWindowStyle.Hidden
        }
      };
      proc.Start();
      proc.WaitForExit();
      return proc.ExitCode;
    }

    private static int InstallService(string serviceName)
    {
      var executable = typeof(Program).Assembly.Location;
      return RunSc("create", serviceName, $"binPath= \"{executable}\"");
    }

    private static int UninstallService(string serviceName)
    {
      return RunSc("delete", serviceName);
    }


    public event MidiMessageReceivedHandler MidiMessageReceived;
    
    public event EventHandler<OscEventArgs> OscMessageReceived;


    private void CreateMidiIn()
    {
      CloseMidiIn();
      var name = Settings.Default.InputMidiDevice;
      MidiDevice.Refresh();
      var midiDevice = MidiDevice.ByName(MidiDeviceType.Input, name);
      if (midiDevice != null)
      {
        midiDevice.MessageReceived += DeviceOnMessageReceived;
        //device.StartRecording();
        midiDevice.Open();
        MidiIn = midiDevice;
        WriteDebug($"MIDI device '{name}' successfully opened");
      }
    }

    private void LoadPlugins()
    {
      Plugins = new List<IMoscowPlugin>();

      var pluginFolder = Settings.Default.PluginFolder;
      if (String.IsNullOrEmpty(pluginFolder))
        pluginFolder = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "Plugins");

      if (!Directory.Exists(pluginFolder))
        return;

      foreach (var file in Directory.EnumerateFiles(pluginFolder, "*.dll"))
      {
        Type[] types = { };
        try
        {
          var asm = Assembly.LoadFile(file);
          types = asm.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IMoscowPlugin))).ToArray();
        }
        catch
        {
          // ignore invalid assemblies
        }

        foreach (var type in types)
        {
          try
          {
            var plugin = (IMoscowPlugin) Activator.CreateInstance(type);
            plugin.HostStartup(this);
            Plugins.Add(plugin);
            WriteDebug($"Loaded plugin '{plugin.Name}' from '{file}'");
          }
          catch
          {
            // ignore plugins that fail to load
          }
        }
      }
    }

    private void DeviceOnMessageReceived(IMidiDevice device, IMidiMessage message)
    {
      WriteDebug($"Got MIDI : {message}");
      MidiMessageReceived?.Invoke(device, message);
    }

    private void WriteDebug(string msg)
    {
      if (!DebugMode)
        return;
      Console.WriteLine(msg);
    }

    private static OscMessage[] ExplodeBundle(OscPacket packet)
    {
      if (packet is OscMessage message)
        return new [] { message };

      if (packet is OscBundle bundle)
        return bundle.OfType<OscMessage>().ToArray();

      return new OscMessage[] { };
    }

    private void CloseMidiIn()
    {
      MidiIn?.Close();
      MidiIn = null;
    }

    private void MidiInputKeepAlive()
    {
      if (MidiIn == null)
      {
        var midiDeviceName = Settings.Default.InputMidiDevice;
        WriteDebug($"MIDI device '{midiDeviceName}' appears offline. Reconnecting ...");
        CreateMidiIn();
      }
    }

    private void HandleOscIn()
    {
      OscPacket packet = null;
      try
      {
        packet = OscIn.Receive();
      }
      catch
      {
        // Ignore exceptions.
        // Most probably an exception indicating the underlying socket was closed during service shutdown.
      }

      if (packet == null)
        return;

      var messages = ExplodeBundle(packet);
      foreach (var message in messages)
        try
        {
          if (Settings.Default.ResetMidiOnPlay)
            message.On("/play", (Single) 1, m =>
            {
              WriteDebug("Received 'play' message. Resetting MIDI device ...");
              CreateMidiIn();
            });

          var msg = new OscEventArgs(message.Address, message.ToArray());
          //WriteDebug($"Got OSC: {msg}");
          OscMessageReceived?.Invoke(this, msg);
        }
        catch
        {
          // ignore
        }
    }


    protected override void OnStart(string[] args)
    {
      MidiKeepAliveTask = new PeriodicTask(MidiInputKeepAlive, TimeSpan.FromSeconds(2.5));

      OscOut = new OscSender(IPAddress.Parse(Settings.Default.OscHost), 0, Settings.Default.OscPortOut);
      OscOut.Connect();

      OscIn = new OscReceiver(IPAddress.Parse(Settings.Default.OscHost), Settings.Default.OscPortIn);
      OscIn.Connect();
      OscInputTask = new PeriodicTask(HandleOscIn, TimeSpan.Zero);

      LoadPlugins();
    }

    protected override void OnStop()
    {
      foreach (var plugin in Plugins)
        plugin.HostShutdown();

      MidiKeepAliveTask.Abort();
      CloseMidiIn();

      OscOut?.Close();
      OscOut?.Dispose();

      OscInputTask.Abort();
      OscIn?.Close();
      OscIn?.Dispose();
    }

    
    public void SendOscMessage(OscEventArgs msg)
    {
      WriteDebug($"Sending OSC: {msg}");
      OscOut.Send(new OscMessage(msg.Address, msg.Args));
    }

  }

}
