using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageR;

namespace Msgr.Server.Test
{
  class Program
  {
    static void Main(string[] args)
    {

      var sv = new MsgrServer();
      //sv.ClientConnected += (s, e) => { Console.WriteLine($"Client {e.Name} connected."); };
      sv.MessageReceived += (s, e) => Console.WriteLine($"{e.Sender} to {e.Recipient}: {e.Subject}");
      Console.ReadLine();

    }
  }
}
