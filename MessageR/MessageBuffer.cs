using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Msgr
{

  internal class MessageBuffer
  {

    private MemoryStream Buffer { get; set; }

    public MessageBuffer()
    {
      Buffer = new MemoryStream();
    }


    
    public void Write(byte[] data, int length)
    {
      Buffer.Write(data, 0, length);
    }

    public bool TryGetMessage(out MsgrMessage message)
    {
      try
      {
        Buffer.Position = 0;
        message = MsgrMessage.FromStream(Buffer);
        var messageLength = message.ToData().Length;
        var remainingBuffer = Buffer.ToArray().Skip(messageLength).ToArray();
        Buffer = new MemoryStream();
        Buffer.Write(remainingBuffer, 0, remainingBuffer.Length);
        return true;
      }
      catch
      {
        message = null;
        return false;
      }
      finally
      {
        Buffer.Position = Buffer.Length;
      }
    }


  }

}
