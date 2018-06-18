using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Msgr
{

  public class MsgrMessage
  {

    public string Sender { get; internal set; }

    public string Subject { get; private set; }
    
    /// <summary>
    /// Indicates the recipient of the message.
    /// If this is null or empty, the message is broadcast to all clients.
    /// </summary>
    public string Recipient { get; private set; }

    public byte[] Data { get; private set; }


    private MsgrMessage()
    {
    }

    public MsgrMessage(string recipient, string subject, byte[] data = null)
    {
      Subject = subject;
      Recipient = recipient;
      Data = data;
    }

    internal MsgrMessage(IMsgrClient endpoint, string recipient, string subject, byte[] data)
      : this(recipient, subject, data)
    {
      Sender = endpoint.Name;
    }


    public static MsgrMessage FromStream(Stream s)
    {
      using (var br = new BinaryReader(s, Encoding.UTF8, true))
      {
        br.ReadAssert((byte) 0x02);
        var sender = br.ReadBlock().ToUtf8String();
        var recipient = br.ReadBlock().ToUtf8String();
        var subject = br.ReadBlock().ToUtf8String();
        var data = br.ReadBlock();
        br.ReadAssert((byte) 0x03);

        var msg = new MsgrMessage
        {
          Recipient = recipient,
          Subject = subject,
          Data = data,
          Sender = sender
        };
        return msg;
      }
    }

    public byte[] ToData()
    {
      using (var ms = new MemoryStream())
      using (var bw = new BinaryWriter(ms))
      {
        bw.Write((byte) 0x02);
        bw.WriteBlock(Sender);
        bw.WriteBlock(Recipient);
        bw.WriteBlock(Subject);
        bw.WriteBlock(Data);
        bw.Write((byte) 0x03);

        bw.Flush();
        return ms.ToArray();
      }
    }

  }

}
