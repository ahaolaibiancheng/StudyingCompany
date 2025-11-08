using System.IO;
using UnityEngine;

namespace SleepTracker
{
    public class SleepStorageFile : ISleepStorage
    {
        private static string FilePath(int year, int month)
        {
            string dir = Path.Combine(Application.persistentDataPath, "sleep");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return Path.Combine(dir, $"sleep_{year}_{month:00}.json");
        }

        public bool TryLoad(int year, int month, out SleepMonthData data)
        {
            string path = FilePath(year, month);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                data = JsonUtility.FromJson<SleepMonthData>(json);
                return data != null;
            }
            data = null;
            return false;
        }

        public void Save(SleepMonthData data)
        {
            string path = FilePath(data.year, data.month);
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(path, json);
        }
    }
}
