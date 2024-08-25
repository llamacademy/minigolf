using System.Collections;
using System.Linq;
using LlamAcademy.Minigolf.Bus;
using LlamAcademy.Minigolf.Bus.Events;
using Unity.Collections;
using UnityEngine;

namespace LlamAcademy.Minigolf
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class Ball : MonoBehaviour
    {
        [SerializeField] private float SettledVelocity = 0.01f;
        [SerializeField] private PreventionMode BouncePreventionMode = PreventionMode.Simple;

        private Rigidbody Rigidbody;
        private Collider Collider;

        private Coroutine ListenForBallSettlingCoroutine;
        private int ColliderInstanceId;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Collider = GetComponent<Collider>();

            Collider.hasModifiableContacts = true;
            Collider.providesContacts = true;
            ColliderInstanceId = Collider.GetInstanceID();

            Physics.ContactModifyEvent += PreventGhostBumpsCCD;
        }


        private void OnEnable()
        {
            EventBus<PlayerStrokeEvent>.OnEvent += HandlePlayerStroke;
        }

        private void OnDisable()
        {
            EventBus<PlayerStrokeEvent>.OnEvent -= HandlePlayerStroke;
        }

        private void OnDestroy()
        {
            Physics.ContactModifyEvent -= PreventGhostBumpsCCD;
        }

        private void PreventGhostBumpsCCD(PhysicsScene scene, NativeArray<ModifiableContactPair> contactPairs)
        {
            ModifiableContactPair[] ballContactPairs =
                contactPairs.Where(pair => pair.colliderInstanceID == ColliderInstanceId).ToArray();
            // Debug.Log(
            //     $"CCD Collision found: {contactPairs.Length} contact pairs. {ballContactPairs.Length} affecting the ball.");

            switch (BouncePreventionMode)
            {
                case PreventionMode.Simple:
                    SimpleBouncePrevention(ballContactPairs);
                    return;
                case PreventionMode.None:
                default:
                    return;
            }
        }

        private void SimpleBouncePrevention(ModifiableContactPair[] ballContactPairs)
        {
            // Debug.Log("Modifying contact pairs with simple bounce prevention algorithm.");
            foreach (ModifiableContactPair pair in ballContactPairs)
            {
                for (int i = 0; i < pair.contactCount; i++)
                {
                    if (pair.GetSeparation(i) > 0)
                    {
                        // Debug.Log($"<color=#00ff00>Ignoring contact because separation of contact index {i} > 0</color>");
                        pair.SetNormal(i, Vector3.up);
                    }
                }
            }
        }

        private enum PreventionMode
        {
            None,
            Simple
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
            WaitForFixedUpdate waitForFixedUpdate = new();
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
