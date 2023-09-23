using System;
using System.IO;
using System.Diagnostics;
using SimpleJSON;
using Core;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UTanksServer.Database
{
    public class UTServer
    {
        public static UTServer Instance { get; private set; }
        public static JSONNode Config => Program.Config["JTServer"];

        Task serverTask;
        Thread serverThread;
        public bool IsRunning { get; private set; }
        public UTServer()
        {
            Instance?.Destroy(false);
            Instance = this;

            if (Config["EnableModule"]) Start();
            else Logger.Log("JTServer not started. Module is disabled", "JTServer");
        }
        ~UTServer() {
            if (IsRunning) Destroy(false);
        }

        void Start()
        {
            #region NULLABLE
            //в общем ситуация следующая. Чтобы силой рестартить поток сервера, нужно выносить его в отдельный процесс, ибо тред.аборт() вырезали. В итоге мысль следующая - нам нужно вынести в отдельный процесс весь сервер, как это было сделано в ТХ. Но для дебага мы оставим сервер запускаться напрямую.

            //string path = Path.GetFullPath(Config["Path"]);
            //ProcessStartInfo startInfo = new ProcessStartInfo()
            //{
            //    // Get stuff from config (btw, casts are nessesary... I think)
            //    WorkingDirectory = path,
            //    FileName = Path.Combine(path, (string)Config["Excecutable"]),
            //    Arguments = (string)Config["Arguments"],
            //    CreateNoWindow = true, // Do not create a new window for this process
            //    ErrorDialog = false,
            //    RedirectStandardOutput = true,
            //    RedirectStandardError = true,
            //    RedirectStandardInput = false,
            //    UseShellExecute = false
            //};

            //try
            //{
            //    Process = Process.Start(startInfo);
            //    Logger.Log("Process Started", "JTServer");
            //    Process.ErrorDataReceived += (sendingProcess, errorLine) => Console.WriteLine(errorLine.Data); // Temporary, needs to be unified
            //    Process.OutputDataReceived += (sendingProcess, dataLine) =>
            //    {
            //        Console.WriteLine(dataLine.Data);
            //    };
            //    Process.BeginErrorReadLine();
            //    Process.BeginOutputReadLine();
            //    IsRunning = true;
            //    await Process.WaitForExitAsync();
            //    IsRunning = false;
            //    if (!Process.HasExited)
            //        Process.Kill();
            //    Logger.Log($"Process exited with code {Process.ExitCode}", "JTServer");
            //} catch (Exception err)
            //{ Logger.LogError(err.ToString(), "JTServer"); }
            //Func<Task> asyncUpd = async () => {


            //};
            #endregion


            serverTask = new Task(() =>
            {
                ServerInstanceStart.MainServerInstanceStart(JsonConvert.DeserializeObject<string[]>(Config["Arguments"].ToString()));

            });
            serverTask.ContinueWith(ServerExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
            serverTask.Start();
            IsRunning = true;
        }

        void ServerExceptionHandler(Task task)
        {
            Logger.LogError($"Server crash with {task.Exception.StackTrace}", "JTServer");
            if(Program.Config["ServerAutoRestart"] == true)
            {
                Destroy(true);
                #region NULLABLE
                //task = new Task(() =>
                //{
                //    ServerInstanceStart.MainServerInstanceStart(new string[] { (string)Config["Arguments"] });
                //});
                //task.ContinueWith(ServerExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
                //task.Start();
                //IsRunning = true;
                #endregion
            }
        }

        public void Destroy(bool restart = true)
        {
            if (serverTask != null && !(serverTask.Status == TaskStatus.Running)) 
            {
                serverThread.Interrupt();
                //ThreadEx.Abort(serverThread);

            }

            if (restart) new UTServer();
        }
    }
}
