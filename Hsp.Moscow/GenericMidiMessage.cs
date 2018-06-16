using Hsp.Moscow.Extensibility;

namespace Hsp.Moscow
{

  class GenericMidiMessage : IMidiMessage
  {

    public int Channel { get; set; }
    
    public int Status { get; set; }
    
    public int Data1 { get; set; }
    
    public int Data2 { get; set; }

    public override string ToString()
    {
      return $"Ch{Channel + 1} - {Status:X2} - {Data1:D3} {Data2:D3}";
    }
  }

}
