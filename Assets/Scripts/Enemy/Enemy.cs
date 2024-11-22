using System;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private float detectRadius = 3f;
        [SerializeField] private float health = 20f;

        private StateMachine _stateMachine = new StateMachine();

        protected virtual void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        
            // STATES
            IState idle = new Idle();
            IState moveToPlayer = new MoveToPlayer();
            IState attack = new Attack();
            IState beingAttacked = new BeingAttacked();
            IState dead = new Dead();
            
            void AT(IState from, IState to, Func<bool> condition) =>
                _stateMachine.AddTransition(from, to, condition);
            void AAT(IState to, Func<bool> condition) =>
                _stateMachine.AddAnyTransition(to, condition);
            
            // TRANSITIONS
            AT(idle, moveToPlayer, /* When player in FOV */ );
            AT(moveToPlayer, attack, /* When player in range */ );
            AT(attack, moveToPlayer, /* Player not in range, in FOV */ );

            AAT(idle, /* Player no long in FOV after delay */);
            AAT(beingAttacked, /* Enemy can't see player, but taking damage */ );
            AAT(dead, () => health == 0);
        }


        private void Update() => _stateMachine.Tick();

        private void FixedUpdate() => _stateMachine.FixedTick();
    }
}
