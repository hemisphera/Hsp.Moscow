using System;

namespace Hsp.Moscow.Extensibility
{

  /// <summary>
  /// Represents a OSC message
  /// </summary>
  public class OscEventArgs : EventArgs
  {

    /// <summary>
    /// The OSC address
    /// </summary>
    public string Address { get; }
    
    /// <summary>
    /// Arguments
    /// </summary>
    public object[] Args { get; }


    public OscEventArgs(string address, params object[] args)
    {
      Address = address;
      Args = args;
    }


    public T GetArg<T>(int index)
    {
      return (T) Args[index];
    }

    public override string ToString()
    {
      var argsStr = String.Join(",", Args);
      return $"{Address}: [{argsStr}]";
    }

  }

}
