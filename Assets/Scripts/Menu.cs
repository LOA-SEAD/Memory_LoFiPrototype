using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	private GameObject lastselect;
	private EventSystem eventSystem;
    public  AudioClip menuInstructions;
    public List<AudioClip> instructions;
    public AudioClip edgeSound;
    public AudioClip emptySound;
    public AudioClip selectedSound;
    private static bool started = false;
    private static bool isPlayingOrientation = true;
    AudioSource audioSource;

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

	// Use this for initialization
	void Start()
    {
        #if UNITY_ANDROID
            Screen.orientation = ScreenOrientation.Portrait;
            Screen.fullScreen = false;
        #endif
    }

    // Update is called once per frame
    void Update () {         
        //Pause orientation and start the level
        if(Input.GetKeyDown(KeyCode.Escape) && (isPlayingOrientation)) {  //Moving
            audioSource.Stop();
            isPlayingOrientation = false;
            eventSystem.enabled = true;
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

    public void ChangePhase(int number)
    {
        try
        {
            SceneManager.LoadScene(number);
        }
        catch
        {
        }

    }

    public void quit()
    {
        Application.Quit();

    }

    //Stop the Event System and read an orientation
    public IEnumerator ReadOrientations()
    {
        //Start of game
        if (lastselect == null) {
            eventSystem.enabled = false;
            audioSource.clip = menuInstructions;
            audioSource.Play();
            yield return new WaitForSeconds(menuInstructions.length + 2.0f);
            eventSystem.enabled = true;
        } else {
            isPlayingOrientation = true;
            eventSystem.enabled = false;
            foreach (AudioClip audio in instructions)
            {
                if (isPlayingOrientation){
                    PlayNewOrientations(audio);
                    yield return new WaitForSeconds(audio.length + 0.5f);
                } else {
                    break;
                }
            }
            eventSystem.enabled = true;
        }
    }

    public void PlayNewOrientations(AudioClip orientation)
    {
        audioSource.clip = orientation;
        audioSource.Play();
    }

    public void triggerOrientations(){
        if (!audioSource.isPlaying)
        {
            StartCoroutine(ReadOrientations());
        }
    }
}
