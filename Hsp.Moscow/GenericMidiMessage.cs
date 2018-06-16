using Hsp.Moscow.Extensibility;

namespace Hsp.Moscow
{

  class GenericMidiMessage : IMidiMessage
  {

    public int Channel { get; set; }
    
    public int Status { get; set; }
    
    public int Data1 { get; set; }
    
    public int Data2 { get; set; }

  }

}
