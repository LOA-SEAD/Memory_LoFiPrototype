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
    AudioSource audioSource;

	// Use this for initialization
	void Start ()
    {
        canSelect = true;
        previousCard = null;
        audioSource = this.GetComponent<AudioSource>();
	}

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
            yield return new WaitForSeconds(waitingTime);

            if (card.cardNumber == previousCard.cardNumber) //Correct Pair
            {
                CorrectPair();
            } else //Wrong pair
            {
                WrongPair();
            }
            canSelect = true;
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
        audioSource.clip = correctPair;
        audioSource.Play();
        
        currentCard.SetAsFound();
        previousCard.SetAsFound();
    }

    void WrongPair()
    {

        // Play sound
        audioSource.clip = wrongPair;
        audioSource.Play();

        currentCard.UnFlip();
        previousCard.UnFlip();
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(2.0f);

        // Play sound
        audioSource.clip = allPairsFound;
        audioSource.Play();

        yield return new WaitForSeconds(1.0f);
        //MainMenu();
    }
}
