using System.Collections;
using LlamAcademy.Minigolf.Bus;
using LlamAcademy.Minigolf.Bus.Events;
using UnityEngine;

namespace LlamAcademy.Minigolf
{
    [RequireComponent(typeof(Rigidbody))]
    public class Ball : MonoBehaviour
    {
        [SerializeField] private float SettledVelocity = 0.01f;
        private Rigidbody Rigidbody;

        private Coroutine ListenForBallSettlingCoroutine;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            EventBus<PlayerStrokeEvent>.OnEvent += HandlePlayerStroke;
        }

        private void OnDisable()
        {
            EventBus<PlayerStrokeEvent>.OnEvent -= HandlePlayerStroke;
        }

        private void HandlePlayerStroke(PlayerStrokeEvent evt)
        {
            if (ListenForBallSettlingCoroutine != null)
            {
                StopCoroutine(ListenForBallSettlingCoroutine);
            }

            ListenForBallSettlingCoroutine = StartCoroutine(ListenForBallSettling());
        }

        private IEnumerator ListenForBallSettling()
        {
            WaitForFixedUpdate waitForFixedUpdate = new ();
            yield return null;
            yield return waitForFixedUpdate;
            while (Rigidbody.linearVelocity.magnitude >= SettledVelocity)
            {
                yield return waitForFixedUpdate;
            }

            EventBus<BallSettledEvent>.Raise(new BallSettledEvent(transform.position));
        }
    }
}
