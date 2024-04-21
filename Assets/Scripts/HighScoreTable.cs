using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;


public class HighScoreTable : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;
    private List<Transform> highscoreEntryTransformList;
    private void Awake()
    {
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("HSET");
     
        entryTemplate.gameObject.SetActive(false);

        //AddHighscoreEntry(100000, "Endrew");
        //AddHighscoreEntry(10200, "Andrew");
        //AddHighscoreEntry(10300, "Andrew");

        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);
        

        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
            {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                {
                    HighScoreEntry tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }

        highscoreEntryTransformList = new List<Transform>();
        foreach(var highscoreEntry in highscores.highscoreEntryList)
        {
            CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
        }
    }
    public static void AddHighscoreEntry(int score,string name) //додавання нового запису в таблицю
    {
        //Create HighscoreEntry
        HighScoreEntry highScoreEntry = new HighScoreEntry { score = score, name = name };
        //Load saved Highscores
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);
        //Add new entry to Highscores

        bool findMatch = false;
        foreach(HighScoreEntry item in highscores.highscoreEntryList)
        {
            if(highScoreEntry.name == item.name)
            {
                item.score = highScoreEntry.score;
                findMatch = true;
                break;
            }
        }
        if (!findMatch)
        {
            highscores.highscoreEntryList.Add(highScoreEntry);
        }

        //Save updated Highscores
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }
    public void ClearListOfScoreEntry() //очистка таблиці
    {
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);
        
        highscores.highscoreEntryList.Clear();

        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }
    private void CreateHighscoreEntryTransform(HighScoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHight = 20f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank)
        {
            case 1:
                rankString = "1ST";
                break;
            case 2:
                rankString = "2ND";
                break;
            case 3:
                rankString = "3RD";
                break;
            default:
                rankString = rank + "TH";
                break;
        }
        entryTransform.Find("positionText").GetComponent<Text>().text = rankString;

        int score = highscoreEntry.score;
        entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

        string name = highscoreEntry.name;
        entryTransform.Find("nameText").GetComponent<Text>().text = name;

        entryTransform.Find("background").gameObject.SetActive(rank % 2 == 1);
        if(rank == 1)
        {
            entryTransform.Find("positionText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("scoreText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("nameText").GetComponent<Text>().color = Color.green;
        }

        transformList.Add(entryTransform);
    }
}
