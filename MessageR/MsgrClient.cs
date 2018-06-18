using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Msgr
{

  public class MsgrClient : MsgrEndpoint
  {

    public string Hostname { get; }

    public int Port { get; }


    private Dictionary<string, Action<MsgrMessage>> Handlers { get; set; }


    public MsgrClient(string name, string hostname = "", int port = 0)
      : base(name, new TcpClient())
    {
      Handlers = new Dictionary<string, Action<MsgrMessage>>();
      Hostname = String.IsNullOrEmpty(hostname) ? "localhost" : hostname;
      Port = port == 0 ? MsgrServer.DefaultPort : port;

      MessageReceived += OnMessageReceived;
    }

    private void OnMessageReceived(object sender, MsgrMessage message)
    {
      var handlers = Handlers.Where(f => f.Key.Equals(message.Subject, StringComparison.OrdinalIgnoreCase));
      foreach (var handler in handlers)
        handler.Value(message);
    }


    public override void Connect()
    {
      Client.Connect(Hostname, Port);
      base.Connect();
      Send("", "ID");
    }

    public override void Send(MsgrMessage msg)
    {
      if (msg.Sender != Name)
        msg.Sender = Name;
      base.Send(msg);
    }

    public void On(string subject, Action<MsgrMessage> act)
    {
      Handlers.Add(subject, act);
    }

  }

}
