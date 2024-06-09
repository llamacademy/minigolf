using LlamAcademy.Minigolf.Bus;
using LlamAcademy.Minigolf.Bus.Events;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LlamAcademy.Minigolf.LevelManagement
{
    [RequireComponent(typeof(BoxCollider))]
    public class LevelBounds : MonoBehaviour
    {
        public Rigidbody Ball;
        private BoxCollider Collider;

        public void Resize(Transform parent)
        {
            if (Collider == null)
            {
                Collider = GetComponent<BoxCollider>();
                Collider.isTrigger = true;
            }

            Bounds bounds = new(Vector3.zero, Vector3.one);

            foreach (Transform child in parent)
            {
                if (child.TryGetComponent(out Renderer renderer) && renderer is not TilemapRenderer)
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }


            bounds.min -= new Vector3(2, 2, 2);
            bounds.max += new Vector3(2, 2, 2);

            Collider.center = bounds.center;
            Collider.size = bounds.size;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Ball"))
            {
                EventBus<BallExitedLevelBoundsEvent>.Raise(new BallExitedLevelBoundsEvent());
            }
        }
    }
}
