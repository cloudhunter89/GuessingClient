using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessingClient.Messages
{
  public enum MessageType
  {
    NewGame = 1,
    GameDef,
    Guess,
    Answer,
    GetHint,
    Hint,
    Exit,
    Ack,
    Error,
    HeartBeat,
    InvalidMessageType
  }
}
