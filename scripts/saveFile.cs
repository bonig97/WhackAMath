using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace WhackAMath{
    public class SaveFile
    {
        public static int MaxLevelUnlocked { get; set; }
        public static string LanguageSelected { get; set; }
        public static List<int> Leaderboards { get; set; }
        public static int moleSelected { get; set; }
        public static int currentLevel { get; set; }

        public static void InitialSaveFile()
        {
            MaxLevelUnlocked = 1;
            LanguageSelected = "English";
            Leaderboards = new List<int>();
            moleSelected = 0;
            currentLevel = 1;
        }

        public static Dictionary<string, object> ConvertToDictionary()
        {
            Dictionary<string, object> saveData = new Dictionary<string, object>();
            saveData["MaxLevelUnlocked"] = MaxLevelUnlocked;
            saveData["LanguageSelected"] = LanguageSelected;
            saveData["moleSelected"] = moleSelected;
            saveData["Leaderboards"] = Leaderboards;
            return saveData;
        }

        public static void ConvertFromDictionary(Dictionary<string, object> saveData)
        {
            MaxLevelUnlocked = Convert.ToInt32(saveData["MaxLevelUnlocked"]);
            LanguageSelected = Convert.ToString(saveData["LanguageSelected"]);
            moleSelected = Convert.ToInt32(saveData["moleSelected"]);
            Leaderboards = (List<int>)saveData["Leaderboards"];

        }

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
            moleSelected = mole;

            await FirestoreHelper.UpdateDocument(ConvertToDictionary());
        }

        public static async void LoadSaveFile()
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

        }

    }
}
