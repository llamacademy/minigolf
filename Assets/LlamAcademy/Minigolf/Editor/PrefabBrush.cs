using UnityEditor.Tilemaps;
using UnityEngine;

namespace LlamAcademy.Minigolf.Editor
{
    [CreateAssetMenu(fileName = "Prefab Brush", menuName = "Brushes/Prefab Brush")]
    [CustomGridBrush(false, true, false, "Prefab Brush")]
    public class PrefabBrush : GameObjectBrush {}
}
