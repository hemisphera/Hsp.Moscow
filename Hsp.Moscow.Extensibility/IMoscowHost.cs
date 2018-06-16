using System;

namespace Hsp.Moscow.Extensibility
{

  /// <summary>
  /// Provides an interface to Moscow to the plugin
  /// </summary>
  public interface IMoscowHost
  {

    /// <summary>
    /// Gets called whenever Moscow receives a MIDI message on its configured MIDI input device
    /// </summary>
    event MidiMessageReceivedHandler MidiMessageReceived;

    /// <summary>
    /// Gets called whenever Moscow receives a OSC message on its configured OSC input port
    /// </summary>
    event EventHandler<OscEventArgs> OscMessageReceived;

    /// <summary>
    /// Sends an OSC message to the OSC host that Moscow is connected to
    /// </summary>
    /// <param name="msg">The OSC message to be sent</param>
    void SendOscMessage(OscEventArgs msg);

  }

}