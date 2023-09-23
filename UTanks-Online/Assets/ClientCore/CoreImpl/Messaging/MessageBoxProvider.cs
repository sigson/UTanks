using HiddenMessaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class MessageBoxProvider
{
    public static void ShowWarning(string text, string title)
    {
        MessageBox.Init(text, title, new UnityEngine.Color(0.8f, 0f, 0f), true, 3, true, new UnityEngine.Color(1f, 1f, 1f), () => { });
    }

    public static void ShowWarning(string text, string title, Action callback)
    {
        MessageBox.Init(text, title, new UnityEngine.Color(0.8f, 0f, 0f), true, 3, true, new UnityEngine.Color(1f, 1f, 1f), callback);
    }

    public static void ShowInfo(string text, string title)
    {
        MessageBox.Init(text, title, new UnityEngine.Color(0.65f, 0.9f, 0.91f), true, 3, true, new UnityEngine.Color(0f, 0f, 0f), () => { });
    }

    public static void ShowInfo(string text, string title, Action callback)
    {
        MessageBox.Init(text, title, new UnityEngine.Color(0.65f, 0.9f, 0.91f), true, 3, true, new UnityEngine.Color(0f, 0f, 0f), callback);
    }
}

