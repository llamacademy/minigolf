using UnityEngine;

namespace LlamAcademy.Sensors
{
    [RequireComponent(typeof(SphereCollider))]
    public class PlayerSensor : MonoBehaviour
    {
        public delegate void PlayerEnterEvent(Transform Player);

        public delegate void PlayerExitEvent(Vector3 LastKnownPosition);

        public event PlayerEnterEvent OnPlayerEnter;
        public event PlayerExitEvent OnPlayerExit;

        [SerializeField] private Color GizmoColor;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                OnPlayerEnter?.Invoke(player.transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                OnPlayerExit?.Invoke(other.transform.position);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(GizmoColor.r, GizmoColor.g, GizmoColor.b, 0.25f);
            Gizmos.DrawSphere(transform.position, GetComponent<SphereCollider>().radius * 1.5f);
        }
    }
}