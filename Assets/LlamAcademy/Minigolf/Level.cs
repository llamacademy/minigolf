using System.Collections.Generic;
using UnityEngine;

namespace LlamAcademy.Minigolf
{
    public class Level : ScriptableObject
    {
        public List<PrefabSpawnData> WorldObjectPositions;

        public Level() {}

        public Level(List<PrefabSpawnData> worldObjectPositions)
        {
            WorldObjectPositions = worldObjectPositions;
        }
    }
}
