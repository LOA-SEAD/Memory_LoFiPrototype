using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BoardInputHandler : MonoBehaviour {

    public List<GameObject> row1;
    public List<GameObject> row2;
    public List<GameObject> row3;
    private EventSystem eventSystem;
    private MemoryPairing memoryPairing;

    // Use this for initialization
    void Start () {
        eventSystem = GetComponent<EventSystem>();
        memoryPairing = GetComponent<MemoryPairing>();
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
        } else if (Input.GetKeyDown(KeyCode.W)) {
            Access(0, 1);
        } else if (Input.GetKeyDown(KeyCode.E)) {
            Access(0, 2);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            Access(0, 3);
        }
        else if (Input.GetKeyDown(KeyCode.A)) {
            Access(1, 0);
        } else if (Input.GetKeyDown(KeyCode.S)) {
            Access(1, 1);
        } else if (Input.GetKeyDown(KeyCode.D)) {
            Access(1, 2);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            Access(1, 3);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            Access(2, 0);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            Access(2, 1);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            Access(2, 2);
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            Access(2, 3);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }

    private void Submit() {
        eventSystem.currentSelectedGameObject.GetComponent<Card>().TrySubmit();
    }

    private void MoveDown()
    {
        if (eventSystem.currentSelectedGameObject != null)
        {
            if (row1.Contains(eventSystem.currentSelectedGameObject))
            {
                //select Next card
                eventSystem.SetSelectedGameObject(row2[row1.IndexOf(eventSystem.currentSelectedGameObject)]);
            }
            else
            {
                if (row3.Count == 0)
                {
                    //play wall sound
                    eventSystem.currentSelectedGameObject.GetComponent<Card>().PlayWallSound();
                }
                else
                {
                    if (row2.Contains(eventSystem.currentSelectedGameObject)) //selected in row2
                    {
                        //select next card in row 3
                        eventSystem.SetSelectedGameObject(row3[row2.IndexOf(eventSystem.currentSelectedGameObject)]);
                    }
                    else
                    {
                        //play wall sound
                        eventSystem.currentSelectedGameObject.GetComponent<Card>().PlayWallSound();
                    }
                }
            }
        } else
        {
            eventSystem.SetSelectedGameObject(row1[0]);
        }
    }

    private void MoveUp()
    {
        if (eventSystem.currentSelectedGameObject != null)
        {
            if (row2.Contains(eventSystem.currentSelectedGameObject))
            {
                //select next card
                eventSystem.SetSelectedGameObject(row1[row2.IndexOf(eventSystem.currentSelectedGameObject)]);
            }
            else
            {
                if (row3.Contains(eventSystem.currentSelectedGameObject))
                {
                    //select next card
                    eventSystem.SetSelectedGameObject(row2[row3.IndexOf(eventSystem.currentSelectedGameObject)]);
                }
                else
                {
                    //play wall sound
                    eventSystem.currentSelectedGameObject.GetComponent<Card>().PlayWallSound();
                }
            }
        }
        else
        {
            eventSystem.SetSelectedGameObject(row1[0]);
        }
    }

    private void MoveLeft()
    {
            if (eventSystem.currentSelectedGameObject != null)
            {
                if (row1.Contains(eventSystem.currentSelectedGameObject))
                {
                    //get the index of card
                    int cardIndex = row1.IndexOf(eventSystem.currentSelectedGameObject);

                    if (cardIndex == 0)
                    {
                        //play wall sound
                        eventSystem.currentSelectedGameObject.GetComponent<Card>().PlayWallSound();
                    }
                    else
                    {
                        //Select next card
                        eventSystem.SetSelectedGameObject(row1[cardIndex - 1]);
                    }
                }
                else if (row2.Contains(eventSystem.currentSelectedGameObject))
                {
                    //get the index of card
                    int cardIndex = row2.IndexOf(eventSystem.currentSelectedGameObject);

                    if (cardIndex == 0)
                    {
                        //play wall sound
                        eventSystem.currentSelectedGameObject.GetComponent<Card>().PlayWallSound();
                    }
                    else
                    {
                        //Select next card
                        eventSystem.SetSelectedGameObject(row2[cardIndex - 1]);
                    }
                }
                else
                {
                    //get the index of card
                    int cardIndex = row3.IndexOf(eventSystem.currentSelectedGameObject);

                    if (cardIndex == 0)
                    {
                        //play wall sound
                        eventSystem.currentSelectedGameObject.GetComponent<Card>().PlayWallSound();
                    }
                    else
                    {
                        //Select next card
                        eventSystem.SetSelectedGameObject(row3[cardIndex - 1]);
                    }
                }
            }
            else
            {
                eventSystem.SetSelectedGameObject(row1[0]);
            }
    }

    private void MoveRight()
    {
        if (eventSystem.currentSelectedGameObject != null)
        {
            if (row1.Contains(eventSystem.currentSelectedGameObject))
            {
                //get the index of card
                int cardIndex = row1.IndexOf(eventSystem.currentSelectedGameObject);

                if (cardIndex == row1.Count - 1)
                {
                    //play wall sound
                    eventSystem.currentSelectedGameObject.GetComponent<Card>().PlayWallSound();
                }
                else
                {
                    //Select next card
                    eventSystem.SetSelectedGameObject(row1[cardIndex + 1]);
                }
            }
            else if (row2.Contains(eventSystem.currentSelectedGameObject))
            {
                //get the index of card
                int cardIndex = row2.IndexOf(eventSystem.currentSelectedGameObject);

                if (cardIndex == row2.Count - 1)
                {
                    //play wall sound
                    eventSystem.currentSelectedGameObject.GetComponent<Card>().PlayWallSound();
                }
                else
                {
                    //Select next card
                    eventSystem.SetSelectedGameObject(row2[cardIndex + 1]);
                }
            }
            else if (row3.Contains(eventSystem.currentSelectedGameObject))
            {
                //get the index of card
                int cardIndex = row3.IndexOf(eventSystem.currentSelectedGameObject);

                if (cardIndex == row3.Count - 1)
                {
                    //play wall sound
                    eventSystem.currentSelectedGameObject.GetComponent<Card>().PlayWallSound();
                }
                else
                {
                    //Select next card
                    eventSystem.SetSelectedGameObject(row3[cardIndex + 1]);
                }
            }
        }
        else
        {
            eventSystem.SetSelectedGameObject(row1[0]);
        }
    }

    private void Access(int v1, int v2)
    {
        switch (v1)
        {
            case 0:
                if (v2 < row1.Count)
                {
                    eventSystem.SetSelectedGameObject(row1[v2]);
                }
                break;

            case 1:
                if (v2 < row2.Count)
                {
                    eventSystem.SetSelectedGameObject(row2[v2]);
                }
                break;

            case 2:
                if (v2 < row3.Count)
                {
                    eventSystem.SetSelectedGameObject(row3[v2]);
                }
                break;
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
