using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BlankGenerator : MonoBehaviour {
    public string verse = "";
    public readonly string verse0 = "In the beginning, God created the heavens and the earth.";
    public readonly string verse1 = "Now may the God of peace Himself sanctify you completely; and may your whole spirit, soul, and body be preserved blameless at the coming of our Lord Jesus Christ.";
    public readonly string verse2 = "As you therefore have received Christ Jesus the Lord, so walk in Him, rooted and built up in Him and established in the faith, as you have been taught, abounding in it with thanksgiving.";
    public readonly string verse3 = "Knowing that a man is not justified by the works of the law but by faith in Jesus Christ, even we have believed in Christ Jesus, that we might be justified by faith in Christ and not by the works of the law; for by the works of the law no flesh shall be justified.";
    public static int currentVerse = 0, numVerses = 4;
    public List<Words> wordObjects = new List<Words>();
    public string display = "";
    private List<int> randomInts = new List<int>();
    private static System.Random rng = new System.Random();
    int removedWordsCount = 0;
    public static int savedBlankCount = 5;
    int bottom = 0, top = savedBlankCount;
    private bool isFinished = false, onLastSection = false;
    private int round = 1, currentPlayer = 1;
    public static int numPlayers = 3, savedNumPlayers = 3;
    [SerializeField] private GameObject verseDisplay, inputBox, playerCount, boxText;
    private TextMeshProUGUI verseDisplayText, playerCountText;
    public static TMP_InputField inputBoxText, optionsInputBoxText;
    public static float theTime = 15.0f, savedTime = 15.0f;
    private float ticking = 1.0f;
    public static bool isStarting = false;
    public static BlankGenerator inst;
    
    private void Start() {
        verseDisplayText = verseDisplay.GetComponent<TextMeshProUGUI>();
        inputBoxText = inputBox.GetComponent<TMP_InputField>();
        playerCountText = playerCount.GetComponent<TextMeshProUGUI>();
        optionsInputBoxText = boxText.GetComponent<TMP_InputField>();

        inputBoxText.ActivateInputField();
    }

    private void Awake() {
        inst = this;
    }

    public static void BeginGame() {
        inst.inputBox.SetActive(true);

        if (optionsInputBoxText.text != "") {
            inst.verse = optionsInputBoxText.text;
        } else {
            if (currentVerse == 0) {
                inst.verse = inst.verse0;
            } else if (currentVerse == 1) {
                inst.verse = inst.verse1;
            } else if (currentVerse == 2) {
                inst.verse = inst.verse2;
            } else if (currentVerse == 3) {
                inst.verse = inst.verse3;
            } else {
                Debug.LogWarning("SOMETHING IS WRONG");
            }
        }

        string[] splitVerse = inst.verse.Split(' ');
        for (int i = 0; i < splitVerse.Length; i++) {
            inst.wordObjects.Add(new Words(splitVerse[i], inst.toBlank(splitVerse[i]), i, false));
        }

        //Creates an array of random ints
        inst.generateRandomInts();
        // generateBlanks();
        inst.verseDisplayText.text = inst.verse;
    }

    void Update() {
        if (isStarting) {
            if (theTime > 0) {
                theTime -= Time.deltaTime;
            } else {
                generateBlanks();
                verseDisplayText.text = display;
                isStarting = false;
            }

            if (ticking > 0) {
                ticking -= Time.deltaTime;
            } else {
                AudioManager.PlaySound("tick");
                ticking = 1.0f;
            }
        }


        if (Input.GetButtonDown("Submit")) {
            if (inputBoxText.text == "" || inputBoxText.text == " ") {
                inputBoxText.text = "";
                return;
            }

            Debug.Log(" ".Equals(inputBoxText.text.Substring(inputBoxText.text.Length - 1)));
            if (" ".Equals(inputBoxText.text.Substring(inputBoxText.text.Length - 1))) {
                inputBoxText.text = inputBoxText.text.Substring(0, inputBoxText.text.Length - 1);
            }
            scanWithInput();

            inputBoxText.text = "";
            if (!isFinished) {
                inputBoxText.ActivateInputField();
            }

            displayString();

            currentPlayer %= numPlayers;
            currentPlayer++;
            playerCountText.text = "PLAYER " + currentPlayer;
        }
    }

    private void scanWithInput() {
        //Given an input string, the program will scan
        //all words from the verse to see if it matches
        int foundID = -1;

        for (int i = 0; i < verse.Split(' ').Length; i++) {
            string compareString = wordObjects[i].getWord();
            bool hasPunctuation = !Char.IsLetter(compareString.Substring(compareString.Length - 1).ToCharArray()[0]);
            if (hasPunctuation) {
                compareString = compareString.Substring(0, compareString.Length - 1);
            }

            if (compareString.Equals(inputBoxText.text, StringComparison.InvariantCultureIgnoreCase) && wordObjects[i].isBlankBool()) {
                //If it found a match
                wordObjects[i].changeBlankBool(false);
                foundMatch();
                foundID = wordObjects[i].getID();
                return;
            }
        }

        Debug.Log("NONE MATCHED");
        AudioManager.PlaySound("wrongAnswer");
    }

    private void foundMatch() {
        if (verseRoundIsCompleted()) {
            if (!onLastSection) {
                Debug.Log("ROUND FINISHED");
                AudioManager.PlaySound("levelUp");
            }
            roundFinished();
        } else {
            Debug.Log("KEEP GOING");
            AudioManager.PlaySound("rightAnswer");

            displayString();
            verseDisplayText.text = display;
        }
    }

    private bool verseRoundIsCompleted() {
        for (int i = 0; i < verse.Split(' ').Length; i++) {
            if (wordObjects[i].isBlankBool()) {
                return false;
            }
        }
        return true;
    }

    private void roundFinished() {
        if (!onLastSection) {
            generateBlanks();
            verseDisplayText.text = display;
        } else {
            isFinished = true;
            Debug.Log("YOU FINISHED EVERYTHING");
            inputBox.SetActive(false);
            AudioManager.PlaySound("finished");
            verseDisplayText.text = verse;
        }
    }

    private void generateRandomInts() {
        for (int i = 0; i < verse.Split(' ').Length; i++) {
            randomInts.Add(i);
        }
        Shuffle(randomInts);
    }

    private void generateBlanks() {
        //if (round != 1) {
            try {
                for (int i = bottom; i < top; i++) {
                    removedWordsCount++;
                    wordObjects[randomInts[i]].changeBlankBool(true);
                }
            } catch {
                Debug.Log("You'll need to do the verse again");
            }

            if ((top + savedBlankCount) > wordObjects.Count) {
                if (verse.Split(' ').Length % savedBlankCount != 0 && top != wordObjects.Count) {
                    //bottom += blankCount;
                    top = wordObjects.Count;
                } else {
                    onLastSection = true;
                }
            } else {
                //bottom += blankCount;
                top += savedBlankCount;
            }
        //}

        displayString();
    }

    private void displayString() {
        display = "";
        for (int i = 0; i < wordObjects.Count; i++) {
            display += wordObjects[i].isBlankBool() ? wordObjects[i].getBlank() : wordObjects[i].getWord();
            if (i != wordObjects.Count - 1) {
                display += " ";
            }
        }
        round++;
    }

    private string toBlank(string input) {
        string returnString = "";
        int loopLength = input.Length;
        bool hasPunctuation = !Char.IsLetter(input.Substring(input.Length - 1).ToCharArray()[0]);

        //Is the last letter an alphabet?
        if (hasPunctuation) {
            loopLength--;
        }

        //Adds in blanks
        for (int i = 0; i < loopLength; i++) {
            returnString += "_";
        }

        //Adds in that last punctuation if needed
        if (hasPunctuation) {
            returnString += input.Substring(input.Length - 1);
        }

        return returnString;
    }

    private List<int> Shuffle(List<int> inputList) {
        List<int> list = inputList;
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            int value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }
        return list;
    }

    public static void Reset() {
        inst.wordObjects = new List<Words>();
        inst.display = "";
        inst.randomInts = new List<int>();
        inst.bottom = 0;
        inst.top = 5;
        savedBlankCount = 5;
        inst.removedWordsCount = 0;
        inst.isFinished = false;
        inst.onLastSection = false;
        inst.round = 1;
        inst.currentPlayer = 1;
        theTime = savedTime;
        numPlayers = savedNumPlayers;
        inst.ticking = 1.0f;
        isStarting = false;

        PressMenu.ReturnToMain();
    }
}
