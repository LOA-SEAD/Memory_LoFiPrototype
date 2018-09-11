using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	//To disable the mouse, together with Graphic Raycaster of Canvas unchecked
	private GameObject lastselect;
	private EventSystem eventSystem;
    public AudioClip orientationMenu;
    public AudioClip orientationInstrucions1;
    public AudioClip orientationInstrucions2;
    public AudioClip orientationInstrucions3;
    public AudioClip orientationInstrucions4;
    public AudioClip orientationInstrucions5;
    public AudioClip orientationInstrucions6;
    public AudioClip orientationInstrucions7;
    public AudioClip edgeSound;
    public AudioClip emptySound;
    public AudioClip selectedSound;
    private static bool started = false;
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
            triggerOrientations(orientationMenu);
        }
    }

	// Use this for initialization
	void Start()
    {
    }

    // Update is called once per frame
    void Update () {         
        if (eventSystem.currentSelectedGameObject == null)
        {
            eventSystem.SetSelectedGameObject(lastselect);
        }
        else
        {
            lastselect = eventSystem.currentSelectedGameObject;
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
    public IEnumerator ReadOrientations(AudioClip orientation)
    {
        //Start of game
        if (lastselect == null){
            eventSystem.enabled = false;
        }
        audioSource.clip = orientation;
        audioSource.Play();
        if (lastselect != null){
            yield return new WaitForSeconds(orientation.length + 0.1f);
            PlayNewOrientations(edgeSound);
            yield return new WaitForSeconds(edgeSound.length + 0.1f);
            PlayNewOrientations(orientationInstrucions2);
            yield return new WaitForSeconds(orientationInstrucions2.length + 0.1f);
            PlayNewOrientations(orientationInstrucions3);
            yield return new WaitForSeconds(orientationInstrucions3.length + 0.1f);
            PlayNewOrientations(orientationInstrucions4);
            yield return new WaitForSeconds(orientationInstrucions4.length + 0.1f);
            PlayNewOrientations(emptySound);
            yield return new WaitForSeconds(emptySound.length + 0.1f);
            PlayNewOrientations(orientationInstrucions5);
            yield return new WaitForSeconds(orientationInstrucions5.length + 0.1f);
            PlayNewOrientations(selectedSound);
            yield return new WaitForSeconds(selectedSound.length + 0.1f);
            PlayNewOrientations(orientationInstrucions6);
            yield return new WaitForSeconds(orientationInstrucions6.length + 0.1f);
            PlayNewOrientations(orientationInstrucions7);
            yield return new WaitForSeconds(orientationInstrucions7.length + 0.2f);
        }
        else{
            yield return new WaitForSeconds(orientation.length + 0.4f);
            eventSystem.enabled = true;
            eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
        }
    }

    public void PlayNewOrientations(AudioClip orientation)
    {
        audioSource.clip = orientation;
        audioSource.Play();
    }

    public void triggerOrientations(AudioClip orientation){
        StartCoroutine(ReadOrientations(orientation));
    }
}
