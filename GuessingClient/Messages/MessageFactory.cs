using System;
using log4net;

namespace GuessingClient.Messages
{
  public class MessageFactory
  {
    public Message decodeMessage(byte[] msgBuffer, Int16 size)
    {
      Message msg = createMessage(Message.getType(msgBuffer, size));

      msg?.Decode(msgBuffer);
      return msg;
    }

    public Message createMessage(MessageType msgType)
    {
      Message msg;
      switch (msgType)
      {
        case MessageType.NewGame:
          log.DebugFormat("Created new message with type {0}", MessageType.NewGame);
          msg = new NewGameMessage();
          break;
        case MessageType.GameDef:
          log.DebugFormat("Created new message with type {0}", MessageType.GameDef);
          msg = new GameDefMessage();
          break;
        case MessageType.Guess:
          log.DebugFormat("Created new message with type {0}", MessageType.Guess);
          msg = new GuessMessage();
          break;
        case MessageType.Answer:
          log.DebugFormat("Created new message with type {0}", MessageType.Answer);
          msg = new AnswerMessage();
          break;
        case MessageType.GetHint:
          log.DebugFormat("Created new message with type {0}", MessageType.GetHint);
          msg = new GetHintMessage();
          break;
        case MessageType.Hint:
          log.DebugFormat("Created new message with type {0}", MessageType.Hint);
          msg = new HintMessage();
          break;
        case MessageType.Exit:
          log.DebugFormat("Created new message with type {0}", MessageType.Exit);
          msg = new ExitMessage();
          break;
        case MessageType.Ack:
          log.DebugFormat("Created new message with type {0}", MessageType.Ack);
          msg = new AckMessage();
          break;
        case MessageType.Error:
          log.DebugFormat("Created new message with type {0}", MessageType.Ack);
          msg = new ErrorMessage();
          break;
        case MessageType.HeartBeat:
          log.DebugFormat("Created new message with type {0}", MessageType.HeartBeat);
          msg = new HeartbeatMessage();
          break;
        case MessageType.InvalidMessageType:
        default:
          log.DebugFormat("Invalid request: {0} returning null", msgType);
          msg = null;
          break;
      }

      return msg;
    }

    private static readonly ILog log = LogManager.GetLogger(typeof(MessageFactory));
  }
}
