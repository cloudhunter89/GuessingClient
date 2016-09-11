using System;
using System.Configuration;

namespace GuessingClient
{
  class Program
  {
    static void Main(string[] args)
    {
      GetUserInfo();
      Client guessClient = new Client();
      guessClient.Run();
      guessClient.inputReader.Start();
      guessClient.Stop();
    }

    static void GetUserInfo()
    {
      var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      var appSettings = config.AppSettings.Settings;
      string ANumber = appSettings["ANumber"]?.Value;
      string FirstName = appSettings["FirstName"]?.Value;
      string LastName = appSettings["LastName"]?.Value;
      Console.Write("A# ({0}): ", ANumber);
      string input = Console.ReadLine();
      if (!string.IsNullOrWhiteSpace(input))
      {
        if (appSettings["ANumber"] == null)
          appSettings.Add("ANumber", input);
        else
          appSettings["ANumber"].Value = input;
      }

      Console.Write("First Name ({0}): ", FirstName);
      input = Console.ReadLine();
      if (!string.IsNullOrWhiteSpace(input))
      {
        if (appSettings["FirstName"] == null)
          appSettings.Add("FirstName", input);
        else
          appSettings["FirstName"].Value = input;
      }

      Console.Write("Last Name ({0}): ", LastName);
      input = Console.ReadLine();
      if (!string.IsNullOrWhiteSpace(input))
      {
        if (appSettings["LastName"] == null)
          appSettings.Add("LastName", input);
        else
          appSettings["LastName"].Value = input;
      }

      config.Save(ConfigurationSaveMode.Modified);
      ConfigurationManager.RefreshSection("appSettings");
    }
  }
}
