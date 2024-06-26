using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace WhackAMath {
	public class SaveFile
	{
		public static int MaxLevelUnlocked { get; set; }
		public static string LanguageSelected { get; set; }
		public static List<int> Leaderboards { get; set; }
		public static int MoleSelected { get; set; }
		public static int CurrentLevel { get; set; }
		public static PackedScene PrevScene { get; set; }
		public static void InitialSaveFile()
		{
			MaxLevelUnlocked = 1;
			LanguageSelected = "English";
			Leaderboards = new List<int>
			{
				0
			};
			MoleSelected = 0;
			CurrentLevel = 1;
		}

		public static Dictionary<string, object> ConvertToDictionary()
		{
			Dictionary<string, object> saveData = new()
            {
                ["MaxLevelUnlocked"] = MaxLevelUnlocked,
                ["LanguageSelected"] = LanguageSelected,
                ["moleSelected"] = MoleSelected,
                ["Leaderboards"] = Leaderboards
            };
			return saveData;
		}

		public static void ConvertFromDictionary(Dictionary<string, object> saveData)
		{
			MaxLevelUnlocked = Convert.ToInt32(saveData["MaxLevelUnlocked"]);
			LanguageSelected = Convert.ToString(saveData["LanguageSelected"]);
			MoleSelected = Convert.ToInt32(saveData["moleSelected"]);

			if (saveData["Leaderboards"] is List<object> tempLeaderboards)
			{
				Leaderboards = tempLeaderboards.ConvertAll(x => Convert.ToInt32(x));
			}
			else
			{
				throw new InvalidCastException("Leaderboards data is not in the expected format.");
			}
		}

		/// <summary>
		/// Updates the leaderboard with a new score.
		/// </summary>
		/// <param name="level">The level at which the score was obtained.</param>
		/// <param name="score">The score to add to the leaderboard.</param>
		public static async void UpdateLeaderboard(int level, int score)
		{
			if (Leaderboards.Count < 10)
			{
				Leaderboards.Add(score);
			}
			else
			{
				for (int i = 0; i < Leaderboards.Count; i++)
				{
					if (score > Leaderboards[i])
					{
						Leaderboards.Insert(i, score);
						Leaderboards.RemoveAt(Leaderboards.Count - 1);
						break;
					}
				}
			}

			await FirestoreHelper.UpdateDocument(ConvertToDictionary());
		}

		public static async void UpdateMaxLevelUnlocked(int level)
		{
			if (level > MaxLevelUnlocked)
			{
				MaxLevelUnlocked = level;
			}

			await FirestoreHelper.UpdateDocument(ConvertToDictionary());
		}

		public static async void UpdateLanguageSelected(string language)
		{
			LanguageSelected = language;

			await FirestoreHelper.UpdateDocument(ConvertToDictionary());
		}

		public static async void UpdateMoleSelected(int mole)
		{
			MoleSelected = mole;

			await FirestoreHelper.UpdateDocument(ConvertToDictionary());
		}

		public static async Task LoadSaveFile()
		{
			Dictionary<string, object> saveData = await FirestoreHelper.GetDocument();
			if (saveData == null)
			{
				InitialSaveFile();
				await FirestoreHelper.CreateDocument(ConvertToDictionary());
			}
			else
			{
				ConvertFromDictionary(saveData);
			}

			switch (SaveFile.LanguageSelected)
			{
				case "English":
					TranslationServer.SetLocale("en");
					break;
				case "Italian":
					TranslationServer.SetLocale("it");
					break;
				case "Arabic":
					TranslationServer.SetLocale("ar");
					break;
				default:
					TranslationServer.SetLocale("en");
					break;
			}
		}
	}
}
