using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardInputHandler : MonoBehaviour {

    public List<GameObject> row1;
    public List<GameObject> row2;
    private EventSystem eventSystem;

	// Use this for initialization
	void Start () {
        eventSystem = GetComponent<EventSystem>();
    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.RightArrow)) {  //Moving
            MoveRight();
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            MoveLeft();
        } else if(Input.GetKeyDown(KeyCode.UpArrow)) {
            MoveUp();
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            MoveDown();
        } else if(Input.GetKeyDown(KeyCode.Space)) { //Submit
            Submit();
        } else if (Input.GetKeyDown(KeyCode.Q)) { //Direct access
            Access(0, 0);
        } else if(Input.GetKeyDown(KeyCode.W)) {
            Access(0, 1);
        } else if (Input.GetKeyDown(KeyCode.A)) {
            Access(1, 0);
        } else if (Input.GetKeyDown(KeyCode.S)) {
            Access(0, 1);
        }
    }

    private void Submit() {
        eventSystem.currentSelectedGameObject.GetComponent<Card>().TrySubmit();
    }

    private void MoveDown()
    {
        if (row1.Contains(eventSystem.currentSelectedGameObject))
        {
            //select Next card
            eventSystem.SetSelectedGameObject(row2[row1.IndexOf(eventSystem.currentSelectedGameObject)]);
        }
        else
        {
            //play wall sound
        }
    }

    private void MoveUp()
    {
        if (row2.Contains(eventSystem.currentSelectedGameObject))
        {
            //select next card
            eventSystem.SetSelectedGameObject(row1[row2.IndexOf(eventSystem.currentSelectedGameObject)]);
        }
        else
        {
            //play wall sound
        }
    }

    private void MoveLeft()
    {
        if (row1.Contains(eventSystem.currentSelectedGameObject))
        {
            //get the index of card
            int cardIndex = row1.IndexOf(eventSystem.currentSelectedGameObject);

            if (cardIndex == 0)
            {
                //play wall sound
            }
            else
            {
                //Select next card
                eventSystem.SetSelectedGameObject(row1[cardIndex - 1]);
            }
        }
        else
        {
            //get the index of card
            int cardIndex = row2.IndexOf(eventSystem.currentSelectedGameObject);

            if (cardIndex == 0)
            {
                //play wall sound
            }
            else
            {
                //Select next card
                eventSystem.SetSelectedGameObject(row2[cardIndex - 1]);
            }
        }
    }

    private void MoveRight()
    {
        if (row1.Contains(eventSystem.currentSelectedGameObject))
        {
            //get the index of card
            int cardIndex = row1.IndexOf(eventSystem.currentSelectedGameObject);

            if (cardIndex == row1.Count - 1)
            {
                //play wall sound
            }
            else
            {
                //Select next card
                eventSystem.SetSelectedGameObject(row1[cardIndex + 1]);
            }
        }
        else
        {
            //get the index of card
            int cardIndex = row2.IndexOf(eventSystem.currentSelectedGameObject);

            if (cardIndex == row2.Count - 1)
            {
                //play wall sound
            }
            else
            {
                //Select next card
                eventSystem.SetSelectedGameObject(row2[cardIndex + 1]);
            }
        }
    }

    private void Access(int v1, int v2)
    {
        if (v1 == 0)
        {
            row1[v2].GetComponent<Card>().ActivateCard();
        } else
        {
            row2[v2].GetComponent<Card>().ActivateCard();
        }
    }

    // Handles OnMove events
    /*void OnMove(AxisEventData eventData)
    {
        Debug.Log("moving" + eventData.ToString());

        if(eventData.moveDir == MoveDirection.Down)
        { 
            if(row1.Contains(eventData.selectedObject))
            {
                //select Next card
            }
            else
            {
                //play wall sound
            }
        } else if (eventData.moveDir == MoveDirection.Up)
        {
            if (row1.Contains(eventData.selectedObject))
            {
                //play wall sound
            }
            else
            {
                //select next card
            }
        }
        else if(eventData.moveDir == MoveDirection.Right)
        {
            if(row1.Contains(eventData.selectedObject))
            {
                //get the index of card
                int cardIndex = row1.IndexOf(eventData.selectedObject);
                
                if(cardIndex == row1.Count)
                {
                    //play wall sound
                }
                else
                {
                    //Select next card
                    eventSystem.SetSelectedGameObject(row1[cardIndex + 1]);
                }
            }
            else
            {
                //get the index of card
                int cardIndex = row2.IndexOf(eventData.selectedObject);

                if (cardIndex == row2.Count)
                {
                    //play wall sound
                }
                else
                {
                    //Select next card
                    eventSystem.SetSelectedGameObject(row2[cardIndex + 1]);
                }
            }
        } else if (eventData.moveDir == MoveDirection.Left)
        {
            if (row1.Contains(eventData.selectedObject))
            {
                //get the index of card
                int cardIndex = row1.IndexOf(eventData.selectedObject);

                if (cardIndex == row1.Count)
                {
                    //play wall sound
                }
                else
                {
                    //Select next card
                    eventSystem.SetSelectedGameObject(row1[cardIndex - 1]);
                }
            }
            else
            {
                //get the index of card
                int cardIndex = row2.IndexOf(eventData.selectedObject);

                if (cardIndex == row2.Count)
                {
                    //play wall sound
                }
                else
                {
                    //Select next card
                    eventSystem.SetSelectedGameObject(row2[cardIndex - 1]);
                }
            }
        }
    }*/
}
