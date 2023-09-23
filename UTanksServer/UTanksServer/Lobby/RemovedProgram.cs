using System;
using System.Collections.Generic;
using SimpleJSON;
using UTanksServer.Database.Databases;
using Core;
using UTanksServer.Network.Simple.Net;
using UTanksServer.Network.NetworkEvents.Communications;
using UTanksServer.Network.NetworkEvents.Security;
using UTanksServer.Network.NetworkEvents.PlayerAuth;
using UTanksServer.Network.NetworkEvents.PlayerSettings;
using System.Text;

namespace UTanksServer.Database
{
    static class Program2
    {
        public static JSONFile Config;
        static void Main2(string[] args)
        {
            Console.Title = "JTDatabase";
            Console.WriteLine(@"
      _   _____     ____    _____   ____   __     __  _____   ____  
     | | |_   _|   / ___|  | ____| |  _ \  \ \   / / | ____| |  _ \ 
  _  | |   | |     \___ \  |  _|   | |_) |  \ \ / /  |  _|   | |_) |
 | |_| |   | |      ___) | | |___  |  _ <    \ V /   | |___  |  _ < 
  \___/    |_|     |____/  |_____| |_| \_\    \_/    |_____| |_| \_\
                                                                    
");

            Config = new JSONFile("config.json").Load();
            if (Config["Logger"]["PrintDebugVerbose"].AsBool) Console.WriteLine("The server is set to display debug messages, to disable set 'Logger.PrintDebugVerbose' to false (in config)\n");
            Logger.Log("Config loaded!", "init");
            ServerDatabase.Load();
            UserDatabase.Load();
            Networking.Start();
            new UTServer();
            RunInput();
        }

        static async void RunInput()
        {
            while (true)
            {
                string[] input = CreateArgs(Console.ReadLine());
                if (input.Length == 0) input = new string[] { string.Empty };
                switch (input[0].ToLower())
                {
                    case "exit":
                        Networking.Stop();
                        return;
                    case "txserver":
                        if (input.Length >= 2)
                        {
                            switch (input[1].ToLower())
                            {
                                case "kill":
                                case "exit":
                                case "stop":
                                    if (UTServer.Instance.IsRunning)
                                        UTServer.Instance?.Destroy(false);
                                    else Console.WriteLine("TXServer is not running");
                                    break;
                                case "start":
                                case "run":
                                    if (!UTServer.Instance.IsRunning)
                                        new UTServer();
                                    else Console.WriteLine("TXServer is already running");
                                    break;
                                case "restart":
                                case "reboot":
                                    if (UTServer.Instance.IsRunning)
                                        UTServer.Instance.Destroy(true);
                                    else new UTServer();
                                    break;
                                case "enable":
                                    UTServer.Config["EnableModule"] = true;
                                    Console.WriteLine("TXServer Enabled");
                                    break;
                                case "disable":
                                    UTServer.Config["EnableModule"] = false;
                                    Console.WriteLine("TXServer Disabled");
                                    if (UTServer.Instance.IsRunning)
                                        UTServer.Instance.Destroy(false);
                                    break;
                                default:
                                    Console.WriteLine($"Unknown command '{input[1]}', to view a list of options, use 'TXServer'");
                                    break;
                            }
                        }
                        else
                            Console.WriteLine("TXServer Commands:" +
                                "\n  kill - Stops the server (alias: exit, stop)" +
                                "\n  start - Starts the server (alias: run)" +
                                "\n  enable - Enable the excecution of the server (session-only)" +
                                "\n  disable - Disabled the server and kills the process (if any, session-only)"
                            );
                        break;
                    case "addserver":
                        if (input.Length < 5)
                        {
                            Console.WriteLine($"Not enough parameters! (Need: 5 got {input.Length})\nSyntax: addserver <string: name> <string: key> <string: token> <string: addresses>");
                            break;
                        }
                        Console.WriteLine("Validating and creating credentials...");
                        ServerRow serverRow = new ServerRow
                        {
                            name = input[1],
                            key = input[2],
                            token = input[3],
                            addresses = new List<string>(input[4].Split(' '))
                        };
                        ServerRow otherServer = await ServerDatabase.Servers.Get(HashUtil.MD5(serverRow.key));
                        if (otherServer != ServerRow.Empty)
                        {
                            Console.WriteLine($"Unable to create login credentials, key is used by '{otherServer.name}' (sid: {otherServer.uid})");
                            break;
                        }
                        Console.WriteLine($"Confirm Info (ignore uid): {serverRow}");

                        bool createAccount = false;
                        while (true)
                        {
                            Console.Write("\nConfirm (Y/n): ");
                            ConsoleKeyInfo response = Console.ReadKey();
                            if (response.Key == ConsoleKey.Y || response.Key == ConsoleKey.N || response.Key == ConsoleKey.Enter)
                            {
                                createAccount = response.Key == ConsoleKey.Enter ? true : response.Key == ConsoleKey.Y;
                                if (response.Key != ConsoleKey.Enter) Console.WriteLine();
                                break;
                            }
                        }
                        serverRow.key = HashUtil.MD5(serverRow.key);
                        serverRow.token = HashUtil.Compute(serverRow.token);
                        if (!createAccount)
                            Console.WriteLine("Canceled");
                        else if (await ServerDatabase.Servers.Create(serverRow))
                            Console.WriteLine($"Created login creds for '{serverRow.name}'. Now enter them in your server config, and boot it up!");
                        else Console.WriteLine("Failed to create login creds. Database error");
                        break;
                    case "help":
                        Console.WriteLine("Useful Commands" +
                            "\n  addserver - Create login credentials to the DB" +
                            "\n  exit - Disconnect all clients and quit" +
                            "\n  TXServer - Modify the TXServer process");
                        break;
                    default:
                        Console.WriteLine($"Unknown Command '{input[0]}'");
                        break;
                }
            }
        }

        public static string[] CreateArgs(string commandLine)
        {
            StringBuilder argsBuilder = new StringBuilder(commandLine);
            bool inQuote = false;

            // Convert the spaces to a newline sign so we can split at newline later on
            // Only convert spaces which are outside the boundries of quoted text
            for (int i = 0; i < argsBuilder.Length; i++)
            {
                if (argsBuilder[i].Equals('"'))
                {
                    inQuote = !inQuote;
                }

                if (argsBuilder[i].Equals(' ') && !inQuote)
                {
                    argsBuilder[i] = '\n';
                }
            }

            // Split to args array
            string[] args = argsBuilder.ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Clean the '"' signs from the args as needed.
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = ClearQuotes(args[i]);
            }

            return args;
        }
        static string ClearQuotes(string stringWithQuotes)
        {
            int quoteIndex;
            if ((quoteIndex = stringWithQuotes.IndexOf('"')) == -1)
            {
                // String is without quotes..
                return stringWithQuotes;
            }

            // Linear sb scan is faster than string assignemnt if quote count is 2 or more (=always)
            StringBuilder sb = new StringBuilder(stringWithQuotes);
            for (int i = quoteIndex; i < sb.Length; i++)
            {
                if (sb[i].Equals('"'))
                {
                    // If we are not at the last index and the next one is '"', we need to jump one to preserve one
                    if (i != sb.Length - 1 && sb[i + 1].Equals('"'))
                    {
                        i++;
                    }

                    // We remove and then set index one backwards.
                    // This is because the remove itself is going to shift everything left by 1.
                    sb.Remove(i--, 1);
                }
            }

            return sb.ToString();
        }
    }
}
