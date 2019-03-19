using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public static TargetManager _instance;

    public GameObject[] monsters;
    public GameObject activeMonster = null;

    private float timer = 0;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        foreach(GameObject monster in monsters)
        {
            monster.GetComponent<BoxCollider>().enabled = false;
            monster.SetActive(false);
        }
        //ActivateMonster();
        StartCoroutine("AliveTimer");
        //StartCoroutine("DeathTimer");
    }
    private void Update()
    {
        timer += Time.deltaTime;
        //Debug.Log(timer);
    }

    private void ActivateMonster()
    {
        int index = Random.Range(0, monsters.Length);
        activeMonster = monsters[index];
        activeMonster.SetActive(true);
        activeMonster.GetComponent<BoxCollider>().enabled = true;
        StartCoroutine("DeathTimer");
    }

    IEnumerator AliveTimer()
    {
        yield return new WaitForSeconds(Random.Range(1, 5));
        ActivateMonster();
    }

    private void DeActiveMonster()
    {
        if(activeMonster != null)
        {
            activeMonster.GetComponent<BoxCollider>().enabled = false;
            activeMonster.SetActive(false);
            activeMonster = null;
        }
        StartCoroutine("AliveTimer");
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(Random.Range(3, 8));
        DeActiveMonster();
    }

    public void UpdateMonster()
    {
        StopAllCoroutines();
        if (activeMonster != null)
        {
            activeMonster.SetActive(false);
            activeMonster = null;
        }
        StartCoroutine("AliveTimer");
    }

}
