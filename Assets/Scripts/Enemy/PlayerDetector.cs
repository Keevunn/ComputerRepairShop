using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class PlayerDetector : MonoBehaviour
    {
        private GameObject _player;
        // Will only have max 2 colliders for player detection
        private float _radius = 100f;
        private bool _canSeePlayer = false, _playerInSight = false, _canHearPlayer = false;
        [SerializeField] private LayerMask obstacleMask;
        private float _hearRadius;
        [SerializeField] private bool canHear;
        [SerializeField] private float fieldOfView = 180f;

        private void Awake()
        {
            SphereCollider[] colliders = GetComponents<SphereCollider>();
            if (colliders.Length == 1) _radius = colliders[0].radius;
            else
            {
                _radius = Math.Max(colliders[0].radius, colliders[1].radius) + 0.4f;
                _hearRadius = Math.Min(colliders[0].radius, colliders[1].radius) + 0.4f;
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _player = other.gameObject;
                _playerInSight = true;
                if (canHear)
                {
                    float distance = Vector3.Distance(transform.position, other.bounds.center);
                    // In fov radius - can see player 
                    if (distance > _hearRadius && distance <= _radius) _playerInSight = true;
                    // In hearing radius - can hear player
                    if (distance <= _hearRadius) _canHearPlayer = true;
                }
            }
                
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                float distance = Vector3.Distance(transform.position, other.bounds.center);
                StartCoroutine(ClearPlayerObj(distance));
            }
        }

        private IEnumerator ClearPlayerObj(float distance)
        {
            yield return new WaitForSeconds(0.75f);
            
            if (canHear)
            {
                if (distance >= _hearRadius) _canHearPlayer = false;
                if (distance >= _radius)
                {
                    _player = null;
                    _playerInSight = false;
                    _canSeePlayer = false;
                }
            }
            else _player = null;
        }

        public float GetFoV()
        {
            return fieldOfView;
        }

        public bool PlayerInRange()
        {
            return _canHearPlayer || _canSeePlayer;
        } 

        public Vector3 GetPlayerPosition()
        {
            return _player?.transform.position ?? Vector3.zero;
        }

        public Transform GetPlayerTransform()
        {
            return _player?.transform;
        }
        
        // FOV checks
        public IEnumerator FoVRoutine()
        {
            yield return new WaitForSeconds(0.2f);
            if (_player) FoVCheck();
        }

        private void FoVCheck()
        {
            if (!_playerInSight)
            {
                _canSeePlayer = false;
                return;
            }

            Vector3 dir = (_player.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dir);

            if (angle > fieldOfView / 2)
            {
                _canSeePlayer = false;
                return;
            }
            
            float distance = Vector3.Distance(transform.position, _player.transform.position);

            if (!Physics.Raycast(transform.position, dir, distance, obstacleMask)) _canSeePlayer = true;
            else if (_canSeePlayer) _canSeePlayer = false;
            

        }
    }
}