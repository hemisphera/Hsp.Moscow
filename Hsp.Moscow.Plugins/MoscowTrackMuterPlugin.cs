using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

    private void HostOnMidiMessageReceived(string device, IMidiMessage message)
    {
      if (message.Status != 12) // program change
        return;

      var tracks = GetTracks(message.Channel);
      foreach (var trk in tracks)
        MuteTrack(trk, trk.Program != message.Data1);
    }

    private void MuteTrack(TrackInfo track, bool isMute)
    {
      if (track.IsMuted == isMute)
        return;

      // Mute / Unmute track in REAPER
      Host.SendOscMessage(new OscEventArgs($"/track/{track.Id}/mute", isMute ? 1 : 0));
    }

    private bool FindTrack(string trackNoStr, out TrackInfo track)
    {
      track = null;

      if (!int.TryParse(trackNoStr, out var trackNo))
        return false;

      track = Tracks.FirstOrDefault(t => t.Id == trackNo);
      if (track == null)
      {
        track = new TrackInfo
        {
          Id = trackNo
        };
        Tracks.Add(track);
      }

      return true;
    }

    private void HostOnOscMessageReceived(object sender, OscEventArgs e)
    {
      // reacts to track messages, extracting track number and name and adds it to 'Tracks' list
      var m = Regex.Match(e.Address, "/track(/(?<trackNo>[0-9]+))?/(?<param>[a-z]+)");
      if (m.Success)
      {
        var trackNoStr = m.Groups["trackNo"].Value;
        if (!FindTrack(trackNoStr, out var track))
          return;
        
        if (m.Groups["param"].Value.Equals("name", StringComparison.OrdinalIgnoreCase))
          track.Name = (string) e.Args[0];
        if (m.Groups["param"].Value.Equals("mute", StringComparison.OrdinalIgnoreCase))
          if (int.TryParse($"{e.Args[0]}", out var isMutedVal))
            track.IsMuted = isMutedVal == 1;
      }
    }

    private TrackInfo[] GetTracks(int channel)
    {
      return Tracks.Where(t => t.Channel == channel).ToArray();
    }

    public void HostShutdown()
    {
      Host.OscMessageReceived -= HostOnOscMessageReceived;
      Host.MidiMessageReceived -= HostOnMidiMessageReceived;
    }

  }

}
