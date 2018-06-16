namespace Hsp.Moscow.Extensibility
{

  public interface IMidiMessage
  {
    
    int Channel { get; }
  
    int Status { get; }

    int Data1 { get; }
    
    int Data2 { get; }

  }

}