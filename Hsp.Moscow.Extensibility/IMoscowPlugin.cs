namespace Hsp.Moscow.Extensibility
{

  public interface IMidiToOscPlugin
  {

    string Name { get; }

    void HostStartup(IMidiToOscHost host);

    void HostShutdown();

  }

}