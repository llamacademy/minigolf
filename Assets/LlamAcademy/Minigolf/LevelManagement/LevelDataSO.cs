using UnityEngine;

namespace LlamAcademy.Minigolf.LevelManagement
{
    [CreateAssetMenu(fileName = "Level Data", menuName = "Level Data")]
    public class LevelDataSO : ScriptableObject
    {
        public LevelSO Level;

        public LevelSO[] AllLevels;
    }
}
