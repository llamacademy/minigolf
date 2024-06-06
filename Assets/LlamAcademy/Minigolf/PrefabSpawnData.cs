using UnityEngine;

namespace LlamAcademy.Minigolf
{
    [System.Serializable]
    public class PrefabSpawnData
    {
        public string PrefabResourcePath;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public PrefabSpawnData() {}

        public PrefabSpawnData(string prefabResourcePath, Vector3 position, Quaternion rotation )
        {
            PrefabResourcePath = prefabResourcePath;
            Position = position;
            Rotation = rotation;
            Scale = Vector3.one;
        }

        public PrefabSpawnData(string prefabResourcePath, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            PrefabResourcePath = prefabResourcePath;
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }
    }
}
