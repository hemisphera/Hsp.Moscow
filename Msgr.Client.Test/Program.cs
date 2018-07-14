using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessageR;

namespace Msgr.Client.Test
{
  class Program
  {

    static void Main(string[] args)
    {

      var cl = new MsgrClient("Schmoazi");
      cl.Connect();
      Thread.Sleep(500);
      cl.Send("Main", "Hello1");
      cl.Send("Main", "Hello2");
      cl.Send("Main", "Hello3");
      cl.Send("Main", "Hello4");
      cl.Send("Main", "Hello5");
      cl.Send("Main", "Hello6");
      var msg = "";
      while (msg != "exit")
      {
        msg = Console.ReadLine();
        cl.Send("", msg);
      }

      Console.WriteLine("done");
      Console.ReadLine();
    }

  }
}
