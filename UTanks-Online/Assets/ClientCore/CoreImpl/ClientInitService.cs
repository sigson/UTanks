using Newtonsoft.Json.Linq;
using SecuredSpace.Battle;
using SecuredSpace.ClientControl.Model;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.Settings;
using SecuredSpace.UI;
using SecuredSpace.UI.GameUI;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UTanksClient;
using UTanksClient.Core.Logging;
using UTanksClient.ECS;
using UTanksClient.ECS.ECSCore;
using UTanksClient.Extensions;
using UTanksClient.Services;
using SecuredSpace.Settings.SettingsObject;

public static class GlobalProgramState
{
    //#if !UNITY_EDITOR
    //    public static string ConfigDir = Application.streamingAssetsPath + "\\Data\\";
    //#else

    //#endif

    private static string _confdir = "";
    public static string ConfigDir {
        get
        {
            if (_confdir == "")
            {
                ClientInitService.instance.ExecuteFunction(() => GlobalProgramState.ConfigDir = Application.persistentDataPath + "\\Data\\");
            }
            //while (_confdir == "") { }
            return _confdir;
        }
        set
        {
            _confdir = value;
        }
    }
    public static ProgramType programType = ProgramType.Client;
    public enum ProgramType { Server, Client }
}
public class ClientInitService : IService
{
    public static ClientInitService instance => SGT.Get<ClientInitService>();
    public JSONFile Config;
    public JSONFile UserSettingsConfig;
    public JSONFile UserSecurityConfig;
    
    public GameObject MainCamera;
    public GameObject NowMainCamera;
    public bool CheckMait = false;
    public static object globalSerializationLocker = new object();
    public bool LockInput = false;
    public AnchorHandler anchorHandler = new AnchorHandler();
    public GameSettings gameSettings = new GameSettings();
    public bool ECSLoaded = false;
    public event EventHandler? OnLoadedGame = (en, en2) => { };
    //public bool StopECS;

    public void Update()
    {
        if (CheckMait)
            CheckMait = false;
    }

    public override void InitializeProcess()
    {
        //bmark: stageinit 1
        LoadConfig();

        MainCamera = new GameObject() { name = "MainCamera", tag = "MainCamera" };
        MainCamera.AddComponent<Camera>();
        MainCamera.AddComponent<AudioListener>();
        NowMainCamera = MainCamera;

        return;
        try
        {

            //uiManager = UIManagerLocal.GetComponent<UIService>();
            //battleManager = BattleManagerLocal.GetComponent<BattleManager>();
            
            
            
            
            //ECSComponentManager.IdStaticCache();//move it from managerscope because that shit not working normally and time-to-time destroy method work way without any errors or messages, JUST STOP THE METHOD EXECUTION WITHOUT ENTER TO METHOD BODY.
            
        }
        catch { }
    }

