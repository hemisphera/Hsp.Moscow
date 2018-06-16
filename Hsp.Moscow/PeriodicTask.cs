using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hsp.Moscow
{

  internal class PeriodicTask
  {

    private readonly CancellationTokenSource Token = new CancellationTokenSource();

    private Task Task { get; set; }

    public TimeSpan Interval { get;set; }

    public Action AbortHandler { get; set; }


    public PeriodicTask(Action action)
      : this(action, TimeSpan.FromMilliseconds(250))
    {
    }

    public PeriodicTask(Action action, TimeSpan interval)
    {
      Interval = interval;
      Task = Task.Run(() =>
      {
        var running = true;
        while (running)
        {
          try
          {
            Token.Token.ThrowIfCancellationRequested();
            action();
            if (Interval != TimeSpan.Zero)
              Thread.Sleep(Interval);
          }
          catch (OperationCanceledException)
          {
            running = false;
          }
        }
      });
    }


    public void Abort()
    {
      AbortHandler?.Invoke();
      Token.Cancel();
      Task.Wait();
    }


  }

}
