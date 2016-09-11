﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessingClient.Messages
{
  class ExitMessage : Message
  {
    public Int16 GameId { get; set; }

    public ExitMessage()
    {
      Type = MessageType.Exit;
      GameId = -1;
    }

    public override void Decode(Byte[] buffer)
    {
      Type = getType(buffer, buffer.Count());
      if (Type == MessageType.Exit)
      {
        Int16 nextStringOffset = 2;
        GameId = getShort(buffer, nextStringOffset);
        nextStringOffset += sizeof(Int16);
      }
      else
      {
        log.ErrorFormat("{0}:  Attempt to decode a message of type {1} refused",GetType(), Type);
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
