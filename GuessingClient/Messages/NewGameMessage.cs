using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessingClient.Messages
{
  public class NewGameMessage : Message
  {
    public string ANumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public NewGameMessage()
    {
      Type = MessageType.NewGame;
      ANumber = "";
      FirstName = "";
      LastName = "";
    }

    public override void Decode(Byte[] buffer)
    {
      Type = getType(buffer, buffer.Count());
      if (Type == MessageType.NewGame)
      {
        log.Debug("Beginning Decode of MessageType.NewGame");
        Int16 nextStringOffset = 2;
        ANumber = decodeString(buffer, ref nextStringOffset);
        FirstName = decodeString(buffer, ref nextStringOffset);
        LastName = decodeString(buffer, ref nextStringOffset);
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
      encodeList.AddRange(encodeString(ANumber));
      encodeList.AddRange(encodeString(FirstName));
      encodeList.AddRange(encodeString(LastName));

      return encodeList.ToArray();
    }
  }
}
