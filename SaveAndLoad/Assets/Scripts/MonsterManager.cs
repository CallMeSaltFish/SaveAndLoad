using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    private Animation anim;

    public AnimationClip idleClip;
    public AnimationClip dieClip;
    public int monsterType;

    private AudioSource kickAudio;

    void Awake()
    {
        anim = GetComponent<Animation>();
        anim.clip = idleClip;
        kickAudio = GetComponentInParent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Bullet")
        {
            Destroy(collision.gameObject);
            anim.clip = dieClip;
            anim.Play();
            GetComponent<BoxCollider>().enabled = false;
            StartCoroutine("Deactivate");
            kickAudio.Play();
            UIManager._instance.AddScore();
        }
    }

    private void OnDisable()
    {
        anim.clip = idleClip;
    }

    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(0.9f);
        GetComponentInParent<TargetManager>().UpdateMonster();
    }
}
