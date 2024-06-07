using System.Collections.Generic;
using UnityEngine;

namespace LlamAcademy.Minigolf
{
    public class LevelSO : ScriptableObject
    {
        public List<PrefabSpawnData> WorldObjectPositions;

        public Par[] Par = new Par[]
        {
            new (Rating.Perfect, 1),
            new (Rating.Good, 2),
            new (Rating.Ok, 3),
            new (Rating.Fail, 5)
        };

        public LevelSO() {}

        public LevelSO(List<PrefabSpawnData> worldObjectPositions)
        {
            WorldObjectPositions = worldObjectPositions;
        }
    }
}
