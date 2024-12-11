using System;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] protected float walkDistance = 10f;
        [SerializeField] protected float radius = 3f;
        [SerializeField] protected float speed = 5f;
        
        [SerializeField] protected NavMeshAgent agent;
        [SerializeField] protected PlayerDetector detectRange;
        public Transform Target { get; set; }
        [SerializeField] protected float health = 20f;

        protected StateMachine StateMachine;
        protected IState Idle;
        protected IState MoveToPlayer;
        protected IState Attack;
        protected IState BeingAttacked;
        protected IState Dead;

        public virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = speed;
            
            StateMachine = new StateMachine();
            
            if (!detectRange) detectRange = GetComponent<PlayerDetector>();
        
            // STATES
            Idle = new Idle(this, agent, detectRange, walkDistance);
            MoveToPlayer = new MoveToPlayer(this, agent);
            Attack = new Attack();
            BeingAttacked = new BeingAttacked();
            Dead = new Dead();
            
            // TRANSITIONS
            AT(Idle, MoveToPlayer, detectRange.PlayerInRange);
            AT(MoveToPlayer, Idle, () => !detectRange.PlayerInRange());
            
        }
        
        protected void AT(IState from, IState to, Func<bool> condition) =>
            StateMachine.AddTransition(from, to, condition);
        protected void AAT(IState to, Func<bool> condition) =>
            StateMachine.AddAnyTransition(to, condition);

        /*private void Update() => StateMachine.Tick();
        private void FixedUpdate() => stateMachine.FixedTick();*/
    }
}
