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
    private List<CardClass> cardsData = new List<CardClass>();

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
                
                int j = 0;
                while (sr.Peek() >= 0) 
                {   
                    CardClass cc = JsonUtility.FromJson<CardClass>(sr.ReadLine());
                    cardsData.Add(cc);
                    j++;

                    if (j % 2 != 0) 
                    {
                        pairGridTexts[i].text = cc.cardText;
                        i++;
                    }
                }
            }
        }

        Debug.Log("Finished loading cards.");


    }

    public void openPairDetails(int pairNumber)
    {
        bool foundOneCard = false;

        foreach (CardClass card in this.cardsData)
        {
            if (card.cardNumber == pairNumber)
            {
                // Found first card for pair
                if (!foundOneCard)
                {
                    this.cardA.text = card.cardText;
                    foundOneCard = true;
                } else
                // Found second card for pair
                {
                    this.cardB.text = card.cardText;
                    this.detailPanel.SetActive(true);
                    break;
                }
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
