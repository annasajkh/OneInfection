// Ignore Spelling: Utils

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace OneInfection.Src.Utils
{
    public struct DialogItem
    {
        public string face;
        public string dialog;
        public float delayToNext;
    }

    public static class DialogParser
    {
        public static List<DialogItem> Parse(string pathToConversation)
        {
            return JsonConvert.DeserializeObject<List<DialogItem>>(File.ReadAllText(pathToConversation));
        }
    }
}
