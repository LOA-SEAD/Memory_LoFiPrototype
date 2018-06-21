using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {

    public CardNumber cardNumber;
    public AudioClip selectSound;
    public AudioClip wallSound;
    public AudioClip contentValue;
    public AudioClip invalidCard;
    public SpriteRenderer hoverEffect;
    public float flippingTime;
    private float selectPitch;

    private bool found = false;
    private bool flipped = false;
    private MemoryPairing memoryPairing;
    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        hoverEffect.enabled = false;
        memoryPairing = FindObjectOfType<MemoryPairing>();
        audioSource = this.GetComponent<AudioSource>();
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
                //Play removed card sound
                PlayContentValue();
            }
        }
    }

    public void SelectCard()
    {
        if(!flipped)
        {
            PlaySelectSound();
        }
        else
        {
            PlayRemovedCard();
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
            sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);           
        }
        //hoverEffect.color = new Color(237.0f/255.0f, 1.0f, 0.0f, 0.5f);
    }

    private void Flip() {
        transform.rotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
        PlayContentValue();
        flipped = true;
    }
    public void UnFlip() {
        transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
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
}
