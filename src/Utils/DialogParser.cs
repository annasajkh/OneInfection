using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OneInfection.src.Utils
{
	public struct DialogItem {
		public string emotion;
		public string dialog;
		public float delayToNext;
	}

	public static class DialogParser
	{
		public static Array<Array<Variant>> Parse(string pathToScenario)
		{
			var dialogItems = new List<DialogItem>();
			using (StreamReader r = new StreamReader(pathToScenario))
			{
				string json = r.ReadToEnd();
				dialogItems = JsonSerializer.Deserialize<List<DialogItem>>(json);
			}

			var array = new Array<Array<Variant>>();

			for(int i = 0; i < dialogItems.Count; i++)
			{
				var instance = dialogItems[i];
				array.Add(
					new Array<Variant>() {
						instance.emotion,
						instance.dialog,
						instance.delayToNext
					});
			}

			return array;
		}
	}
}
