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
        private float _stoppingRadius;
        
        private Vector3 _lastPos;

        private float _timeStuck;
        private const float TimeStuckMax = 0.5f;

        public MoveToPlayer(Enemy enemyRef, NavMeshAgent agent, float stoppingRadius)
        { 
            _enemyRef = enemyRef;
            _agent = agent;
            _stoppingRadius = stoppingRadius;
            
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
            Debug.Log("MoveToPlayer State");
            _timeStuck = 0;
            //_agent.stoppingDistance = _stoppingRadius;
            _target = _enemyRef.Target.transform;
            
            _agent.SetDestination(_target.position);
            //_agent.isStopped = false;
        }

        public void OnExit()
        {
            //_agent.stoppingDistance = 0;
            //_agent.isStopped = true;
        }
    }
}