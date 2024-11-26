using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Slug_Enemy
{
    public class SlugEnemy : Enemy  
    {
        [SerializeField] private float walkDistance = 10f;
        [SerializeField] private float walkRadius = 3f;
        [SerializeField] private float runRadius = 5f;

        [SerializeField] private PlayerDetector detectRange;
        
        private StateMachine _stateMachine;
        
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, walkRadius);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, runRadius);
        }

        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new StateMachine();
            if (!detectRange) detectRange = GetComponent<PlayerDetector>();
            
            IState idle = new Idle(transform, agent, detectRange, walkDistance);
            
            _stateMachine.SetState(idle);
        }
        
        private void Update() => _stateMachine.Tick();
    }
}