using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessingClient.Messages
{
  public class AckMessage : Message
  {
    public Int16 GameId { get; set; }

    public AckMessage()
    {
      Type = MessageType.Ack;
      GameId = -1;
    }

    public override void Decode(Byte[] buffer)
    {
      Type = getType(buffer, buffer.Count());
      if (Type == MessageType.Ack)
      { 
        Int16 nextFieldOffset = 2;
        GameId = getShort(buffer, nextFieldOffset);
        nextFieldOffset += sizeof(Int16);
      }
      else
      {
        log.ErrorFormat("GuessMessage:  Attempt to decode a message of type {0} refused", Type);
      }
    }

    public override Byte[] Encode()
    {
      List<byte> encodeList = new List<byte>();
      encodeList.AddRange(base.Encode());
      encodeList.AddRange(getBytes(GameId));

      return encodeList.ToArray();
    }
  }
}
