using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hsp.Moscow.Extensibility
{

  public class OscEventArgs : EventArgs
  {

    public string Address { get; }
    
    public object[] Args { get; }

    public OscEventArgs(string address, params object[] args)
    {
      Address = address;
      Args = args;
    }

    public override string ToString()
    {
      var argsStr = String.Join(",", Args);
      return $"{Address}: [{argsStr}]";
    }
  }

}
