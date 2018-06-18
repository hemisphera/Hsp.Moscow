using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Msgr;
using Sanford.Multimedia.Midi;

namespace Hsp.Moscow.MidiHandler
{
  class Program
  {

    private static bool CanExit { get; set; }

    private static InputDevice MidiIn { get; set; }

    private static MsgrClient Client { get; set; }


    static void Main(string[] args)
    {
      CanExit = false;

      var clientName = args.Length > 0 ? args[0] : Guid.NewGuid().ToString();

      var recipient = "Moscow";

      Client = new MsgrClient(clientName);
      Client.Connect();
      Client.Send(recipient, "Ready");

      Client.On("MidiOpen", msg =>
      {
        CloseMidi();
        OpenMidi(msg);
      });

      Client.On("Shutdown", msg =>
      {
        CloseMidi();
        CanExit = true;
      });

      while (!CanExit) 
        Thread.Sleep(100);
    }

    private static void OpenMidi(MsgrMessage msg)
    {
      var deviceName = Encoding.UTF8.GetString(msg.Data);
      var deviceId = -1;
      for (var i = 0; i < InputDevice.DeviceCount; i++)
      {
        var caps = InputDevice.GetDeviceCapabilities(i);
        if (caps.name.Equals(deviceName, StringComparison.OrdinalIgnoreCase))
          deviceId = i;
      }

      if (deviceId > 0)
      {
        MidiIn = new InputDevice(deviceId);
        MidiIn.ChannelMessageReceived += MidiInOnChannelMessageReceived;
      }
    }

    private static void MidiInOnChannelMessageReceived(object sender, ChannelMessageEventArgs e)
    {
      using (var ms = new MemoryStream())
      using (var bw = new BinaryWriter(ms))
      {
        bw.Write(e.Message.MidiChannel);
        bw.Write(e.Message.Status);
        bw.Write(e.Message.Data1);
        bw.Write(e.Message.Data2);
        bw.Flush();
        Client.Send("Moscow", "MidiIn", ms.ToArray());
      }
    }

    private static void CloseMidi()
    {
      if (MidiIn != null)
      {
        MidiIn.ChannelMessageReceived -= MidiInOnChannelMessageReceived;
        MidiIn.StopRecording();
        MidiIn.Close();
        MidiIn.Dispose();
      }

      MidiIn = null;
    }

  }

}
