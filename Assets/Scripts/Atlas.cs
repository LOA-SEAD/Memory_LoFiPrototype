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
    public List<CardPairClass> cardsData = new List<CardPairClass>();
    private EventSystem eventSystem;

    public AudioClip atlasHelpAudio, descAudio, firstAudio, pairAudio;
    private AtlasCard detailsCard;

    private AudioSource audioSource;

    public GameObject lastselect;
    private static bool started = false;
    private static bool isPlayingOrientation = true;

    //Function executed before Start()
    void Awake(){
        audioSource = this.GetComponent<AudioSource>();
        eventSystem = GetComponent<EventSystem>();
        //Execute only when the game starts
        if (!started){
            started = true;
            lastselect = new GameObject();
            lastselect = null;
            triggerOrientations();
        }
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
                this.detailsCard = card;
                this.cardA.text = card.cardData.textA;
                this.cardB.text = card.cardData.textB;
                this.descriptionText.text = card.cardData.description;
                this.detailPanel.SetActive(true);
                eventSystem.enabled = false;
                StartCoroutine("playPairDetails", card);
                break;
            }
        }
    }

    private IEnumerator playPairDetails(AtlasCard card) {
        List<AudioClip> clips = new List<AudioClip>();
        clips.Add(firstAudio);
        clips.Add(card.cardAudioA);
        clips.Add(pairAudio);
        clips.Add(card.cardAudioB);
        clips.Add(descAudio);
        clips.Add(card.descriptionAudio);

        isPlayingOrientation = true;
        foreach (AudioClip clip in clips)
        {
            if (isPlayingOrientation) {
                audioSource.clip = clip;
                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length);
            } else {
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPlayingOrientation) {
                StartCoroutine(stopOrientations());
            } else {
                if (detailPanel.activeSelf) {
                    if (isPlayingOrientation) {
                        audioSource.Stop();
                        isPlayingOrientation = false;
                    } else {
                        // Hides detail panel
                        detailPanel.SetActive(false);
                        eventSystem.enabled = true;
                    }
                } else {
                    // Trasition back to main menu
                    SceneManager.LoadScene(0);
                }
            }
        } else if (Input.GetKeyDown(KeyCode.K)) {
            if (!isPlayingOrientation){
                if (detailPanel.activeSelf) {
                    StartCoroutine("playPairDetails", detailsCard);
                } else {
                    triggerOrientations();
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
        string wwwPlayerFilePathD = "file://" + Application.streamingAssetsPath + "/" + ac.cardData.descriptionAudio;

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
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(wwwPlayerFilePathD, AudioType.WAV)) {
            yield return www.SendWebRequest();
            if (www.isNetworkError) {
            } else {
                ac.descriptionAudio = DownloadHandlerAudioClip.GetContent(www);
            }
        };
        
        ac.cardText.text = ac.cardData.textA;
        ac.cardAudio.clip = ac.cardAudioA;
    }

    private void loadCards()
    {
        String filepath = Application.streamingAssetsPath + "/";

        Debug.Log("Reading cards " + filepath);
        foreach (String level in filenames)
        {
            using (StreamReader sr = new StreamReader(filepath + level)) 
            {
                while (sr.Peek() >= 0) 
                {   
                    CardPairClass cpc = JsonUtility.FromJson<CardPairClass>(sr.ReadLine());
                    cardsData.Add(cpc);
                }
            }
        }

        SortCards();

        Debug.Log("Finished loading cards.");
    }

    private void SortCards() {
        cardsData.Sort((x, y)=> string.Compare(x.textA, y.textA) );
        int i = 0;
        foreach (CardPairClass cpc in cardsData)
        {
            atlasCards[i].cardData = cpc;
            StartCoroutine("loadAudio", atlasCards[i]);        
            i++;
        }
    }

    private IEnumerator playOrientation()
    {
        audioSource.Play();
        isPlayingOrientation = true;
        eventSystem.enabled = false;
        yield return new WaitForSeconds(audioSource.clip.length + 1.5f);
    }

    public void triggerOrientations(){
        if (!audioSource.isPlaying)
        {
            audioSource.clip = atlasHelpAudio;
            StartCoroutine(playOrientation());
        }
    }

    private IEnumerator stopOrientations() {
        audioSource.Stop();
        isPlayingOrientation = false;
        yield return new WaitForSeconds(0.5f);
        eventSystem.enabled = true;
        eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
    }
}
