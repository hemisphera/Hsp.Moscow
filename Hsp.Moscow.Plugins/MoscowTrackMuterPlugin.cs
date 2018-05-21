using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hsp.Moscow.Extensibility;

namespace Hsp.Moscow.Plugins
{

  public class MoscowTrackMuterPlugin : IMoscowPlugin
  {

    public string Name => "Hsp.TrackMuter";

    private IMoscowHost Host { get; set; }

    private List<TrackInfo> Tracks { get; set; }


    public MoscowTrackMuterPlugin()
    {
      Tracks = new List<TrackInfo>();
    }


    public void HostStartup(IMoscowHost host)
    {
      Host = host;

      // subscribe to MIDI / OSC event receiver events on host
      Host.OscMessageReceived += HostOnOscMessageReceived;
      Host.MidiMessageReceived += HostOnMidiMessageReceived;

      // send REAPER action 41743 (Refresh All Control Surfaces) to trigger a complete dump of tracks
      Host.SendOscMessage(new OscEventArgs("/action/41743"));
    }

    private void HostOnMidiMessageReceived(object sender, MidiEventArgs e)
    {
      if (e.Status != 12) // program change
        return;

      // Find all tracks that have the string 'ChXX-PCYYY' in it's name.
      // where XX is for the midi channel and YYY for the program change
      // Whenever a MIDI program change is received, all tracks on that channel are muted, except the one where YYY matches the program change received
      var tracks = Tracks.Select(t =>
        {
          var m = Regex.Match(t.Name, "Ch(?<mc>[0-9]{2})-PC(?<mpc>[0-9]{3})");
          var channel = m.Success ? int.Parse(m.Groups["mc"].Value) : -1;
          var programChange = m.Success ? int.Parse(m.Groups["mpc"].Value) : -1;
          return new
          {
            Track = t,
            Channel = channel,
            PC = programChange
          };
        })
        .Where(i => i.Channel == e.Channel + 1)
        .ToArray();
      
      foreach (var trk in tracks)
        MuteTrack(trk.Track, trk.PC != e.Data1);
    }

    private void MuteTrack(TrackInfo track, bool isMute)
    {
      // Mute / Unmute track in REAPER
      Host.SendOscMessage(new OscEventArgs($"/track/{track.Id}/mute", isMute ? 1 : 0));
    }

    private void HostOnOscMessageReceived(object sender, OscEventArgs e)
    {
      // reacts to track messages, extracting track number and name and adds it to 'Tracks' list
      var m = Regex.Match(e.Address, "/track(/(?<trackNo>[0-9]+))?/name");
      if (!m.Success)
        return;

      var trackNoStr = m.Groups["trackNo"].Value;
      var trackNo = String.IsNullOrEmpty(trackNoStr) ? 0 : int.Parse(trackNoStr);
      if (trackNo == 0)
        return;
      var trackName = (string) e.Args[0];

      var track = Tracks.FirstOrDefault(t => t.Id == trackNo);
      if (track == null)
      {
        track = new TrackInfo();
        Tracks.Add(track);
      }
      track.Id = trackNo;
      track.Name = trackName;
    }

    public void HostShutdown()
    {
      Host.OscMessageReceived -= HostOnOscMessageReceived;
      Host.MidiMessageReceived -= HostOnMidiMessageReceived;
    }

  }
}
