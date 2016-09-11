using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessingClient.Messages
{
  public class ErrorMessage : Message
  {
    public Int16 GameId { get; set; }
    public string ErrorText { get; set; }

    public ErrorMessage()
    {
      Type = MessageType.Error;
      GameId = -1;
      ErrorText = string.Empty;
    }

    public override void Decode(Byte[] buffer)
    {
      Type = getType(buffer, buffer.Count());
      if (Type == MessageType.Error)
      {
        Int16 nextStringOffset = 2;
        GameId = getShort(buffer, nextStringOffset);
        nextStringOffset += sizeof(Int16);
        ErrorText = decodeString(buffer, ref nextStringOffset);
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
      encodeList.AddRange(encodeString(ErrorText));

      return encodeList.ToArray();
    }
  }
}
