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
       SpawnNpc();
    }

    private void SpawnNpc()
    {
        _npc = Instantiate(npcObj, transform.position, Quaternion.identity).GetComponent<NPCControlScript>();
    }

    private IEnumerator CooldownTimer()
    {
        yield return new WaitForSeconds(_cooldownTimer);
        SpawnNpc();
    }

// Update is called once per frame
    void Update()
    {
        if (!_npc.IsAlive())
        {
            _npc.DestroyObj();
            StartCoroutine(CooldownTimer());
        }
    }
}
