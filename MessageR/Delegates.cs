namespace Msgr
{

  public delegate void MsgrMessageReceivedHandler(object sender, MsgrMessage message);

  public delegate void MsgrClientConnectedHandler(object sender, IMsgrClient client);

}
