using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine("DestroyMyself");
    }

    IEnumerator DestroyMyself()
    {
        yield return new WaitForSeconds(2);
        Destroy(this.gameObject);
    }
}
