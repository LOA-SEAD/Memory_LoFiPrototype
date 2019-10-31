using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Atlas : MonoBehaviour
{
    

    public GameObject detailPanel;
    public Text cardA;
    public Text cardB;
    public Text descriptionText;
    public List<String> filenames;
    public List<AtlasCard> atlasCards;    
    private EventSystem eventSystem;

    private AudioSource audioSource;

    public GameObject lastselect;
    public bool started = false;

    private Boolean playingOrientations = false;

    //Function executed before Start()
    void Awake(){
        audioSource = this.GetComponent<AudioSource>();
        eventSystem = GetComponent<EventSystem>();
        //Execute only when the game starts
        lastselect = null;
        triggerOrientations();
    }

    // Start is called before the first frame update
    void Start()
    {
        loadCards();
    }

    public void openPairDetails(int pairNumber)
    {
        foreach (AtlasCard card in this.atlasCards)
        {
            if (card.cardData.pairNumber == pairNumber)
            {
                this.cardA.text = card.cardData.textA;
                this.cardB.text = card.cardData.textB;
                this.descriptionText.text = card.cardData.description;
                this.detailPanel.SetActive(true);
                break;
            }
        }
    }

    private IEnumerator playPairDetails(AtlasCard card) {
        return null;
    } 

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (playingOrientations) {
                stopOrientations();
                eventSystem.enabled = true;
                eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
            } else {
                if (detailPanel.activeSelf) {
                    // Hides detail panel
                    detailPanel.SetActive(false);
                } else {
                    // Trasition back to main menu
                    SceneManager.LoadScene(0);
                }
            }
        } else {
            if (eventSystem.currentSelectedGameObject == null)
            {
                eventSystem.SetSelectedGameObject(lastselect);
            }
            else
            {
                lastselect = eventSystem.currentSelectedGameObject;
            }
        }
    }

     IEnumerator loadAudio(AtlasCard ac) {
        string wwwPlayerFilePath = "file://" + Application.streamingAssetsPath + "/" + ac.cardData.audioA;
        string wwwPlayerFilePathB = "file://" + Application.streamingAssetsPath + "/" + ac.cardData.audioB;

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(wwwPlayerFilePath, AudioType.WAV)) {
            yield return www.SendWebRequest();
            if (www.isNetworkError) {
            } else {
                ac.cardAudioA = DownloadHandlerAudioClip.GetContent(www);
            }
        };
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(wwwPlayerFilePathB, AudioType.WAV)) {
            yield return www.SendWebRequest();
            if (www.isNetworkError) {
            } else {
                ac.cardAudioB = DownloadHandlerAudioClip.GetContent(www);
            }
        };
        
        ac.cardText.text = ac.cardData.textA;
        ac.cardAudio.clip = ac.cardAudioA;
    }

    private void loadCards()
    {
        String filepath = Application.streamingAssetsPath + "/";

        Debug.Log("Reading cards " + filepath);
        int i = 0;
        foreach (String level in filenames)
        {
            using (StreamReader sr = new StreamReader(filepath + level)) 
            {
                while (sr.Peek() >= 0) 
                {   
                    CardPairClass cpc = JsonUtility.FromJson<CardPairClass>(sr.ReadLine());
                    atlasCards[i].cardData = cpc;
                    StartCoroutine("loadAudio",atlasCards[i]);
                    i++;
                }
            }
        }

        Debug.Log("Finished loading cards.");
    }

    private IEnumerator playOrientation()
    {
        audioSource.Play();
        playingOrientations = true;
        eventSystem.enabled = false;
        yield return new WaitForSeconds(audioSource.clip.length + 0.1f);
    }

    public void triggerOrientations(){
        if (!audioSource.isPlaying)
        {
            StartCoroutine(playOrientation());
        }
    }

    private void stopOrientations() {
        audioSource.Stop();
        eventSystem.enabled = true;
        playingOrientations = false;
    }
}
