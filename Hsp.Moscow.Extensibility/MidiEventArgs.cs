namespace Hsp.Moscow.Extensibility
{

  /// <summary>
  /// Represents a MIDI message
  /// </summary>
  public class MidiEventArgs
  {

    /// <summary>
    /// The MIDI channel
    /// </summary>
    public int Channel { get; }
    
    /// <summary>
    /// The MIDI command. NoteOn, NoteOff, CC, ProgramChange ecc.
    /// </summary>
    public int Command { get; }
    
    /// <summary>
    /// Data1 of the message
    /// </summary>
    public int Data1 { get; }

    /// <summary>
    /// Data2 of the message
    /// </summary>
    public int Data2 { get; }


    public MidiEventArgs(int channel, int command, int data1, int data2)
    {
      Channel = channel;
      Command = command;
      Data1 = data1;
      Data2 = data2;
    }


    public override string ToString()
    {
      return $"Ch{Channel:D2} - {Command:D2} - {Data1:D3}, {Data2:D3}";
    }

  }

}