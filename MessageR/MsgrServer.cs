using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;

namespace Msgr
{

  public class MsgrServer : IDisposable
  {

    public const int DefaultPort = 8889;

    private TcpListener Server { get; set; }

    private List<IMsgrClient> ClientsList { get; set; }

    private CancellationTokenSource CancellationToken { get; set; }


    public ReadOnlyCollection<IMsgrClient> Clients => ClientsList.AsReadOnly();
    
    public int Port { get; }


    public event MsgrClientConnectedHandler ClientConnected;
    
    public event MsgrClientConnectedHandler ClientDisconnected;

    public event MsgrMessageReceivedHandler MessageReceived;


    public MsgrServer(int port = 0)
    {
      ClientsList = new List<IMsgrClient>();

      if (port == 0)
        port = DefaultPort;
      Port = port;
    }


    private void ClOnMessageReceived(object sender, MsgrMessage message)
    {
      MessageReceived?.Invoke(sender, message);
      var targets = Clients
        .Where(c => String.IsNullOrEmpty(message.Recipient) || c.Name.Equals(message.Recipient))
        .ToArray();
      foreach (var target in targets)
        target.Send(message);
    }
        
    private void ClDisconnected(object sender, EventArgs args)
    {
      if (!(sender is MsgrEndpoint ep))
        return;
      ClientsList.Remove(ep);
      ClientDisconnected?.Invoke(this, ep);
    }


    public void Start()
    {
      Task.Run(() =>
      {
        Server = new TcpListener(IPAddress.Any, Port);
        CancellationToken = new CancellationTokenSource();
        var server = Server;
        server.Start();
        var running = true;
        while (running)
        {
          try
          {
            CancellationToken.Token.ThrowIfCancellationRequested();
            var client = server.AcceptTcpClient();
            var msg = MsgrMessage.FromStream(client.GetStream());
            var cl = new MsgrEndpoint(msg.Sender, client);
            cl.MessageReceived += ClOnMessageReceived;
            cl.Disconnected += ClDisconnected;
            cl.Connect();
            ClientConnected?.Invoke(this, cl);
            ClientsList.Add(cl);
          }
          catch (OperationCanceledException)
          {
            running = false;
          }
          catch (ObjectDisposedException)
          {
            running = false;
          }
        }
        Stop();
      });
    }

    public void Stop()
    {
      Server?.Stop();
      Server = null;
    }

    public void Dispose()
    {
      Stop();
    }
  }

}
