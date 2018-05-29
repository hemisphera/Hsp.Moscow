using System.Text.RegularExpressions;

namespace Hsp.Moscow.Plugins
{

  internal class TrackInfo
  {

    private static readonly Regex NameRegex = new Regex("Ch(?<mc>[0-9]{2})-PC(?<mpc>[0-9]{3})", RegexOptions.Compiled);


    private string _name;


    public int Id { get; set; }

    public string Name
    {
      get => _name;
      internal set
      {
        _name = value;
        ParseName();
      }
    }

    public int Channel { get; private set; }
    
    public int Program { get; private set; }

    public bool IsMuted { get; internal set; }


    public TrackInfo()
    {
      Channel = -1;
      Program = -1;
    }


    private void ParseName()
    {
      // Find all tracks that have the string 'ChXX-PCYYY' in it's name.
      // where XX is for the midi channel and YYY for the program change
      // Whenever a MIDI program change is received, all tracks on that channel are muted, except the one where YYY matches the program change received
      var m = NameRegex.Match(Name);
      var channel = m.Success ? int.Parse(m.Groups["mc"].Value) : -1;
      var programChange = m.Success ? int.Parse(m.Groups["mpc"].Value) : -1;

      Channel = channel;
      Program = programChange;
    }

  }

}