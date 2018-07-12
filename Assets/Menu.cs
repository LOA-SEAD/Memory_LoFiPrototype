using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	//To disable the mouse, together with Graphic Raycaster of Canvas unchecked
	private GameObject lastselect;
	private EventSystem eventSystem;

	// Use this for initialization
	void Start()
    {
    	eventSystem = GetComponent<EventSystem>();
        lastselect = new GameObject();
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
}
