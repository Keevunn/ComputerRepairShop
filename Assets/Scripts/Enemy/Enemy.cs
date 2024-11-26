using System;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] protected NavMeshAgent agent;
        //[SerializeField] private float detectRadius = 3f;
        //[SerializeField] private float health = 20f;

        protected StateMachine stateMachine;

        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            stateMachine = new StateMachine();
        
            // STATES
            IState idle = new Idle();
            IState moveToPlayer = new MoveToPlayer();
            IState attack = new Attack();
            IState beingAttacked = new BeingAttacked();
            IState dead = new Dead();
            
            void AT(IState from, IState to, Func<bool> condition) =>
                stateMachine.AddTransition(from, to, condition);
            void AAT(IState to, Func<bool> condition) =>
                stateMachine.AddAnyTransition(to, condition);
            
            // TRANSITIONS
            /*AT(idle, moveToPlayer, /* When player in FOV #1# );
            AT(moveToPlayer, attack, /* When player in range #1# );
            AT(attack, moveToPlayer, /* Player not in range, in FOV #1# );

            AAT(idle, /* Player no long in FOV after delay #1#);
            AAT(beingAttacked, /* Enemy can't see player, but taking damage #1# );
            AAT(dead, () => health == 0);*/
        }


        private void Update() => stateMachine.Tick();

        //private void FixedUpdate() => stateMachine.FixedTick();
    }
}
