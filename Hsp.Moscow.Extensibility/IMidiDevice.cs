using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hsp.Moscow.Extensibility
{

  public interface IMidiDevice
  {

    MidiDeviceType Type { get; }

    string Name { get; }

  }

}
