namespace Hsp.Moscow.Extensibility
{

  /// <summary>
  /// The interface that every Moscow plugin needs to implement in order to be able to be used in Moscow
  /// </summary>
  public interface IMoscowPlugin
  {

    /// <summary>
    /// Provides a name for the plugin
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets called by Moscow at startup, after it has completed its internal configuration. MIDI and OSC devices have already been set up.
    /// </summary>
    /// <param name="host"></param>
    void HostStartup(IMoscowHost host);

    /// <summary>
    /// Gets called by Moscow when its about to be shut down. You should release resources here, dispose stuff and such.
    /// </summary>
    void HostShutdown();

  }

}