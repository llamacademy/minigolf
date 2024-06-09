using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace LlamAcademy.Minigolf.Persistence
{
    public static class SavedDataService
    {
        private static string SAVE_FILE_NAME = "/player-data.json";

        public static void SaveData(PlayerLevelCompletionData data)
        {
            string path = Application.persistentDataPath + SAVE_FILE_NAME;

            try
            {
                if (File.Exists(path))
                {
                    Debug.Log("Data exists. Deleting old file and writing a new one!");
                    File.Delete(path);
                }
                else
                {
                    Debug.Log("Writing file for the first time!");
                }

                File.WriteAllText(path, JsonConvert.SerializeObject(data));
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
            }
        }

        public static PlayerLevelCompletionData LoadData()
        {
            string path = Application.persistentDataPath + SAVE_FILE_NAME;

            try
            {
                if (!File.Exists(path))
                {
                    Debug.LogWarning("Save file does not exist! Cannot load data");
                    return new PlayerLevelCompletionData();
                }

                string text = File.ReadAllText(path);
                PlayerLevelCompletionData data = JsonConvert.DeserializeObject<PlayerLevelCompletionData>(text);
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to load data due to: {e.Message} {e.StackTrace}");
            }

            return new PlayerLevelCompletionData();
        }
    }
}
