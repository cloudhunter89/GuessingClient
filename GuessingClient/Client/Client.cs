using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuessingClient.Messages;
using GuessingClient.Network;
using log4net;

namespace GuessingClient
{
  public class Client
  {
    static public System.Collections.Specialized.NameValueCollection AppSettings = ConfigurationManager.AppSettings;
    static public string ConfigServerIP = "Server";
    static public string ConfigTimeout = "Timeout";
    static public string ConfigPort = "Port";
    
    public UserInput inputReader;
    Int16 GameId { get; set; }
    ClientCommander commander;
    Thread receiveThread;
    string definition;
    string hint;
    bool running;

    public Client()
    {      
      receiveThread = new Thread(new ThreadStart(Receive));
    }

    public void Run()
    {
      running = true;
      bool retry = true;
      while (retry)
      {
        GetSettings();
        commander = new ClientCommander();
        if (!commander.IsReady())
        {
          Console.Write("Would you like to re-enter settings? (Y or y to retry any other key will quit)");
          char choice = (char)Console.Read();
          Console.WriteLine();
          if (choice == 'y' || choice == 'Y')
          {
            retry = true;
          }
          else
            retry = false;
        }
        else
          retry = false;
      }

      if (commander.IsReady())
      {
        inputReader = new UserInput(commander);
        receiveThread.Start();
      }
      else
        Stop();
    }

    public void Stop()
    {
      running = false;
      if (receiveThread.ThreadState != ThreadState.Unstarted)
        receiveThread.Join();
    }

    private void HandleMessage(Message msg)
    {
      switch (msg.Type)
      {
        case MessageType.Ack:
          AckMessage ack = msg as AckMessage;
          if (ack != null)
          {
            HandleAcknowledge(ack);
          }
          else
          {
            log.Error("Message was null after safe cast to {0}" + typeof(AckMessage));
          }
          break;
        case MessageType.Answer:
          AnswerMessage answer = msg as AnswerMessage;
          if (answer != null)
          {
            HandleAnswer(answer);
          }
          else
          {
            log.Error("Message was null after safe cast to {0}" + typeof(AnswerMessage));
          }
          break;
        case MessageType.Error:
          ErrorMessage error = msg as ErrorMessage;
          if (error != null)
          {
            HandleError(error);
          }
          else
          {
            log.Error("Message was null after safe cast to {0}" + typeof(ErrorMessage));
          }
          break;
        case MessageType.GameDef:
          GameDefMessage newGame = msg as GameDefMessage;
          if (newGame != null)
          {
            HandleGameDefinition(newGame);
          }
          else
          {
            log.Error("Message was null after safe cast to {0}" + typeof(GameDefMessage));
          }
          break;
        case MessageType.Hint:
          HintMessage hintMsg = msg as HintMessage;
          if (hintMsg != null)
          {
            HandleHint(hintMsg);
          }
          else
          {
            log.Error("Message was null after safe cast to {0}" + typeof(HintMessage));
          }
          break;
        case MessageType.HeartBeat:
          HeartbeatMessage heartbeat = msg as HeartbeatMessage;
          if (heartbeat != null)
          {
            HandleHeartbeat(heartbeat);
          }
          else
          {
            log.Error("Message was null after safe cast to {0}" + typeof(HeartbeatMessage));
          }
          break;
      }

    }

    private void HandleHeartbeat(HeartbeatMessage heartBeat)
    {
      AckMessage ack = new AckMessage()
      {
        GameId = GameId
      };

      commander.SendMessage(ack);
    }

    private void HandleError(ErrorMessage error)
    {
      log.Error("Server sent error message:\n\t" + error.ErrorText);
      // Console.WriteLine("Server sent error message:\n\t" + error.ErrorText);
    }

    private void HandleGameDefinition(GameDefMessage def)
    {
      GameId = def.GameId;
      commander.GameId = def.GameId;
      if ((!definition?.Equals(def.Definition) ?? true) || (!hint?.Equals(def.Hint) ?? true))
      {
        definition = def.Definition;
        hint = def.Hint;
        UpdateView();
      }
    }

    private void HandleHint(HintMessage hintMsg)
    {

      if (!hint.Equals(hintMsg.Hint))
      {
        hint = hintMsg.Hint;
        UpdateView();
      }
    }

    private void HandleAnswer(AnswerMessage answerMsg)
    {
      if (answerMsg.Result == 0)
      {
        Console.WriteLine("Your Guess Is Incorrect");
        hint = answerMsg.Hint;
        UpdateView();
      }
      else
      {
        Console.WriteLine("\n CORRECT \n");
        Console.WriteLine("You scored {0} points", answerMsg.Score);
      }
    }

    private void UpdateView()
    {
      Console.WriteLine("\nDefinition: " + definition);
      Console.Write("Hint:  ");
      foreach (char letter in hint)
      {
        Console.Write("{0} ", letter);
      }
      Console.Write("\nGuess: ");
    }

    private void HandleAcknowledge(AckMessage ackMsg)
    {
      inputReader.Stop();
    }

    private void Receive()
    {
      Message readMsg;
      while (running)
      {
        readMsg = commander.ReadMsg();
        if (readMsg != null)
        {
          HandleMessage(readMsg);
        }
      }
    }

    private void GetSettings()
    {
      var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      var appSettings = config.AppSettings.Settings;
      string serverIp = appSettings[ConfigServerIP]?.Value;
      string port = appSettings[ConfigPort]?.Value;
      string timeout = appSettings[ConfigTimeout]?.Value;
      Console.Write("Server IP Address ({0}): ", serverIp);
      string input = Console.ReadLine();
      if (!string.IsNullOrWhiteSpace(input))
      {
        if (appSettings[ConfigServerIP] == null)
          appSettings.Add(ConfigServerIP, input);
        else
          appSettings[ConfigServerIP].Value = input;
      }

      Console.Write("Server Port ({0}): ", port);
      input = Console.ReadLine();
      if (!string.IsNullOrWhiteSpace(input))
      {
        if (appSettings[ConfigPort] == null)
          appSettings.Add(ConfigPort, input);
        else
          appSettings[ConfigPort].Value = input;
      }

      Console.Write("Receive Timeout ({0}): ", timeout);
      input = Console.ReadLine();
      if (!string.IsNullOrWhiteSpace(input))
      {
        if (appSettings[ConfigTimeout] == null)
          appSettings.Add(ConfigTimeout, input);
        else
          appSettings[ConfigTimeout].Value = input;
      }

      config.Save(ConfigurationSaveMode.Modified);
      ConfigurationManager.RefreshSection("appSettings");
    }

    private static readonly ILog log = LogManager.GetLogger(typeof(Client));
  }
}
