using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Msgr
{

  internal static class Extensions
  {

    public static void ReadAssert(this BinaryReader br, byte value)
    {
      var r = br.ReadByte();
      if (r != value)
        throw new FormatException();
    }

    public static void WriteBlock(this BinaryWriter bw, byte[] data)
    {
      var dataLength = data?.Length ?? 0;
      bw.Write(dataLength);
      if (dataLength > 0 && data != null)
        bw.Write(data);
    }

    public static void WriteBlock(this BinaryWriter bw, string data)
    {
      bw.WriteBlock(Encoding.UTF8.GetBytes(data));
    }

    public static void WriteBlock(this Stream s, byte[] data)
    {
      s.Write(data, 0, data.Length);
    }

    public static byte[] ReadBlock(this BinaryReader br)
    {
      var dataLength = br.ReadInt32();
      if (dataLength > 0)
        return br.ReadBytes(dataLength);
      return new byte[] { };
    }

    public static string ToUtf8String(this byte[] data)
    {
      return Encoding.UTF8.GetString(data);
    }

  }

}
