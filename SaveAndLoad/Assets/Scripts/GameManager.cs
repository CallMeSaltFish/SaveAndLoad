using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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

    private Save CreateSaveGO()
    {
        Save save = new Save();
        foreach(GameObject targetGO in targetsGOs)
        {
            TargetManager targetManager = targetGO.GetComponent<TargetManager>();
            if(targetManager.activeMonster != null)
            {
                save.livingTargetPosition.Add(targetManager.targetPosition);
                int type = targetManager.activeMonster.GetComponent<MonsterManager>().monsterType;
                save.livingMonsterTypes.Add(type);
            }
        }
        save.shootNum = UIManager._instance.shootNum;
        save.score = UIManager._instance.score;

        return save;
    }

    private void SetGame(Save save)
    {
        //先将所有的Target里面的怪物清空，并重置所有的计时
        foreach(GameObject targetGO in targetsGOs)
        {
            targetGO.GetComponent<TargetManager>().UpdateMonster();
        }
        //通过反序列化得到的Save对象中储存的信息，激活指定的怪物
        for(int i = 0;i<save.livingTargetPosition.Count;i++)
        {
            int position = save.livingTargetPosition[i];
            int type = save.livingMonsterTypes[i];
            targetsGOs[position].GetComponent<TargetManager>().ActivateMonsterByType(type);
        }
        //更新UI显示
        UIManager._instance.shootNum = save.shootNum;
        UIManager._instance.score = save.score;
        //调整为未暂停状态
        UnPause();
    }
    //二进制存档
    private void SaveByBin()
    {
        //序列化过程（将Save对象转换为字节流）
        //创建Save对象并保存当前游戏状态
        Save save = CreateSaveGO();
        //创建一个二进制格式化程序
        BinaryFormatter bf = new BinaryFormatter();
        //创建一个文件流
        FileStream fileStream = File.Create(Application.dataPath + "/StreamingFile" + "/byBin.txt");
        //用二进制格式程序的序列化方法来序列化Save对象,参数：创建的文件流和需要序列化的对象
        bf.Serialize(fileStream, save);
        //关闭流
        fileStream.Close();


        if (File.Exists(Application.dataPath + "/StreamingFile" + "/byBin.txt"))
        {
            UIManager._instance.showMessage("保存成功");
        }
    }
    //二进制读档
    private void LoadByBin()
    {
        if(File.Exists(Application.dataPath + "/StreamingFile" + "/byBin.txt"))
        {
            //反序列化过程
            //创建一个二进制格式化程序
            BinaryFormatter bf = new BinaryFormatter();
            //打开一个文件流
            FileStream fileStream = File.Open(Application.dataPath + "/StreamingFile" + "/byBin.txt", FileMode.Open);
            //调用格式化程序的反序列化方法，将文件流转化为一个Save对象
            Save save = (Save)bf.Deserialize(fileStream);
            //关闭
            fileStream.Close();
            SetGame(save);
        }
        else
        {
            UIManager._instance.showMessage("存档文件不存在");
        }
    }


    private void SaveByXML()
    {

    }

    private void LoadByXML()
    {

    }


    private void SaveByJSon()
    {

    }

    private void LoadByJSon()
    {

    }


    public void ContinueGame()
    {
        UnPause();
        UIManager._instance.showMessage("");
    }

    public void NewGame()
    {
        foreach(GameObject go in targetsGOs)
        {
            go.GetComponent<TargetManager>().UpdateMonster();
        }
        UIManager._instance.score = 0;
        UIManager._instance.shootNum = 0;
        UIManager._instance.showMessage("");
        UnPause();
    }

    public void ExitGame()
    {
        Application.Quit();
    }


    public void SaveGame()
    {
        SaveByBin();
    }

    public void LoadGame()
    {
        LoadByBin();
        UIManager._instance.showMessage("");
    }
}
