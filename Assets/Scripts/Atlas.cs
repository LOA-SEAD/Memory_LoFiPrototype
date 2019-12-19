using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Atlas : MonoBehaviour
{
    public GameObject detailPanel;
    public Text cardA;
    public Text cardB;
    public Text descriptionText;
    public Text hintText;
    public List<String> filenames;
    public List<AtlasCard> atlasCards;  
    public List<CardPairClass> cardsData = new List<CardPairClass>();
    private EventSystem eventSystem;

    public AudioClip atlasHelpAudio, descAudio, firstAudio, pairAudio;
    private AtlasCard detailsCard;

    private AudioSource audioSource;

    public GameObject lastselect;
    private static bool isPlayingAtlasOrientation = true;
    private bool isPlayingCardDetails = false;

    //Function executed before Start()
    void Awake(){
        audioSource = this.GetComponent<AudioSource>();
        eventSystem = GetComponent<EventSystem>();
        lastselect = new GameObject();
        lastselect = null;
        triggerOrientations();
    }

    // Start is called before the first frame update
    void Start()
    {
        BetterStreamingAssets.Initialize();
        #if UNITY_ANDROID
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Screen.fullScreen = false;
        #endif

        loadCards();
    }

    public void openPairDetails(AtlasCard card)
    {
        card.cardAudio.volume = 0;
        this.detailsCard = card;
        this.cardA.text = card.cardData.textA;
        this.cardB.text = card.cardData.textB;
        this.descriptionText.text = card.cardData.description;
        this.detailPanel.SetActive(true);
        eventSystem.enabled = false;
        #if UNITY_ANDROID
            hintText.text = "Pressione [◁] para pular a descrição.";
        #else
            hintText.text = "Pressione [Esc] para pular a descrição.";
        #endif
        StartCoroutine("playPairDetails", card);
    }

    private IEnumerator playPairDetails(AtlasCard card) {
        List<AudioClip> clips = new List<AudioClip>();
        clips.Add(firstAudio);
        clips.Add(card.cardAudioA);
        clips.Add(pairAudio);
        clips.Add(card.cardAudioB);
        clips.Add(descAudio);
        clips.Add(card.descriptionAudio);
        isPlayingCardDetails = true;
        foreach (AudioClip clip in clips)
        {
            if (isPlayingCardDetails) {
                audioSource.clip = clip;
                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length + 0.8f);
                card.cardAudio.volume = 1;
            } else {
                break;
            }
        }

        #if UNITY_ANDROID
            hintText.text = "Pressione [◁] para voltar à lista de cartas.";
        #else
            hintText.text = "Pressione [Esc] para voltar à lista de cartas.";
        #endif
    }

    public void ReturnToMenu(){
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPlayingAtlasOrientation || isPlayingCardDetails) {
                StartCoroutine(stopOrientations());
            } else {
                if (detailPanel.activeSelf) {
                    if (isPlayingAtlasOrientation) {
                        audioSource.Stop();
                        isPlayingAtlasOrientation = false;
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
            if (!isPlayingAtlasOrientation){
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
        #if UNITY_ANDROID
            string wwwPlayerFilePath = Application.streamingAssetsPath + "/" + ac.cardData.audioA;
            string wwwPlayerFilePathB = Application.streamingAssetsPath + "/" + ac.cardData.audioB;
            string wwwPlayerFilePathD = Application.streamingAssetsPath + "/" + ac.cardData.descriptionAudio;
        #else
            string wwwPlayerFilePath = "file://" + Application.streamingAssetsPath + "/" + ac.cardData.audioA;
            string wwwPlayerFilePathB = "file://" + Application.streamingAssetsPath + "/" + ac.cardData.audioB;
            string wwwPlayerFilePathD = "file://" + Application.streamingAssetsPath + "/" + ac.cardData.descriptionAudio;
        #endif

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
        ac.cardAudio.clip = ac.cardAudioA;
        ac.cardText.text = ac.cardData.textA;
    }

    private void loadCards()
    {
        var jsonFiles = BetterStreamingAssets.GetFiles("Json").Where(x => Path.GetExtension(x) == ".json").ToList();
        foreach (string level in jsonFiles)
        {

            Debug.Log("Found file");
            Debug.Log("Reading " + level);
            var jsonText = BetterStreamingAssets.ReadAllLines(level);
            foreach (var line in jsonText)
            {
                CardPairClass cpc = JsonUtility.FromJson<CardPairClass>(line);
                cardsData.Add(cpc);
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
        isPlayingAtlasOrientation = true;
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
        
        yield return new WaitForSeconds(0.5f);
        
        if (!isPlayingCardDetails) {
            eventSystem.enabled = true;
            eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
            
        } else {
            #if UNITY_ANDROID
                hintText.text = "Pressione [◁] para voltar à lista de cartas.";
            #else
                hintText.text = "Pressione [Esc] para voltar à lista de cartas.";
            #endif
        }

        isPlayingAtlasOrientation = false;
        isPlayingCardDetails = false;
    }
}
