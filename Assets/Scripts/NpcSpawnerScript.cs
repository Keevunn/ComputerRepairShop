using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcSpawnerScript : MonoBehaviour
{
    public GameObject npcObj;

    private NPCControlScript _npc;
    private float _cooldownTimer = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
       SpawnNpc(10);
    }

    private void SpawnNpc(int health)
    {
        _npc = Instantiate(npcObj, transform.position, Quaternion.identity).GetComponent<NPCControlScript>();
        _npc.SetHealth(health);
    }

    private IEnumerator CooldownTimer()
    {
        yield return new WaitForSeconds(_cooldownTimer);
        SpawnNpc(10);
    }

// Update is called once per frame
    void Update()
    {
        if (_npc && !_npc.IsAlive())
        {
            _npc.DestroyObj();
            StartCoroutine(CooldownTimer());
        }
    }
}
