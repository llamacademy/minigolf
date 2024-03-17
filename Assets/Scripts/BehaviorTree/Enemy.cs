using Unity.Muse.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace LlamAcademy.BehaviorTree
{
    [RequireComponent(typeof(NavMeshAgent), typeof(BehaviorGraphAgent))]
    public class Enemy : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Player Player;
        private BehaviorGraphAgent BehaviorAgent;
        private NavMeshAgent Agent;
        private Animator Animator;

        [Header("Attack Config")] 
        [SerializeField] [Range(0.1f, 5f)] private float AttackCooldown = 2;
        [SerializeField] [Range(1, 20f)] private float SpitCooldown = 17;

        [Space] [Header("Debug Info")] 
        [SerializeField] private bool IsInMeleeRange;
        [SerializeField] private bool IsInSpitRange;
        [SerializeField] private float LastAttackTime;
        private static readonly int SPEED = Animator.StringToHash("Speed");

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            BehaviorAgent = GetComponent<BehaviorGraphAgent>();
            Animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            BehaviorAgent.Blackboard.TrySetVariableValue("CanSpit", LastAttackTime + SpitCooldown <= Time.time);
            BehaviorAgent.Blackboard.TrySetVariableValue("CanMelee", LastAttackTime + SpitCooldown <= Time.time);
            Animator.SetFloat(SPEED, Agent.velocity.magnitude);
        }

        public void OnSpit()
        {
            Debug.Log("OnSpit");
            LastAttackTime = Time.time;
        }

        public void OnAttack()
        {
            Debug.Log("OnAttack");
            OnSpit();
        }
    }
}