using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ClientCore.CoreImpl.Extensions
{
    public static class JsonExtend
    {
        public static JObject ReadFromFile(string filePath)
        {
            if( File.Exists(filePath))
            {
                string jsonText = File.ReadAllText(filePath);
                System.IO.MemoryStream mStream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonText));
                var reader = new JsonTextReader(new StreamReader(mStream));
                return JObject.Load(reader);
            }
            else
            {
                MessageBoxProvider.ShowWarning($"Filepath or file \"{0}\" does not exist", filePath);
                return null;
            }
        }
    }
}
