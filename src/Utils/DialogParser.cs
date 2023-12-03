using Godot;
using Godot.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace OneInfection.src.Utils
{
    public struct DialogItem
    {
        public string face;
        public string dialog;
        public float delayToNext;
    }

    public static class DialogParser
    {
        public static Array<Array<Variant>> Parse(string pathToScenario)
        {
            List<DialogItem> dialogItems = JsonConvert.DeserializeObject<List<DialogItem>>(File.ReadAllText(pathToScenario));

            var array = new Array<Array<Variant>>();

            for (int i = 0; i < dialogItems.Count; i++)
            {
                var instance = dialogItems[i];


                array.Add(new Array<Variant>()
                {
                    instance.face,
                    instance.dialog,
                    instance.delayToNext
                });
            }

            return array;
        }
    }
}
