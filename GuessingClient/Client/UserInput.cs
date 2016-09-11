using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GuessingClient
{
  public class UserInput
  {
    private bool reading = false;
    ClientCommander commander;
    Thread readThread = null;

    public UserInput(ClientCommander commander)
    {
      this.commander = commander;
      readThread = new Thread(new ThreadStart(GetInput));
    }

    public void Start()
    {
      reading = true;
      GetInput();
    }

    public void Stop()
    {
      reading = false;
      Console.Write("Press return to exit...");
    }

    void GetInput()
    {
      while (reading)
      {
        string input = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(input))
        {
          if (input[0] == '\\')
          {
            commander.HandleCommand(input);
          }
          else
          {
            commander.SendGuess(input);
          }
        }
      }
    }
  }
}
