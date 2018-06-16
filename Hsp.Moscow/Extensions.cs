using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rug.Osc;

namespace Hsp.Moscow
{

  internal static class Extensions
  {

    public static void On(this OscMessage msg, string address, object arg1, object arg2, Action<OscMessage> act)
    {
      msg.On(address, new[] { arg1, arg2 }, act);
    }

    public static void On(this OscMessage msg, string address, object arg, Action<OscMessage> act)
    {
      msg.On(address, new[] { arg }, act);
    }

    public static void On(this OscMessage msg, string address, object[] args, Action<OscMessage> act)
    {
      if (!msg.Address.Equals(address))
        return;

      var msgArgs = msg.ToList();
      if (args.Length < msgArgs.Count)
        msgArgs = msgArgs.Take(args.Length).ToList();
      while (msgArgs.Count < args.Length)
        msgArgs.Add(null);

      var matches = true;
      for (var i = 0; i < msgArgs.Count; i++)
      {
        matches = args[i] == null || args[i].Equals(msgArgs[i]);
        if (!matches)
          break;
      }

      if (matches)
        act(msg);
    }

  }

}
