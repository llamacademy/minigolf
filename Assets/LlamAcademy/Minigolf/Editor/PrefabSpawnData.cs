using UnityEngine;

namespace LlamAcademy.Minigolf.Editor
{
    [System.Serializable]
    public class PrefabSpawnData
    {
        public string PrefabResourcePath;
        public Vector3Int Position;
        public Quaternion Rotation;

        public PrefabSpawnData() {}

        public PrefabSpawnData(string prefabResourcePath, Vector3Int position, Quaternion rotation)
        {
            PrefabResourcePath = prefabResourcePath;
            Position = position;
            Rotation = rotation;
        }
    }
}
