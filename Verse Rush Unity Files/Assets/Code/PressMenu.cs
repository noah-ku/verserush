using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PressMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPage, gamePage, optionsPage, currentVerseObject, theTimeObject, numPlayersObject;
    private TextMeshProUGUI currentVerseText, theTimeText, numPlayersText;
    public static PressMenu menuInst;
    private void Awake() {
        menuInst = this;
    }
    private void Start() {
        Screen.SetResolution(1920, 1080, true);
        ReturnToMain();

        currentVerseText = currentVerseObject.GetComponent<TextMeshProUGUI>();
        theTimeText = theTimeObject.GetComponent<TextMeshProUGUI>();
        numPlayersText = numPlayersObject.GetComponent<TextMeshProUGUI>();
    }
    public void Play() {
        BlankGenerator.isStarting = true;
        gamePage.SetActive(true);
        mainMenuPage.SetActive(false);
        BlankGenerator.inputBoxText.ActivateInputField();

        BlankGenerator.BeginGame();

        AudioManager.PlaySound("tick");
    }

    public void Options() {
        optionsPage.SetActive(true);
        mainMenuPage.SetActive(false);
    }

    public static void ReturnToMain() {
        menuInst.gamePage.SetActive(false);
        menuInst.optionsPage.SetActive(false);
        menuInst.mainMenuPage.SetActive(true);
    }

    public void IncrementVerse() {
        if (BlankGenerator.currentVerse + 1 == BlankGenerator.numVerses) {
            BlankGenerator.currentVerse = 0;
        } else {
            BlankGenerator.currentVerse++;
        }
        currentVerseText.text = BlankGenerator.currentVerse + "";
    }

    public void DecrementVerse() {
        if (BlankGenerator.currentVerse - 1 < 0) {
            BlankGenerator.currentVerse = BlankGenerator.numVerses - 1;
        } else {
            BlankGenerator.currentVerse--;
        }
        currentVerseText.text = BlankGenerator.currentVerse + "";
    }

    public void DecrementTime() {
        if (BlankGenerator.theTime - 5.0f == 0.0f) {
            BlankGenerator.theTime = 30.0f;
        } else {
            BlankGenerator.theTime -= 5.0f;
        }
        BlankGenerator.savedTime = BlankGenerator.theTime;
        theTimeText.text = (int) BlankGenerator.theTime + "";
    }

    public void IncrementTime() {
        if (BlankGenerator.theTime + 5.0f == 35.0f) {
            BlankGenerator.theTime = 5.0f;
        } else {
            BlankGenerator.theTime += 5.0f;
        }
        BlankGenerator.savedTime = BlankGenerator.theTime;
        theTimeText.text = (int) BlankGenerator.theTime + "";
    }

    public void IncrementPlayers() {
        if (BlankGenerator.numPlayers + 1 > 20) {
            BlankGenerator.numPlayers = 2;
        } else {
            BlankGenerator.numPlayers++;
        }
        BlankGenerator.savedNumPlayers = BlankGenerator.numPlayers;
        numPlayersText.text = BlankGenerator.numPlayers + "";
    }

    public void DecrementPlayers() {
        if (BlankGenerator.numPlayers - 1 < 2) {
            BlankGenerator.numPlayers = 20;
        } else {
            BlankGenerator.numPlayers--;
        }
        BlankGenerator.savedNumPlayers = BlankGenerator.numPlayers;
        numPlayersText.text = BlankGenerator.numPlayers + "";
    }

    public void Quit() {
        Application.Quit();
    }
}
