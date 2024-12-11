using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class MoveToPlayer : IState
    {
        private Enemy _enemyRef;
        private Transform _target;
        private NavMeshAgent _agent;
        
        private Vector3 _lastPos;

        private float _timeStuck;
        private const float TimeStuckMax = 0.5f;

        public MoveToPlayer(Enemy enemyRef, NavMeshAgent agent)
        { 
            _enemyRef = enemyRef;
            _agent = agent;
            
            _lastPos = _enemyRef.transform.position;
        }
        
        public void Tick()
        {
            if (Vector3.Distance(_enemyRef.transform.position, _lastPos) <= 0f)
                _timeStuck += Time.deltaTime;

            _agent.SetDestination(_target.position);

            _lastPos = _enemyRef.transform.position;
        } 
        
        public void FixedTick()
        {
            throw new System.NotImplementedException();
        }

        public void OnEnter()
        {
            _timeStuck = 0;
            _target = _enemyRef.Target;
            
            _agent.SetDestination(_target.position);
        }

        public void OnExit()
        {
            return;
        }
    }
}