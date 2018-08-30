using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum CardNumber {
    none,
    card1,
    card2,
    card3,
    card4,
    card5,
    card6
}

public class MemoryPairing : MonoBehaviour {
    public static bool canSelect;
    public Card previousCard;
    public Card currentCard;
    public float waitingTime;
    private EventSystem eventSystem;

    public int pairsTotal;
    public int pairsFound;

    public AudioClip correctPair;
    public AudioClip wrongPair;
    public AudioClip allPairsFound;
    public AudioClip wallSound;
    public AudioClip backToMenu;
    public AudioClip orientationLevel;
    public AudioClip orientationLevelKeyK;
    public AudioClip orientationCongratulations;
    public AudioClip orientationCongratulations2;
    AudioSource audioSource;  
    private bool isPlayingOrientation = true;

    private BoardInputHandler board;

    public static int lastLevel;

    //Function executed before Start()
    void Awake(){
        audioSource = this.GetComponent<AudioSource>();
        board = this.GetComponent<BoardInputHandler>();
        canSelect = false;
        eventSystem = GetComponent<EventSystem>();
        //Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        // Hide cursor when locking
        Cursor.visible = false;
        triggerOrientations(orientationLevel);
    }

	// Use this for initialization
	void Start ()
    {
        canSelect = true;
        previousCard = null;
    }

    void Update () {
        //Pause orientation and start the level
        if(Input.GetKeyDown(KeyCode.K) && (isPlayingOrientation)) {  //Moving
            audioSource.Stop();
            isPlayingOrientation = false;
            StartCoroutine(StartLevel());
        }
    }

    IEnumerator StartLevel(){
        yield return new WaitForSeconds(0.5f);
        eventSystem.enabled = true;
        eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
        board.enabled = true;
    }

    #region Memory Game logics
    public IEnumerator ActivateCard(Card card)
    {
        currentCard = card;
        if(previousCard == null)
        {
            previousCard = card;
            currentCard = null;
        } else
        {
            canSelect = false;
            board.enabled = false;
            yield return new WaitForSeconds(waitingTime);

            if (card.cardNumber == previousCard.cardNumber) //Correct Pair
            {
                CorrectPair();
            } else //Wrong pair
            {
                WrongPair();
            }
            canSelect = true;
            board.enabled = true;

            previousCard = null;
            currentCard = null;

            if(pairsFound == pairsTotal)
            {
                StartCoroutine(EndGame());
            }
        }
    }

    void CorrectPair()
    {
        pairsFound++;

        // Play sound
        PlayCorrectPair();
        
        currentCard.SetAsFound();
        previousCard.SetAsFound();
    }

    void WrongPair()
    {

        // Play sound
        PlayWrongPair();

        currentCard.UnFlip();
        previousCard.UnFlip();
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(correctPair.length + 1.0f); //original: 2.0F / last value: 1.0F

        // Play sound
        PlayAllPairsFound();

        yield return new WaitForSeconds(allPairsFound.length + 0.2f); //original: 1.0F

        // Play congratulations
        PlayOrientationCongratulations(orientationCongratulations);
        yield return new WaitForSeconds(orientationCongratulations.length + 0.1f); //original: 1.0F
        if (SceneManager.GetActiveScene().buildIndex != 3)
        {
        	PlayOrientationCongratulations(orientationCongratulations2);
        	yield return new WaitForSeconds(orientationCongratulations2.length + 0.5f); //original: 1.0F
    	}

        //Release cursor
        Cursor.lockState = CursorLockMode.None;
        //Change cursor to visible again
        Cursor.visible = true;
        //Player can choice to go to next level or to main menu in the first and second levels
        if (SceneManager.GetActiveScene().buildIndex != 3)
        {
        	//Get the index of level played
        	lastLevel = SceneManager.GetActiveScene().buildIndex;
        	//AfterLevelMenu()
        	SceneManager.LoadScene(4);
        }
        else
        {
	        //MainMenu();
	        SceneManager.LoadScene(0);
	    }
    }

    public void triggerOrientations(AudioClip orientation){
        StartCoroutine(ReadOrientations(orientation));
    }

    #endregion

    /*#region After Level Menu logics
    public int getLastLevelIndex(){
    	return lastLevel;
    }
    #endregion*/


    #region Play sound methods
    public void PlayCorrectPair()
    {
        audioSource.clip = correctPair;
        audioSource.Play();
    }

    public void PlayAllPairsFound()
    {
        audioSource.clip = allPairsFound;
        audioSource.Play();
    }

    public void PlayOrientationCongratulations(AudioClip orientationEndLevel)
    {
        audioSource.clip = orientationEndLevel;
        audioSource.Play();
    }

    public void PlayWrongPair()
    {
        audioSource.clip = wrongPair;
        audioSource.Play();
    }
    
    public void PlayBackToMenu()
    {
        audioSource.clip = backToMenu;
        audioSource.Play();
    }

    public void PlayLineCard(AudioClip line)
    {
        audioSource.clip = line;
        audioSource.Play();
    }

    public IEnumerator PlayColumnCard(AudioClip column)
    {
        //Timer to not overlap the line audio 
        yield return new WaitForSeconds(1.5f);
        audioSource.clip = column;
        audioSource.Play();
    }

    //Stop the Event System and read the level's orientation
    public IEnumerator ReadOrientations(AudioClip orientation)
    {
        board.enabled = false;
        eventSystem.enabled = false;
        audioSource.clip = orientation;
        audioSource.Play();
        yield return new WaitForSeconds(orientation.length + 0.1f);
        if (isPlayingOrientation){
        	audioSource.clip = orientationLevelKeyK;
        	audioSource.Play();
        	yield return new WaitForSeconds(orientationLevelKeyK.length + 0.1f);
    	}
        if (isPlayingOrientation){
            isPlayingOrientation = false;
            StartCoroutine(StartLevel());
        }
    }

    #endregion
}
