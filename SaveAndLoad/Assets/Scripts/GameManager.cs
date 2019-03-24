using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LitJson;
using System.Xml;

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
            UIManager._instance.showMessage("");
        }
        else
        {
            UIManager._instance.showMessage("存档文件不存在");
        }
    }


    private void SaveByXML()
    {
        Save save = CreateSaveGO();
        //创建xml文件的存储路径
        string filePath = Application.dataPath + "/StreamingFile" + "/byXML.txt";
        //创建一个xml文档
        XmlDocument xmlDoc = new XmlDocument();
        //创建根节点，即最上层节点
        XmlElement root = xmlDoc.CreateElement("save");
        //设置根节点中的值
        root.SetAttribute("name", "saveFile1");
        //创建XmlElement
        XmlElement target;
        XmlElement targetPosition;
        XmlElement monsterType;
        //遍历save中存储的数据，将数据转换成XML格式
        for(int i = 0; i < save.livingTargetPosition.Count; i++)
        {
            target = xmlDoc.CreateElement("target");

            targetPosition = xmlDoc.CreateElement("targetPosition");
            targetPosition.InnerText = save.livingTargetPosition[i].ToString();

            monsterType = xmlDoc.CreateElement("monsterType");
            monsterType.InnerText = save.livingMonsterTypes[i].ToString();

            //设置节点之间的层级关系root -> target -> (targetPosition,monsterType)
            target.AppendChild(targetPosition);
            target.AppendChild(monsterType);
            root.AppendChild(target);
        }
        //设置射击数和分数节点并设置层级关系 xmlDoc -> root -> (target,shootNum,score)
        XmlElement shootNum = xmlDoc.CreateElement("shootNum");
        shootNum.InnerText = save.shootNum.ToString();
        root.AppendChild(shootNum);

        XmlElement score = xmlDoc.CreateElement("score");
        score.InnerText = save.score.ToString();
        root.AppendChild(score);

        xmlDoc.AppendChild(root);
        xmlDoc.Save(filePath);
        if(File.Exists(filePath))
        {
            UIManager._instance.showMessage("保存成功");
        }
    }

    private void LoadByXML()
    {
        string filePath = Application.dataPath + "/StreamingFile" + "/byXML.txt";
        if (File.Exists(filePath))
        {
            Save save = new Save();
            //加载XML文档
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            //通过节点名称来获取元素，结果为xmlNodeList类型
            XmlNodeList targets = xmlDoc.GetElementsByTagName("target");
            
            //遍历所有的target节点，并获得子节点和子节点的innerText
            if(targets.Count != 0)
            {
                foreach(XmlNode target in targets)
                {
                    XmlNode targetPosition = target.ChildNodes[0];
                    int targetPositionIndex = int.Parse(targetPosition.InnerText);
                    //把得到的值存储到save中
                    save.livingTargetPosition.Add(targetPositionIndex);
                    XmlNode monsterType = target.ChildNodes[1];
                    int monsterTypeIndex = int.Parse(monsterType.InnerText);
                    save.livingMonsterTypes.Add(monsterTypeIndex);
                }
            }
            //用List来接收，得到存储的射击数和分数
            XmlNodeList shootNum = xmlDoc.GetElementsByTagName("shootNum");
            int shootNumCount = int.Parse(shootNum[0].InnerText);
            save.shootNum = shootNumCount;
            //XmlNode shootNum1 = xmlDoc.SelectSingleNode("shootNum");
            //int shootNumCount1 = int.Parse(shootNum1.InnerText);
            //save.shootNum = shootNumCount1;

            XmlNodeList score = xmlDoc.GetElementsByTagName("score");
            int scoreCount = int.Parse(score[0].InnerText);
            save.score = scoreCount;
            SetGame(save);
            UIManager._instance.showMessage("");
        }
        else
        {
            UIManager._instance.showMessage("存档文件不存在");
        }
    }


    private void SaveByJSon()
    {
        Save save = CreateSaveGO();
        string filePath = Application.dataPath + "/StreamingFile" + "/byJson.json";
        //利用JsonMapper将Save对象转化为json格式的字符串
        string saveJsonStr = JsonMapper.ToJson(save);
        //将该字符串写入文件中
        //创建一个StreamWritter,执行上一行注释
        StreamWriter streamWriter = new StreamWriter(filePath);
        streamWriter.Write(saveJsonStr);
        //关闭stringWritter
        streamWriter.Close();
        if (File.Exists(filePath))
        {
            UIManager._instance.showMessage("保存成功");
        }
    }

    private void LoadByJSon()
    {
        string filePath = Application.dataPath + "/StreamingFile" + "/byJson.json";
        if(File.Exists(filePath))
        {
            //创建一个SteamReader,用来读取文件流
            StreamReader streamReader = new StreamReader(filePath);
            //将读取到的流赋值给jsonStr
            string jsonStr = streamReader.ReadToEnd();
            streamReader.Close();

            //将字符串jsonStr转化为Save对象
            Save save = JsonMapper.ToObject<Save>(jsonStr);
            SetGame(save);
            UIManager._instance.showMessage("");

        }
        else
        {
            UIManager._instance.showMessage("存档文件不存在");
        }
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
        //SaveByBin();
        SaveByJSon();
        //SaveByXML();
    }

    public void LoadGame()
    {
        //LoadByBin();
        LoadByJSon();
        //LoadByXML();
    }
}