    #region Common
    public void LoadConfig()
    {
        string configDirPath = Application.streamingAssetsPath + "\\";
        if (File.Exists(configDirPath + "config.json"))
            Config = new JSONFile(configDirPath + "config.json").Load();//JsonExtend.ReadFromFile(configDirPath + "config.json");
        else
        {
            MessageBoxProvider.ShowWarning("Main config file not finded. Application will stopped", "", () => { ClientInitService.instance.QuitGame(); });
            throw new Exception("Main config file not finded. Application will stopped");
        }

        if (File.Exists(configDirPath + "settingsconfig.json"))
            UserSettingsConfig = new JSONFile(configDirPath + "settingsconfig.json").Load();//JsonExtend.ReadFromFile(configDirPath + "settingsconfig.json");
        if (File.Exists(configDirPath + "settingsconfig.json"))
            gameSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<GameSettings>(File.ReadAllText(configDirPath + "settingsconfig.json"));
        else
        {
            File.Copy(configDirPath + "settingsconfigdefault.json", configDirPath + "settingsconfig.json");
            gameSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<GameSettings>(File.ReadAllText(configDirPath + "settingsconfig.json"));
        }
        gameSettings.SoundVolumeRange = gameSettings.SoundVolumeRange;
        if (File.Exists(configDirPath + "loginconfig.json"))
            UserSecurityConfig = new JSONFile(configDirPath + "loginconfig.json").Load();//JsonExtend.ReadFromFile(configDirPath + "loginconfig.json");

        if (File.Exists(Application.streamingAssetsPath + "\\stenography.txt"))
        {
            File.Delete(Application.streamingAssetsPath + "\\stenography.txt");
        }

        if (!(Config["Network"]["HostAddress"].Value.ToString().Contains("192.168") || Config["Network"]["HostAddress"].Value.ToString().Contains("127.0.0") || Config["Network"]["HostAddress"].Value.ToString().Contains("0.0.0.") || Config["Network"]["HostAddress"].Value.ToString().Contains("ngrok")))
        {
            var uri = Config["Network"]["HostAddress"].Value.ToString();
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                webRequest.SendWebRequest();

                while (!(webRequest.isDone || webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.DataProcessingError || webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.isNetworkError))
                {
                    Task.Delay(10).Wait();
                }

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        ULogger.Log(pages[page] + ": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        ULogger.Log(pages[page] + ": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        var webtex = webRequest.downloadHandler.text;
                        var pretext = "&lt;&lt;3r3#!&gt;";
                        var aftertext = "&lt;&gt;3r3#!&lt;";
                        var hostport = webtex.Substring(webtex.IndexOf(pretext) + pretext.Length, webtex.IndexOf(aftertext) - aftertext.Length).Split(':');
                        Config["Network"]["HostAddress"].Value = hostport[0];
                        Config["Network"]["HostPort"].Value = hostport[1];
                        break;
                }
            }
        }
    }

    public bool CheckEntityIsPlayer(ECSEntity entity)
    {
        if (!ClientNetworkService.instance.Connected)
            return false;
        if (entity.instanceId == ClientNetworkService.instance.PlayerEntityId)
            return true;
        else
            return false;
    }

    public bool CheckEntityIsPlayer(long entityId)
    {
        if (ClientNetworkService.instance == null)
            return false;
        if (entityId == ClientNetworkService.instance.PlayerEntityId)
            return true;
        else
            return false;
    }

    #endregion

    public void QuitGame()
    {
        // save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public override void PostInitializeProcess()
    {
        TaskEx.RunAsync(() => {
            ManagerScope.InitManagerScope();
            ECSLoaded = true;
        });
        TaskEx.RunAsync(() =>
        {
            int counter = 0;
            while (!(ConstantService.instance.Loaded && ClientNetworkService.instance.Connected && ECSLoaded) && counter < 800)
            {
                Task.Delay(100).Wait();
                counter++;
                if(!(counter < 800))
                {
                    ULogger.Error("Something went wrong on game loading");
                    return;
                }
            }
            UIService.instance.ExecuteInstruction((object Obj) =>
            {
                ClientInitService.instance.OnLoadedGame(null, null);
            }, null);
            if (UserSecurityConfig["LoginData"].Children.Count() != 0)
            {
                //ULogger.LogLoad("Attempt aoutologin");
                ClientNetworkService.instance.AttemptLogin(UserSecurityConfig["LoginData"]["login"], UserSecurityConfig["LoginData"]["password"], "", (loginFailedEvent) => {
                    UIService.instance.ExecuteInstruction((object Obj) =>
                    {
                        UIService.instance.Hide(new GameObject[] { UIService.instance.LoadingWindowUI });
                        UIService.instance.Show(new GameObject[] { UIService.instance.LoginRegisterUI });
                    }, null);
                });
            }
        });
        //UIService.instance.GameSettingsUI.GetComponent<SettingsUIHandler>().UpdateSettings();
        ClientNetworkService.instance.OnServerAvailableAction = () => {
            int counter = 0;
            while (!(ECSLoaded) && counter < 100)
            {
                Task.Delay(100).Wait();
                counter++;
                if (!(counter < 100))
                {
                    ULogger.Error("Something went wrong on game loading");
                    return;
                }
            }
            ConstantService.instance.PreInitialize();
        };
    }

    public override void OnDestroyReaction()
    {
        
    }
}
