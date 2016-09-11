using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessingClient.Messages
{
  public class GuessMessage : Message
  {
    public Int16 GameId { get; set; }
    public string Guess { get; set; }

    public GuessMessage()
    {
      Type = MessageType.Guess;
      GameId = -1;
      Guess = string.Empty;
    }

    public override void Decode(Byte[] buffer)
    {
      Type = getType(buffer, (Int16)buffer.Count());
      if (Type == MessageType.Guess)
      {
        log.Debug("Beginning Decode of MessageType.Guess");
        Int16 nextStringOffset = 2;
        GameId = getShort(buffer, nextStringOffset);
        nextStringOffset += sizeof(Int16);
        Guess = decodeString(buffer, ref nextStringOffset);
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
      encodeList.AddRange(encodeString(Guess));

      return encodeList.ToArray();
    }
  }
}
