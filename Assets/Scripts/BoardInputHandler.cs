using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BoardInputHandler : MonoBehaviour {

    enum possibleKeyDown {Down, Up, Left, Right};

    public GameObject menuPanel;
    public String filename;
    public int rowCount;
    public int currRow;
    public int currColumn;
    public List<GameObject> row1;
    public List<GameObject> row2;
    public List<GameObject> row3;
    public List<GameObject> row4;
    private EventSystem eventSystem;
    private MemoryPairing memoryPairing;
    //To fix the problem of deselected a card
    private GameObject lastselect;
    private possibleKeyDown lastKeyDown;
    //Location audios
    public AudioClip[] location = new AudioClip[16];
    private int lineCard;
    private int cardIndex;
    private int contador;
    private List<CardClass> cardsData = new List<CardClass>();
    private System.Random rng = new System.Random();

    private void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    // Use this for initialization
    void Start () {
        eventSystem = GetComponent<EventSystem>();
        memoryPairing = GetComponent<MemoryPairing>();
        //lastselect = new GameObject();
        String filepath = Application.streamingAssetsPath + "/" + filename;

        Debug.Log("Reading cards " + filepath);

        using (StreamReader sr = new StreamReader(filepath)) 
        {
            while (sr.Peek() >= 0) 
            {
                CardPairClass cpc = JsonUtility.FromJson<CardPairClass>(sr.ReadLine());
                CardClass cc1 = new CardClass();
                CardClass cc2 = new CardClass();
                cc1.cardText = cpc.textA;
                cc1.audioName = cpc.audioA;
                cc1.pairNumber = cpc.pairNumber;
                cc2.cardText = cpc.textB;
                cc2.audioName = cpc.audioB;
                cc2.pairNumber = cpc.pairNumber;
                cardsData.Add(cc1);
                cardsData.Add(cc2);
            }
        }

        Shuffle<CardClass>(cardsData);

        Debug.Log("Finished loading cards.");
        
        int k = 0;
        for (int i = 0; i < rowCount; i++) {
            for( int j = 0; j < row1.Count; j++)
            {
                currRow = i;
                currColumn = j;
                StartCoroutine("loadAudio", cardsData[k]);
                k++;
            }
        }
    }

    IEnumerator loadAudio(CardClass info) {
        Card cartaAtual = null;
        if (currRow == 0)
        {
            cartaAtual = row1[currColumn].GetComponent<Card>();
        } else if (currRow == 1)
        {
            cartaAtual = row2[currColumn].GetComponent<Card>();
        } else if (currRow == 2)
        {
            cartaAtual = row3[currColumn].GetComponent<Card>();
        } else
        {
            cartaAtual = row4[currColumn].GetComponent<Card>();
        }

        if (cartaAtual != null)
        {
            string wwwPlayerFilePath = "file://" + Application.streamingAssetsPath + "/" + info.audioName;
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(wwwPlayerFilePath, AudioType.WAV)) {
            yield return www.SendWebRequest();
            
            if (www.isNetworkError) {
            } else {
                cartaAtual.contentValue = DownloadHandlerAudioClip.GetContent(www);
            }
        
            cartaAtual.GetComponentInChildren<Card>().cardNumber = (CardNumber) info.pairNumber;
            cartaAtual.GetComponentInChildren<Text>().text = info.cardText;
        };
        }
    }

    // Update is called once per frame
    void Update () {

        if (!menuPanel.activeSelf) {
            //Store the selected card if something is selected
            if (eventSystem.currentSelectedGameObject != null)
            {
                lastselect = eventSystem.currentSelectedGameObject;
            }
            if(Input.GetKeyDown(KeyCode.RightArrow)) {  //Moving
                MoveRight();
            } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                MoveLeft();
            } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                MoveLeft();
            } else if(Input.GetKeyDown(KeyCode.UpArrow)) {
                MoveUp();
            } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                MoveDown();
            } else if (Input.GetMouseButtonDown(0)){ //To not bug the selection system
                eventSystem.SetSelectedGameObject(lastselect);
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
            else if (Input.GetKeyDown(KeyCode.J))
            {
                PlayerLocation();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            toggleMenu();
        }
    }

    public void toggleMenu() {
        memoryPairing.PlayBackToMenu();
        if (menuPanel.activeSelf) {
            //Lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            //Change cursor to invisible again
            Cursor.visible = false;
            menuPanel.SetActive(false);
            eventSystem.SetSelectedGameObject(lastselect);
        } else {
            //Release cursor
            Cursor.lockState = CursorLockMode.None;
            //Change cursor to visible again
            Cursor.visible = true;
            menuPanel.SetActive(true);
            lastselect = eventSystem.currentSelectedGameObject;
            eventSystem.SetSelectedGameObject(GameObject.Find("Continuar"));
        }

    }

    public void Submit() {
        if (eventSystem.currentSelectedGameObject == null)
        {
            //play wall sound
            lastselect.GetComponent<Card>().PlayWallSound();
        }
        else
        {
            eventSystem.currentSelectedGameObject.GetComponent<Card>().TrySubmit();
        }
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
                    //play wall sound and deselect the card
                    eventSystem.currentSelectedGameObject.GetComponent<Card>().PlayWallSound();
                    eventSystem.SetSelectedGameObject(null);
                    lastKeyDown = possibleKeyDown.Down;
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
                        if (row4.Count == 0)
                        {
                            //play wall sound and deselect the card
                            eventSystem.currentSelectedGameObject.GetComponent<Card>().PlayWallSound();
                            eventSystem.SetSelectedGameObject(null);
                            lastKeyDown = possibleKeyDown.Down;
                        }
                        else 
                        {
                            if (row3.Contains(eventSystem.currentSelectedGameObject)) //selected in row3
                            {
                                //select next card in row 4
                                eventSystem.SetSelectedGameObject(row4[row3.IndexOf(eventSystem.currentSelectedGameObject)]);
                            }
                            else
                            {
                                //play wall sound and deselect the card
                                eventSystem.currentSelectedGameObject.GetComponent<Card>().PlayWallSound();
                                eventSystem.SetSelectedGameObject(null);
                                lastKeyDown = possibleKeyDown.Down;
                            }
                        }
                    }
                }
            }
        } else
        {
            if (row1.Contains(lastselect) && (lastKeyDown == possibleKeyDown.Up))
            {
                //select last card
                eventSystem.SetSelectedGameObject(row1[row1.IndexOf(lastselect)]);
            }
            else
            {
                //play wall sound
                lastselect.GetComponent<Card>().PlayWallSound();
            }
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
            else if (row3.Contains(eventSystem.currentSelectedGameObject))
            {
                //select next card
                eventSystem.SetSelectedGameObject(row2[row3.IndexOf(eventSystem.currentSelectedGameObject)]);
            }
            else if (row4.Contains(eventSystem.currentSelectedGameObject))
            {
                //select next card
                eventSystem.SetSelectedGameObject(row3[row4.IndexOf(eventSystem.currentSelectedGameObject)]);
            } else
            {
                //play wall sound and deselect the card
                eventSystem.currentSelectedGameObject.GetComponent<Card>().PlayWallSound();
                eventSystem.SetSelectedGameObject(null);
                lastKeyDown = possibleKeyDown.Up;
            }
        }
        else
        {
            if (row3.Count == 0)
            {
                if (row2.Contains(lastselect) && (lastKeyDown == possibleKeyDown.Down))
                {
                    //select last card
                    eventSystem.SetSelectedGameObject(row2[row2.IndexOf(lastselect)]);
                }
                else
                {
                    //play wall sound
                    lastselect.GetComponent<Card>().PlayWallSound();
                }
            }
            else
            {
                if (row3.Contains(lastselect) && (lastKeyDown == possibleKeyDown.Down))
                {
                    //select last card
                    eventSystem.SetSelectedGameObject(row3[row3.IndexOf(lastselect)]);
                }
                else
                {
                    if (row4.Count == 0)
                    {
                        if (row3.Contains(lastselect) && (lastKeyDown == possibleKeyDown.Down))
                        {
                            //select last card
                            eventSystem.SetSelectedGameObject(row3[row3.IndexOf(lastselect)]);
                        }
                        else
                        {
                            //play wall sound
                            lastselect.GetComponent<Card>().PlayWallSound();
                        }
                    } else {
                        if (row4.Contains(lastselect) && (lastKeyDown == possibleKeyDown.Down))
                        {
                            //select last card
                            eventSystem.SetSelectedGameObject(row4[row4.IndexOf(lastselect)]);
                        }
                    }
                }
            }
        }
    }

    private void doMoveLeft(List<GameObject> row) 
    {
        //get the index of card
        int cardIndex = row.IndexOf(eventSystem.currentSelectedGameObject);

        if (cardIndex == 0)
        {
            //play wall sound and deselect the card
            eventSystem.currentSelectedGameObject.GetComponent<Card>().PlayWallSound();
            eventSystem.SetSelectedGameObject(null);
            lastKeyDown = possibleKeyDown.Left;
        }
        else
        {
            //Select next card
            eventSystem.SetSelectedGameObject(row[cardIndex - 1]);
        }
    }
    private void MoveLeft()
    {
            // Player is inside grid
            if (eventSystem.currentSelectedGameObject != null)
            {
                if (row1.Contains(eventSystem.currentSelectedGameObject))
                {
                    doMoveLeft(row1);
                }
                else if (row2.Contains(eventSystem.currentSelectedGameObject))
                {
                    doMoveLeft(row2);
                }
                else if (row3.Contains(eventSystem.currentSelectedGameObject))
                {
                    doMoveLeft(row3);
                } else
                {
                    doMoveLeft(row4);
                }
            }
            // Player is on a border cell
            else
            {
                if (row1.Contains(lastselect))
                {
                    moveLeftFromBorder(row1);
                }
                else if (row2.Contains(lastselect))
                {
                    moveLeftFromBorder(row2);
                }
                else if (row3.Contains(lastselect))
                {
                    moveLeftFromBorder(row3);
                }
                else
                {
                    moveLeftFromBorder(row4);
                }
            }
    }

    private void moveLeftFromBorder(List<GameObject> row) {
        //get the index of card
        int cardIndex = row.IndexOf(lastselect);

        if ((cardIndex != row.Count - 1) || (lastKeyDown != possibleKeyDown.Right))
        {
            //play wall sound
            lastselect.GetComponent<Card>().PlayWallSound();
        }
        else
        {
            //Select next card
            eventSystem.SetSelectedGameObject(row[cardIndex]);
        }
    }

    private void doMoveRight(List<GameObject> row)
    {
        //get the index of card
        int cardIndex = row.IndexOf(eventSystem.currentSelectedGameObject);

        if (cardIndex == row.Count - 1)
        {
            //play wall sound and deselect the card
            eventSystem.currentSelectedGameObject.GetComponent<Card>().PlayWallSound();
            eventSystem.SetSelectedGameObject(null);
            lastKeyDown = possibleKeyDown.Right;
        }
        else
        {
            //Select next card
            eventSystem.SetSelectedGameObject(row[cardIndex + 1]);
        }
    }

    private void unborderFromRight(List<GameObject> row)
    {
        //get the index of card
        int cardIndex = row.IndexOf(lastselect);

        if ((cardIndex != 0) || (lastKeyDown != possibleKeyDown.Left))
        {
            //play wall sound
            lastselect.GetComponent<Card>().PlayWallSound();
        }
        else
        {
            //Select next card
            eventSystem.SetSelectedGameObject(row[cardIndex]);
        }
    }
    private void MoveRight()
    {
        if (eventSystem.currentSelectedGameObject != null)
        {
            if (row1.Contains(eventSystem.currentSelectedGameObject))
            {
                doMoveRight(row1);
            }
            else if (row2.Contains(eventSystem.currentSelectedGameObject))
            {
                doMoveRight(row2);
            }
            else if (row3.Contains(eventSystem.currentSelectedGameObject))
            {
                doMoveRight(row3);
            } else 
            {
                doMoveRight(row4);
            }
        }
        else
        {
            if (row1.Contains(lastselect))
            {
                unborderFromRight(row1);
            }
            else if (row2.Contains(lastselect))
            {
                unborderFromRight(row2);
            }
            else if (row3.Contains(lastselect))
            {
                unborderFromRight(row3);
            } else 
            {
                unborderFromRight(row4);
            }
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
            case 3:
                if (v2 < row4.Count)
                {
                    eventSystem.SetSelectedGameObject(row4[v2]);
                }
                break;
        }
    }

    private void PlayerLocation()
    {
        //Inform the player's location only if the game is active, not during the initial orientations or in the end 
        if (MemoryPairing.gameActive)
        {
            if (eventSystem.currentSelectedGameObject == null)
            {
                //Wall Sound if player is in the wall
                lastselect.GetComponent<Card>().PlayWallSound();
            }
            else
            {
                /*
                //OLD METHOD
                //Get the line of the card and get the index column of card
                if (row1.Contains(eventSystem.currentSelectedGameObject))
                {
                    memoryPairing.PlayLineCard(linha1);
                    cardIndex = row1.IndexOf(eventSystem.currentSelectedGameObject);
                }
                else if (row2.Contains(eventSystem.currentSelectedGameObject))
                {
                    memoryPairing.PlayLineCard(linha2);
                    cardIndex = row2.IndexOf(eventSystem.currentSelectedGameObject);
                }
                else
                {
                    memoryPairing.PlayLineCard(linha3);
                    cardIndex = row3.IndexOf(eventSystem.currentSelectedGameObject);
                }
                //Get the column of the card
                if (cardIndex == 0)
                {
                    StartCoroutine(memoryPairing.PlayColumnCard(coluna1));
                }
                else if (cardIndex == 1)
                {
                    StartCoroutine(memoryPairing.PlayColumnCard(coluna2));
                }
                else if (cardIndex == 2)
                {
                    StartCoroutine(memoryPairing.PlayColumnCard(coluna3));
                }
                else
                {
                    StartCoroutine(memoryPairing.PlayColumnCard(coluna4));
                }
                */
                if (row1.Contains(eventSystem.currentSelectedGameObject))
                {
                    lineCard = 0;
                    cardIndex = row1.IndexOf(eventSystem.currentSelectedGameObject);
                }
                else if (row2.Contains(eventSystem.currentSelectedGameObject))
                {
                    lineCard = 1;
                    cardIndex = row2.IndexOf(eventSystem.currentSelectedGameObject);
                }
                else if (row3.Contains(eventSystem.currentSelectedGameObject))
                {
                    lineCard = 2;
                    cardIndex = row3.IndexOf(eventSystem.currentSelectedGameObject);
                } else
                {
                    lineCard = 3;
                    cardIndex = row4.IndexOf(eventSystem.currentSelectedGameObject);
                }
                //Play the location
                memoryPairing.PlayLineCard(location[lineCard * 4 + cardIndex]);
            }    
        }
    }

    public void leaveGame() {
        SceneManager.LoadScene(0);
    }

}
