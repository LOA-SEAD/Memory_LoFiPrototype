using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class AfterLevelMenu : MonoBehaviour {

	//To disable the mouse, together with Graphic Raycaster of Canvas unchecked
	private GameObject lastselect;
	private EventSystem eventSystem;
    /*public AudioClip orientationMenu;
    public AudioClip orientationInstrucions;
    public AudioClip emptySound;
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
    }*/

	// Use this for initialization
	void Start()
    {
    	eventSystem = GetComponent<EventSystem>();
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

    public void ChangeScene(int number)
    {
        try
        {
        	if (number > 0){
            	SceneManager.LoadScene(MemoryPairing.lastLevel + number);
        	}
        	else
        	{
        		SceneManager.LoadScene(number);
        	}
        }
        catch
        {
        }

    }

    //Stop the Event System and read an orientation
    /*public IEnumerator ReadOrientations(AudioClip orientation)
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
        }
        else{
            yield return new WaitForSeconds(0.4f);
            eventSystem.enabled = true;
            eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
        }
    }

    public void triggerOrientations(AudioClip orientation){
        StartCoroutine(ReadOrientations(orientation));
    }*/
}
