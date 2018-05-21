namespace Hsp.Moscow.Extensibility
{

  public class MidiEventArgs
  {

    public int Channel { get; }
    
    public int Command { get; }
    
    public int Data1 { get; }
    
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
      return $"Ch{Channel:D2} - {Command:D2}, {Data1:D3}, {Data2:3}";
    }
  }

}