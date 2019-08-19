using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Atlas : MonoBehaviour
{
    

    public GameObject detailPanel;
    public Text cardA;
    public Text cardB;
    public Text descriptionText;
    public List<String> filenames;
    
    private EventSystem eventSystem;
    private List<CardPairClass> cardsData = new List<CardPairClass>();

    public List<Text> pairGridTexts = new List<Text>();

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = GetComponent<EventSystem>();
    

        String filepath = Application.streamingAssetsPath + "/";

        Debug.Log("Reading cards " + filepath);
        int i = 0;
        foreach (String level in filenames)
        {
            using (StreamReader sr = new StreamReader(filepath + level)) 
            {
                while (sr.Peek() >= 0) 
                {   
                    CardPairClass cc = JsonUtility.FromJson<CardPairClass>(sr.ReadLine());
                    cardsData.Add(cc);
                    pairGridTexts[i].text = cc.textA;
                    i++;
                }
            }
        }

        Debug.Log("Finished loading cards.");


    }

    public void openPairDetails(int pairNumber)
    {
        foreach (CardPairClass card in this.cardsData)
        {
            if (card.pairNumber == pairNumber)
            {
                this.cardA.text = card.textA;
                this.cardB.text = card.textB;
                this.descriptionText.text = card.description;
                this.detailPanel.SetActive(true);
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (detailPanel.activeSelf) {
                // Hides detail panel
                detailPanel.SetActive(false);
            } else {
                // Trasition back to main menu
                SceneManager.LoadScene(0);
            }
        }
    }
}
