using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessingClient.Messages
{
  public class AnswerMessage : Message
  {
    public Int16 GameId { get; set; }
    public byte Result { get; set; }
    public Int16 Score { get; set; }
    public string Hint { get; set; }

    public AnswerMessage()
    {
      Type = MessageType.Answer;
      GameId = -1;
      Result = 0x02;
      Score = -1;
      Hint = "";
    }

    public override void Decode(Byte[] buffer)
    {
      Type = getType(buffer, (Int16)buffer.Count());
      if (Type == MessageType.Answer)
      {
        log.DebugFormat("Beginning Decode of MessageType.Answer");
        Int16 nextItemOffset = 2;
        GameId = getShort(buffer, nextItemOffset);
        nextItemOffset += sizeof(Int16);
        Result = buffer[nextItemOffset++];
        Score = getShort(buffer, nextItemOffset);
        nextItemOffset += sizeof(Int16);
        Hint = decodeString(buffer, ref nextItemOffset);
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
      encodeList.Add(Result);
      encodeList.AddRange(getBytes(Score));
      encodeList.AddRange(encodeString(Hint));

      return encodeList.ToArray();
    }
  }
}
