using UnityEngine;

namespace LlamAcademy.Minigolf
{
    [System.Serializable]
    public class PrefabSpawnData
    {
        public string PrefabResourcePath;
        public Vector3 Position;
        public Quaternion Rotation;

        public PrefabSpawnData() {}

        public PrefabSpawnData(string prefabResourcePath, Vector3 position, Quaternion rotation)
        {
            PrefabResourcePath = prefabResourcePath;
            Position = position;
            Rotation = rotation;
        }
    }
}
