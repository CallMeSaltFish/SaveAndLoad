using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager _instance;

    public Text shootNumText;
    public Text scoreText;
    public Text messageText;
    public int shootNum = 0;
    public int score = 0;

    public Toggle musicToggle;
    public AudioSource musicAudio;

    private void Awake()
    {
        _instance = this;
        if(PlayerPrefs.HasKey("MusicOn"))
        {
            if(PlayerPrefs.GetInt("MusicOn") == 1)
            {
                musicToggle.isOn = true;
                musicAudio.enabled = true;
            }
            else
            {
                musicToggle.isOn = false;
                musicAudio.enabled = false;
            }
        }
        else
        {
            musicToggle.isOn = true;
            musicAudio.enabled = true;
        }
    }

    private void Update()
    {
        shootNumText.text = shootNum.ToString();
        scoreText.text = score.ToString();
    }

    public void MusicSwitch()
    {
        if(musicToggle.isOn == false)
        {
            musicAudio.enabled = false;
            PlayerPrefs.SetInt("MusicOn", 0);
        }
        else
        {
            musicAudio.enabled = true;
            PlayerPrefs.SetInt("MusicOn", 1);
        }
        /**/
        PlayerPrefs.Save();
    }

    public void AddShootNum()
    {
        shootNum++;
    }

    public void AddScore()
    {
        score++;
    }

    public void showMessage(string str)
    {
        messageText.text = str;
    }
}
