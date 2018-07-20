using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using Hsp.Moscow.Extensibility;

namespace Hsp.Moscow.Plugins
{

  class MoscowVlcRemote : IMoscowPlugin
  {

    private IMoscowHost Host { get; set; }

    private VlcClient Client { get; set; }


    public string VlcHost => "localhost:8080";

    public string Name => "Hsp.VlcRemote";
    

    public void HostStartup(IMoscowHost host)
    {
      Client = new VlcClient(VlcHost)
      {
        Password = "password"
      };
      host.MidiMessageReceived += HostOnMidiMessageReceived;
      host.OscMessageReceived += HostOnOscMessageReceived;
    }

    public void HostShutdown()
    {
      Host.OscMessageReceived -= HostOnOscMessageReceived;
      Host.MidiMessageReceived -= HostOnMidiMessageReceived;
    }

    private void HostOnMidiMessageReceived(object sender, MidiEventArgs e)
    {
      if (e.Status != 12) // program change
        return;

      //e.Data1
    }

    private void HostOnOscMessageReceived(object sender, OscEventArgs e)
    {
      if (e.Address == "/play" && e.GetArg<Single>(0) == 1)
        InitPlaylist();
    }

    private async Task InitPlaylist()
    {
      await Client.ClearPlaylist();
      await Client.EnqueueItem(@"file:///C:/Users/thomas/Desktop/Projektionen.xspf");
    }

  }

}