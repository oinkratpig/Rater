using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Rater
{
	internal static class SaveData
	{
		private static readonly string _savepath = System.Environment.ExpandEnvironmentVariables("%appdata%\\oinkratpig\\Rater\\save.txt");

		private class Data
		{
			public string Name { get; private set; }
			public int Value { get; private set; }
			public int Max { get; private set; }
			public Data(string name, int value, int max)
			{
				Name = name;
				Value = value;
				Max = max;
			}
		}

		public static void Save(List<RateableUI> rateables)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(_savepath));
			using (StreamWriter writer = new StreamWriter(_savepath))
				foreach (RateableUI rateable in rateables)
				{
					Data data = new Data(rateable.RateableName, rateable.Value, rateable.MaxValue);
					writer.WriteLine(JsonSerializer.Serialize(data));
				}

		} // end Save

		public static void Load(VBoxContainer rateableContainer, PackedScene rateableUIPrefab)
		{
			List<RateableUI> rateables = new List<RateableUI>();

			using (StreamReader reader = new StreamReader(_savepath))
				while(!reader.EndOfStream)
				{
					string line = reader.ReadLine();
					Data data = JsonSerializer.Deserialize<Data>(line);

					Controller.CreateRateableUI(rateableContainer, rateableUIPrefab, data.Name, data.Value, data.Max);
				}

		} // end Load

	} // end class SaveData

} // end namespace