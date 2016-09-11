using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessingClient.Messages
{
  class HintMessage : Message
  {
    public Int16 GameId { get; set; }
    public string Hint { get; set; }

    public HintMessage()
    {
      Type = MessageType.Hint;
      GameId = -1;
      Hint = string.Empty;
    }

    public override void Decode(Byte[] buffer)
    {
      Type = getType(buffer, buffer.Count());
      if (Type == MessageType.Hint)
      {
        log.Debug("Beginning Decode of MessageType.Hint");
        Int16 nextStringOffset = 2;
        GameId = getShort(buffer, nextStringOffset);
        nextStringOffset += sizeof(Int16);
        Hint = decodeString(buffer, ref nextStringOffset);
      }
      else
      {
        log.ErrorFormat("{0}:  Attempt to decode a message of type {1} refused", GetType(), Type);
      }
    }

    public override Byte[] Encode()
    {
      List<byte> encodeList = new List<byte>();
      encodeList.AddRange(base.Encode());
      encodeList.AddRange(getBytes(GameId));
      encodeList.AddRange(encodeString(Hint));

      return encodeList.ToArray();
    }
  }
}
