using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardNumber {
    none,
    card1,
    card2,
    card3,
    card4
}

public class MemoryPairing : MonoBehaviour {
    public static bool canSelect;
    public Card previousCard;
    public Card currentCard;
    public float waitingTime;

    public int pairsTotal;
    public int pairsFound;

    public AudioClip correctPair;
    public AudioClip wrongPair;
    public AudioClip allPairsFound;
    public AudioClip wallSound;
    AudioSource audioSource;

    private BoardInputHandler board;

	// Use this for initialization
	void Start ()
    {
        canSelect = true;
        previousCard = null;
        audioSource = this.GetComponent<AudioSource>();
        board = this.GetComponent<BoardInputHandler>();

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
        yield return new WaitForSeconds(2.0f);

        // Play sound
        PlayAllPairsFound();

        yield return new WaitForSeconds(1.0f);
        //MainMenu();
    }

    #endregion


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

    public void PlayWrongPair()
    {
        audioSource.clip = wrongPair;
        audioSource.Play();
    }

    #endregion
}
