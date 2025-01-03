using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] protected float walkDistance = 10f;
        [SerializeField] protected float attackRadius = 10f;
        [SerializeField] protected float visionRadius = 15f;
        [SerializeField] protected float speed = 5f;
        
        [SerializeField] protected NavMeshAgent agent;
        [SerializeField] public PlayerDetector detectRange;
        public GameObject Target { get; protected set; }
        [SerializeField] protected float health = 20f;
        protected float CurrentHealth;
        [SerializeField] protected TextMeshPro healthDisplay;

        [SerializeField] protected bool canHear;
        //private bool _isAlive = true;
        

        protected StateMachine StateMachine;
        protected IState Idle;
        protected IState MoveToPlayer;
        protected IState Attack;
        protected IState BeingAttacked;
        protected IState Dead;

        public virtual void Awake()
        {
            // Enemies will always have 2 colliders - TODO: create sphere colliders if they don't exist
            SphereCollider[] colliders = GetComponentsInChildren<SphereCollider>();
            colliders.First(sphereCollider => sphereCollider.CompareTag("AttackRadius")).radius = attackRadius;
            colliders.First(sphereCollider => sphereCollider.CompareTag("VisionRadius")).radius = visionRadius;
            
            agent = GetComponent<NavMeshAgent>();
            agent.speed = speed;
            agent.stoppingDistance = attackRadius * 0.75f;
            
            StateMachine = new StateMachine();
            
            if (!detectRange) detectRange = GetComponent<PlayerDetector>();
            detectRange.CanHear = canHear;

            CurrentHealth = health;
            healthDisplay.SetText(CurrentHealth + " / " + health); 
        
            // STATES
            Idle = new Idle(this, agent, detectRange, walkDistance);
            MoveToPlayer = new MoveToPlayer(this, agent, 10f); //TODO: Change hardcoded value
            Attack = new Attack();
            BeingAttacked = new BeingAttacked();
            Dead = new Dead();
            
            // TRANSITIONS
            AT(Idle, MoveToPlayer, () => detectRange.CanSeePlayer || detectRange.CanHearPlayer);
            //AT(MoveToPlayer, Idle, () => !detectRange.CanSeePlayer() && !detectRange.CanHearPlayer());
            
            AAT(Idle, () => !detectRange.CanSeePlayer && !detectRange.CanHearPlayer);
        }
        
        public float GetAttackRadius() => attackRadius;
        public float GetSeeingRadius() => visionRadius;

        protected void AT(IState from, IState to, Func<bool> condition) =>
            StateMachine.AddTransition(from, to, condition);
        protected void AAT(IState to, Func<bool> condition) =>
            StateMachine.AddAnyTransition(to, condition);

        /*private void Update() => StateMachine.Tick();
        private void FixedUpdate() => stateMachine.FixedTick();*/
        
        public void TakeDamage(int dmg)
        {
            CurrentHealth -= dmg;
            if (CurrentHealth <= 0) gameObject.SetActive(false);
        }
    }
}
