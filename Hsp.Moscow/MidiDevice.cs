using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using Hsp.Moscow.Extensibility;
using Sanford.Multimedia.Midi;
using IMidiMessage = Hsp.Moscow.Extensibility.IMidiMessage;

namespace Hsp.Moscow
{

  public partial class MidiDevice : IDisposable, IMidiDevice
  {


    private InputDevice In { get; set; }

    private OutputDevice Out { get; set; }

    public static MidiDevice[] Devices { get; private set; }

    public static void Refresh()
    {
      var devices = new List<MidiDevice>();

      for (var i = 0; i < InputDevice.DeviceCount; i++)
      {
        var caps = InputDevice.GetDeviceCapabilities(i);
        devices.Add(new MidiDevice
        {
          Index = i,
          Name = caps.name,
          Type = MidiDeviceType.Input
        });
      }

      for (var i = 0; i < OutputDeviceBase.DeviceCount; i++)
      {
        var caps = OutputDeviceBase.GetDeviceCapabilities(i);
        devices.Add(new MidiDevice
        {
          Index = i,
          Name = caps.name,
          Type = MidiDeviceType.Output
        });
      }

      Devices = devices.ToArray();
    }

    public static MidiDevice ByName(MidiDeviceType type, string name)
    {
      return Devices
        .Where(d => d.Type == type)
        .SingleOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }


    public Queue<IMidiMessage> MessageQueue { get; }

    public bool QueueMessages { get; set; }

    public MidiDeviceType Type { get; private set; }

    public int Index { get; private set; }

    public string Name { get; private set; }


    public event MidiMessageReceivedHandler MessageReceived;


    private MidiDevice()
    {
      MessageQueue = new Queue<IMidiMessage>();
      QueueMessages = false;
    }

    
    private void In_ChannelMessageReceived(object sender, ChannelMessageEventArgs e)
    {
      var msg = new GenericMidiMessage
      {
        Channel = e.Message.MidiChannel,
        Status = e.Message.Status,
        Data1 = e.Message.Data1,
        Data2 = e.Message.Data2
      };

      if (QueueMessages)
        MessageQueue.Enqueue(msg);

      MessageReceived?.Invoke(this, msg);
    }


    public void Open()
    {
      Close();

      if (Type == MidiDeviceType.Input)
      {
        In = new InputDevice(Index);
        In.ChannelMessageReceived += In_ChannelMessageReceived;
        In.StartRecording();
      }

      if (Type == MidiDeviceType.Output)
      {
        Out = new OutputDevice(Index);
      }
    }

    public void Close()
    {
      if (In != null)
      {
        In.StopRecording();
        In.ChannelMessageReceived -= In_ChannelMessageReceived;
        In.Close();
        In.Dispose();
        In = null;
      }
      if (Out != null)
      {
        Out?.Close();
        Out.Dispose();
        Out = null;
      }
    }

    public void Dispose()
    {
      Close();
    }

  }
}
