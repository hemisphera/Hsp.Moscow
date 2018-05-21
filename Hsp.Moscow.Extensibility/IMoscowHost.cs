using System;
using System.Net.Configuration;

namespace Hsp.Moscow.Extensibility
{

  public interface IMidiToOscHost
  {

    event EventHandler<MidiEventArgs> MidiMessageReceived;

    event EventHandler<OscEventArgs> OscMessageReceived; 


    void SendOscMessage(OscEventArgs msg);

  }

}