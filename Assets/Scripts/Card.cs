using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour {

    public CardNumber cardNumber;
    public AudioClip selectSound;
    public AudioClip wallSound;
    public AudioClip contentValue;
    public AudioClip invalidCard;
    public AudioClip selectedCard;
    public Image hoverEffect;
    public float flippingTime;
    private float selectPitch;

    public Sprite openSprite, closedSprite;
    private bool found = false;
    private bool flipped = false;
    private MemoryPairing memoryPairing;
    private AudioSource audioSource;
    public Image cardSprite;

    private Text text;

	// Use this for initialization
	void Start () {
        memoryPairing = FindObjectOfType<MemoryPairing>();
        audioSource = this.GetComponent<AudioSource>();
        text = this.GetComponentInChildren<Text>();
        selectPitch = audioSource.pitch;
    }

    private void OnMouseEnter()
    {
        SelectCard();
    }
    private void OnMouseExit()
    {
        DeselectCard();
    }

    private void OnMouseDown()
    {
        TrySubmit();
    }

    public void TrySubmit()
    {
        if (MemoryPairing.canSelect)
        {
            if (!found && !flipped)
            {
                StartCoroutine(ActivateCard());
            }
            else
            {
                //empty sound to removed cards
                if (found){
                    PlayRemovedCard();
                }
                else{
                    PlayContentValue();
                }
            }
        }
    }

    public void SelectCard()
    {
        if (found)
        {
            PlayRemovedCard();
        } 
        else 
        {
            if(!flipped)
            {
                PlaySelectSound();
            }
            else
            {
                StartCoroutine(PlaySelectedCard());
            }
        }
        //Activate hover effect
        hoverEffect.enabled = true;
    }
    public void DeselectCard()
    {
        //Deactivate hover effect
        hoverEffect.enabled = false;
    }
    
    public IEnumerator ActivateCard() {
        Flip();
        yield return new WaitForSeconds(flippingTime);
        StartCoroutine(memoryPairing.ActivateCard(this));
    }

    public void SetAsFound()
    {
        audioSource.volume = 0.5f;
        found = true;
        SpriteRenderer[] cardSprites = GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer sprite in cardSprites)
        {
            sprite.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);        
        }
    }

    private void Flip() {
        //transform.rotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
        cardSprite.sprite = openSprite;
        text.enabled = true;
        PlayContentValue();
        flipped = true;
    }
    public void UnFlip() {
        //transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
        cardSprite.sprite = closedSprite;
        text.enabled = false;
        flipped = false;
    }


    public void PlaySelectSound()
    {
        audioSource.pitch = selectPitch;
        audioSource.clip = selectSound;
        audioSource.Play();
    }

    public void PlayWallSound()
    {
        audioSource.pitch = 1.0f;
        audioSource.clip = wallSound;
        audioSource.Play();
    }

    public void PlayContentValue()
    {
        audioSource.pitch = 1.0f;
        audioSource.clip = contentValue;
        audioSource.Play();
    }

    public void PlayRemovedCard()
    {
        audioSource.pitch = 1.0f;
        audioSource.clip = invalidCard;
        audioSource.Play();
    }

    public IEnumerator PlaySelectedCard()
    {
        audioSource.pitch = selectPitch;
        audioSource.clip = selectedCard;
        audioSource.Play();
        yield return new WaitForSeconds(selectedCard.length + 0.5f);
        PlayContentValue();
    }
}
