using System;
using System.Configuration;
using System.Net;
using GuessingClient.Messages;
using GuessingClient.Network;
using log4net;

namespace GuessingClient
{
  public class ClientCommander
  {
    enum Command
    {
      NewGame = 'n',
      GetHint = 'h',
      Quit    = 'q',
      Exit    = 'e'
    }

    public Int16 GameId;

    UdpCommunicator sender = null;
    IPEndPoint serverEP = null;

    public ClientCommander()
    {
      UInt16 port;
      Int32 timeout;
      IPAddress serverIP;
      if (UInt16.TryParse(Client.AppSettings["Port"], out port))
      {
        if (Int32.TryParse(Client.AppSettings["Timeout"], out timeout))
        {
          if (IPAddress.TryParse(Client.AppSettings["Server"], out serverIP))
          {
            serverEP = new IPEndPoint(serverIP, UInt16.Parse(Client.AppSettings["Port"]));
            sender = new Network.UdpCommunicator(0, timeout);
            log.DebugFormat("Setting up to send messages to {0}", serverEP);
          }
          else
            log.Error("Failed to parse server IP address from " + Client.AppSettings["Server"]);
        }
        else
          log.Error("Failed to parse an Int32 value from " + Client.AppSettings["Timeout"]);
      }
      else
        log.Error("Failed to parse an UInt16 from " + Client.AppSettings["Port"]);
    }

    public void SendMessage(Message msg)
    {
      if (sender != null && serverEP != null)
        sender.write(msg, serverEP);
    }

    public bool IsReady()
    {
      return (sender != null && serverEP != null);
    }

    public void SendGuess(string guess)
    {
      Messages.GuessMessage message = new Messages.GuessMessage();
      message.GameId = GameId;
      message.Guess = guess;

      if (sender != null && serverEP != null)
        sender.write(message, serverEP);
    }

    public void HandleCommand(string command)
    {
      Message msg = null;
      switch ((Command)command[1])
      {
        case Command.NewGame:
          msg = new NewGameMessage()
          {
            ANumber = ConfigurationManager.AppSettings["Anumber"],
            FirstName = ConfigurationManager.AppSettings["FirstName"],
            LastName = ConfigurationManager.AppSettings["LastName"],
          };
          break;
        case Command.GetHint:
          msg = new GetHintMessage()
          {
            GameId = GameId
          };
          break;
        case Command.Exit:
        case Command.Quit:
          msg = new ExitMessage()
          {
            GameId = GameId
          };
          break;
      }

      if (msg != null)
      {
        if (sender != null && serverEP != null)
        {
          log.DebugFormat("Sending message of type {0} to server at {1}", msg.GetType(), serverEP);
          sender.write(msg, serverEP);
        }
        else
          log.ErrorFormat("Attempted to send a message to EndPoint {0} and UdpCommunicator {1}", serverEP, sender);
      }
      else
      {
        log.ErrorFormat("Unrecognized command: {0}", command);
      }
    }

    public Message ReadMsg()
    {
      return sender?.read();
    }

    protected static readonly ILog log = LogManager.GetLogger(typeof(ClientCommander));
  }
}
