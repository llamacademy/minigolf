using UnityEngine;

namespace LlamAcademy.Minigolf
{
    [CreateAssetMenu(fileName = "Level Data", menuName = "Level Data")]
    public class LevelDataSO : ScriptableObject
    {
        public Level Level;
    }
}
