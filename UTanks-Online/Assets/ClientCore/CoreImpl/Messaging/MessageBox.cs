using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient.Core.Logging;

namespace HiddenMessaging
{
    public class MessageBox : MonoBehaviour
    {
        public string Text;
        public string Title;
        public Color Color;
        public Color FontColor;
        public bool OnTop;
        public float ScalePerScreenSize;
        public bool Centered;
        public Action Callback;
        public Texture2D background;
        public GUIStyle guiStyle;
        public static MessageBox Init(string text, string title, Color color, bool onTop, float scalePerScreenSize, bool centered, Color fontcolor, Action callback)
        {
            Action<object> InvockedLambda = (object Obj) =>
            {
                GameObject gameObject = new GameObject();
                gameObject.AddComponent<MessageBox>();
                var message = gameObject.GetComponent<MessageBox>();
                message.Text = text;
                message.Title = title;
                message.Color = color;
                message.OnTop = onTop;
                message.ScalePerScreenSize = scalePerScreenSize;
                message.Centered = centered;
                message.Callback = callback;
                message.background = MakeTex((int)(Screen.width / scalePerScreenSize), (int)(Screen.height / scalePerScreenSize), color);
                message.FontColor = fontcolor;
            };

            ClientInitService.instance.ExecuteInstruction(InvockedLambda, null);

            return null;
        }

        void OnGUI()
        {
            guiStyle = new GUIStyle(GUI.skin.box);
            guiStyle.normal.background = background;
            guiStyle.normal.textColor = FontColor;
            guiStyle.alignment = TextAnchor.MiddleLeft;
            guiStyle.fontSize = 18;
            guiStyle.wordWrap = true;
            GUI.backgroundColor = Color;
            GUI.Box(new Rect((Centered ? Screen.width / 2 - Screen.width / ScalePerScreenSize / 2 : 0), (Centered ? Screen.height / 2 - Screen.height / ScalePerScreenSize / 2 : 0), Screen.width / ScalePerScreenSize, Screen.height / ScalePerScreenSize), new GUIContent { text = Text }, guiStyle);
            if (GUI.Button(new Rect(Screen.width / 2 - Screen.width / ScalePerScreenSize / 2, Screen.height / 2 + Screen.height / ScalePerScreenSize / 2, Screen.width / ScalePerScreenSize, 25), "Close"))
            {
                Callback();
                Destroy(this.gameObject);
            }

        }

        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}

