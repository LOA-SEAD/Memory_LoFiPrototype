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
    public AudioClip orientationMenu2;
    public AudioClip orientationInstrucions;
    public AudioClip orientationInstrucions2;
    public AudioClip orientationInstrucions3;
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

    //Stop the Event System and read an orientation
    public IEnumerator ReadOrientations(AudioClip orientation)
    {
        //Start of game
        if (lastselect == null){
            eventSystem.enabled = false;
        }
        audioSource.clip = orientation;
        audioSource.Play();
        yield return new WaitForSeconds(orientation.length + 0.1f);
        if (lastselect != null){
            audioSource.clip = emptySound;
            audioSource.Play();
            yield return new WaitForSeconds(emptySound.length + 0.1f);
            audioSource.clip = orientationInstrucions2;
            audioSource.Play();
            yield return new WaitForSeconds(orientationInstrucions2.length + 0.1f);
            audioSource.clip = selectedSound;
            audioSource.Play();
            yield return new WaitForSeconds(selectedSound.length + 0.1f);
            audioSource.clip = orientationInstrucions3;
            audioSource.Play();
            yield return new WaitForSeconds(orientationInstrucions3.length + 0.2f);
        }
        else{
            audioSource.clip = orientationMenu2;
            audioSource.Play();
            yield return new WaitForSeconds(orientationMenu2.length + 0.4f);
            eventSystem.enabled = true;
            eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
        }
    }

    public void triggerOrientations(AudioClip orientation){
        StartCoroutine(ReadOrientations(orientation));
    }
}
