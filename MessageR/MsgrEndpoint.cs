using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Xsl;

namespace Msgr
{

  public class MsgrEndpoint : IMsgrClient
  {

    public string Name { get; }

    protected TcpClient Client { get; set; }

    private CancellationTokenSource CancellationToken { get; set; }


    public event EventHandler Disconnected;

    public event MsgrMessageReceivedHandler MessageReceived;


    public MsgrEndpoint(string name, TcpClient client)
    {
      Client = client;
      Name = name;
    }


    public virtual void Connect()
    {
      CancellationToken = new CancellationTokenSource();
      Task.Run(() =>
      {
        var msgBuffer = new MessageBuffer();
        var s = Client.GetStream();

        var cont = true;
        while (cont)
        {
          try
          {
            CancellationToken.Token.ThrowIfCancellationRequested();
            var buffer = new byte[Client.ReceiveBufferSize];
            var read = s.Read(buffer, 0, buffer.Length);
            msgBuffer.Write(buffer, read);
          }
          catch (OperationCanceledException)
          {
            cont = false;
          }
          catch (IOException)
          {
            cont = false;
          }

          if (!cont)
            Disconnect();

          while (msgBuffer.TryGetMessage(out var msg))
            MessageReceived?.Invoke(this, msg);
        }
      });
    }

    public void Disconnect()
    {
      CancellationToken.Cancel();
      Client.Close();
      Disconnected?.Invoke(this, new EventArgs());
    }


    public void Send(string recipient, string subject, byte[] data = null)
    {
      var msg = new MsgrMessage(this, recipient, subject, data);
      Send(msg);
    }

    public virtual void Send(MsgrMessage msg)
    {
      lock (Client)
        Client.GetStream().WriteBlock(msg.ToData());
    }
    
  }

}
