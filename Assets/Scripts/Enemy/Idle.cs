using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class Idle : IState
    {
        private Enemy _enemyRef;
        
        private Transform _transform;
        private NavMeshAgent _agent;

        private PlayerDetector _detector;
        
        private float _walkDistance;
        private Vector3 _lastPos;

        private float _timeStuck;
        private const float TimeStuckMax = 0.5f;

        private float _initStoppingDistance;

        public Idle(Enemy enemyRef, NavMeshAgent agent, PlayerDetector detector, float walkDistance)
        {
            _enemyRef = enemyRef;
            _transform = enemyRef.transform;

            _detector = detector;
            
            _lastPos = _transform.position;
            _agent = agent;
            _walkDistance = walkDistance;
            
        }
        
        public void Tick()
        {
            // Move to new position - nav mesh agent
            if (Vector3.Distance(_transform.position, _lastPos) <= 0f)
                _timeStuck += Time.deltaTime;

            if (_agent.remainingDistance <= 0.1f && _timeStuck >= TimeStuckMax)
            {
                _timeStuck = 0f;
                MoveToNewPosition();
            }

            _lastPos = _transform.position;

        }

        private void MoveToNewPosition()
        {
            Vector3 newPos = Random.insideUnitSphere * _walkDistance;
            newPos += _transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(newPos, out hit, _walkDistance, NavMesh.AllAreas))
                _agent.SetDestination(hit.position);
        }
        
        public void FixedTick()
        {
            throw new System.NotImplementedException();
        }

        public void OnEnter()
        {
            Debug.Log("Idle State");
            _initStoppingDistance = _agent.stoppingDistance;
            _agent.stoppingDistance = 0;
            
            MoveToNewPosition();
        }

        public void OnExit()
        {
            // Set player position as target
            //_enemyRef.Target = _detector.GetPlayerRef();
            _agent.stoppingDistance = _initStoppingDistance;
        }
    }
}