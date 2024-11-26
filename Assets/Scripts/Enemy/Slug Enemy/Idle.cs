using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Slug_Enemy
{
    public class Idle : IState
    {
        private Transform _transform;
        private NavMeshAgent _agent;
        private PlayerDetector _detector;
        
        private float _walkDistance;
        private Vector3 _lastPos;

        private float _timeStuck;
        private float _timeStuckMax = 0.5f;

        public Idle(Transform transform, NavMeshAgent agent, PlayerDetector detector, float walkDistance)
        {
            _transform = transform;
            _detector = detector;
            
            _lastPos = _transform.position;
            _agent = agent;
            _walkDistance = walkDistance;
        }
        
        public void Tick()
        {
            // Detect player in walking/running radius
            //if (_detector.PlayerInRange()) Debug.Log("I see");
            
            // Move to new position - nav mesh agent
            if (Vector3.Distance(_transform.position, _lastPos) <= 0f)
                _timeStuck += Time.deltaTime;

            if (_agent.remainingDistance <= 0.1f && _timeStuck >= _timeStuckMax)
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
            MoveToNewPosition();
        }

        public void OnExit()
        {
            // Set player position
        }
    }
}