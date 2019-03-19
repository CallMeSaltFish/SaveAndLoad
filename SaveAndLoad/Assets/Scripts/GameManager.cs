using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public bool isPaused = true;
    public GameObject menuGO;
    public GameObject[] targetsGOs;

    private void Awake()
    {
        _instance = this;
        Pause();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    private void Pause()
    {
        isPaused = true;
        menuGO.SetActive(true);
        Time.timeScale = 0;
        Cursor.visible = true;
    }

    private void UnPause()
    {
        isPaused = false;
        menuGO.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;
    }

    public void ContinueGame()
    {
        UnPause();
    }

    public void NewGame()
    {
        foreach(GameObject go in targetsGOs)
        {
            go.GetComponent<TargetManager>().UpdateMonster();
        }
        UIManager._instance.score = 0;
        UIManager._instance.shootNum = 0;
        UnPause();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
