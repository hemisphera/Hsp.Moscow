namespace Msgr
{

  public interface IMsgrClient
  {

    string Name { get; }

    void Send(MsgrMessage msg);

  }

}
