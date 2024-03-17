using UnityEngine;

namespace LlamAcademy.Sensors
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class ImpactSensor : MonoBehaviour
    {
        public delegate void CollisionEvent(Collision Collision);
        public event CollisionEvent OnCollision;

        public void OnCollisionEnter(Collision Collision)
        {
            OnCollision?.Invoke(Collision);
        }
    }
}
