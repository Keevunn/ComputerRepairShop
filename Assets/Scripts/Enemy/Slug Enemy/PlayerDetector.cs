using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    private GameObject _player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _player = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            StartCoroutine(ClearPlayerObj());
    }

    private IEnumerator ClearPlayerObj()
    {
        yield return new WaitForSeconds(1.5f);
        _player = null;
    }

    public bool PlayerInRange() => _player != null;

    public Vector3 GetPlayerPosition()
    {
        return _player?.transform.position ?? Vector3.zero;
    }
}
