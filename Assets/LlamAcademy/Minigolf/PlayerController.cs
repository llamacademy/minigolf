using Cinemachine;
using LlamAcademy.Minigolf.Bus;
using LlamAcademy.Minigolf.Bus.Events;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace LlamAcademy.Minigolf
{
    [RequireComponent(typeof(Camera))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Rigidbody Ball;
        [SerializeField] private Image PowerImage;
        [SerializeField] private float MaxForce;
        [SerializeField] private Gradient PowerGradient;
        [SerializeField] private CinemachineInputProvider RotationInputProvider;

        private Camera Camera;
        private int Putts = 0;
        private Vector2 InitialTouchPosition;
        private bool ShouldShowPower;

        private Vector3 LastBallPosition;
        private bool WaitingForBallToSettle;

        private void Awake()
        {
            Camera = GetComponent<Camera>();
            EnhancedTouchSupport.Enable();
        }

        private void OnEnable()
        {
            Touch.onFingerDown += TouchOnFingerDown;
            Touch.onFingerMove += TouchOnFingerMove;
            Touch.onFingerUp += TouchOnFingerUp;
        }

        private void Start()
        {
            LastBallPosition = Ball.transform.position;
            // TouchSimulation.Enable(); apparently doesn't work in Unity 6, but with monobehavior it does
            EventBus<BallExitedLevelBoundsEvent>.OnEvent += OnBallExitedLevelBounds;
            EventBus<BallSettledEvent>.OnEvent += OnBallSettled;
            EventBus<BallInHoleEvent>.OnEvent += OnBallInHoleEvent;
            EventBus<PauseEvent>.OnEvent += HandlePause;
            EventBus<ResumeEvent>.OnEvent += HandleResume;
        }

        private void HandleResume(ResumeEvent evt)
        {
            enabled = true;
        }

        private void HandlePause(PauseEvent evt)
        {
            enabled = false;
        }

        private void OnBallInHoleEvent(BallInHoleEvent evt)
        {
            enabled = false;
        }

        private void OnBallSettled(BallSettledEvent evt)
        {
            LastBallPosition = evt.Position + Vector3.up * 0.5f;
            WaitingForBallToSettle = false;
        }

        private void OnBallExitedLevelBounds(BallExitedLevelBoundsEvent evt)
        {
            Ball.angularVelocity = Vector3.zero;
            Ball.linearVelocity = Vector3.zero;
            Ball.transform.position = LastBallPosition;
        }

        private void OnDisable()
        {
            Touch.onFingerDown -= TouchOnFingerDown;
            Touch.onFingerMove -= TouchOnFingerMove;
            Touch.onFingerUp -= TouchOnFingerUp;
        }

        private void OnDestroy()
        {
            EventBus<BallExitedLevelBoundsEvent>.OnEvent -= OnBallExitedLevelBounds;
            EventBus<BallSettledEvent>.OnEvent -= OnBallSettled;
            EventBus<BallInHoleEvent>.OnEvent -= OnBallInHoleEvent;
            EventBus<PauseEvent>.OnEvent -= HandlePause;
            EventBus<ResumeEvent>.OnEvent -= HandleResume;
        }

        private void TouchOnFingerUp(Finger finger)
        {
            RotationInputProvider.enabled = false;
            if (!ShouldShowPower || PowerImage.rectTransform.sizeDelta.magnitude < 0.01f) return;

            PowerImage.gameObject.SetActive(false);

            EventBus<PlayerStrokeEvent>.Raise(new PlayerStrokeEvent(Ball.transform.position, Putts));
            WaitingForBallToSettle = true;

            Vector3 forceDirection = (Ball.transform.position - transform.position).normalized;
            forceDirection.y = 0;
            Ball.AddForce(MaxForce * GetForce(finger) * forceDirection);

            Putts++;
        }

        private void TouchOnFingerMove(Finger finger)
        {
            if (ShouldShowPower)
            {
                PowerImage.rectTransform.sizeDelta = new Vector2(PowerImage.rectTransform.sizeDelta.x, GetForce(finger));
                PowerImage.color = PowerGradient.Evaluate(PowerImage.rectTransform.sizeDelta.y / 100);
            }
        }

        private void TouchOnFingerDown(Finger finger)
        {
            Ray cameraRay = Camera.ScreenPointToRay(finger.screenPosition);
            InitialTouchPosition = finger.screenPosition;

            if (!Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Ball"), QueryTriggerInteraction.Collide)
                || hit.transform != Ball.transform)
            {
                RotationInputProvider.enabled = true;
                ShouldShowPower = false;
                return;
            }

            if (WaitingForBallToSettle)
            {
                ShouldShowPower = false;
                return;
            }
            ShouldShowPower = true;

            PowerImage.gameObject.SetActive(true);
            PowerImage.rectTransform.sizeDelta = new Vector2(PowerImage.rectTransform.sizeDelta.x, 0);
        }

        private float GetForce(Finger finger) => Mathf.Clamp(InitialTouchPosition.y - finger.screenPosition.y, 0, 100);

    }
}
