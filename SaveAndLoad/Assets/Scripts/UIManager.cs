using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager _instance;

    public Text shootNumText;
    public Text scoreText;

    public int shootNum = 0;
    public int score = 0;

    public Toggle musicToggle;
    public AudioSource musicAudio;

    private bool musicOn = true;

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        shootNumText.text = shootNum.ToString();
        scoreText.text = score.ToString();
        MusicSwitch();
    }

    private void MusicSwitch()
    {
        if(musicToggle.isOn == false)
        {
            musicOn = false;
            musicAudio.enabled = false;
        }
        else
        {
            musicOn = true;
            musicAudio.enabled = true;
        }
    }

    public void AddShootNum()
    {
        shootNum++;
    }

    public void AddScore()
    {
        score++;
    }
}
