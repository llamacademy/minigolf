using System.Collections.Generic;

namespace LlamAcademy.Minigolf
{
    [System.Serializable]
    public class PlayerLevelCompletionData
    {
        /// <summary>
        /// Level name to best score map
        /// </summary>
        public Dictionary<string, int> LevelSaveData = new();
    }
}
