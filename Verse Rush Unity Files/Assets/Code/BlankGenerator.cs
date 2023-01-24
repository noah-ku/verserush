using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BlankGenerator : MonoBehaviour {
   public string verse = "";
  
   // Example verses to use
   private readonly string verse0 = "In the beginning, God created the heavens and the earth. (Genesis 1:1)";
   private readonly string verse1 = "In the beginning was the Word, and the Word was with God, and the Word was God. (John 1:1)";
   private readonly string verse2 = "And the Word became flesh and dwelt among us, and we beheld His glory, the glory as of the only begotten of the Father, full of grace and truth. (John 1:14)";
   private readonly string verse3 = "God is Spirit, and those who worship Him must worship in spirit and truth. (John 4:24)";
   private readonly string verse4 = "For they heard them speak with tongues and magnify God. Then Peter answered, \"Can anyone forbid water, that these should not be baptized who have received the Holy Spirit just as we have?\" (Acts 10:46-47)";

   public static int currentVerse = 0, numVerses = 5;
   public List<Words> wordObjects = new List<Words>();
   public string display = "";
   private List<int> randomInts = new List<int>();
   private static readonly System.Random rng = new System.Random();
   private static int savedBlankCount = 5;
   private int bottom, top = savedBlankCount;
   private bool isFinished, onLastSection;
   private int currentPlayer = 1;
   public static int numPlayers = 3, savedNumPlayers = 3;
   [SerializeField] private GameObject verseDisplay, inputBox, playerCount, boxText, progressBar;
   private TextMeshProUGUI verseDisplayText, playerCountText;
   public static TMP_InputField inputBoxText, optionsInputBoxText;
   public static float theTime = 15.0f, savedTime = 15.0f;
   private float ticking = 1.0f;
   public static bool isStarting, forceAllBlanks = false;
   private static BlankGenerator inst;
   private static Slider progress;

   private void Start() {
       verseDisplayText = verseDisplay.GetComponent<TextMeshProUGUI>();
       inputBoxText = inputBox.GetComponent<TMP_InputField>();
       playerCountText = playerCount.GetComponent<TextMeshProUGUI>();
       optionsInputBoxText = boxText.GetComponent<TMP_InputField>();
       progress = progressBar.GetComponent<Slider>();

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
           switch (currentVerse) {
               case 0:
                   inst.verse = inst.verse0;
                   break;
               case 1:
                   inst.verse = inst.verse1;
                   break;
               case 2:
                   inst.verse = inst.verse2;
                   break;
               case 3:
                   inst.verse = inst.verse3;
                   break;
               case 4:
                   inst.verse = inst.verse4;
                   break;
               default:
                   throw new InvalidOperationException("Change the min and max for numVerses");
           }
       }

       // Use Regex to get rid of invisible characters
       inst.verse = Regex.Replace(inst.verse, @"\p{C}+/u", " ");
       // Use Regex to get rid of long dashes
       inst.verse = Regex.Replace(inst.verse, @"[\u2012\u2013\u2014\u2015]", "-");
       // Use Regex to get rid of double spaces
       inst.verse = Regex.Replace(inst.verse, @"\s+", " ");
       // Remove trailing and leading spaces
       inst.verse = inst.verse.Trim(' ');

       string[] splitVerse = inst.verse.Split(' ');
       for (int i = 0; i < splitVerse.Length; i++) {
           inst.wordObjects.Add(new Words(splitVerse[i], inst.toBlank(splitVerse[i]), i, false));
       }

       progress.value = 0;
       int closestMod = splitVerse.Length % 5 == 0 && splitVerse.Length >= 5 ? splitVerse.Length - 5 : splitVerse.Length - splitVerse.Length % 5;
      
       // If force all blanks, change progress bar to just length
       progress.maxValue = !forceAllBlanks ? (float) (0.1 * (closestMod * closestMod) + 0.5 * closestMod + splitVerse.Length) : splitVerse.Length;
      

       // Creates an array of random ints
       inst.generateRandomInts();
       inst.verseDisplayText.text = inst.verse;
   }

   void Update() {
       if (isStarting) {
           if (theTime > 0) {
               theTime -= Time.deltaTime;
           } else {
               generateBlanks();
               verseDisplayText.text = display;
               inputBoxText.ActivateInputField();
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
       /* Given an input string, the program will scan
       all words from the verse to see if it matches */

       for (int i = 0; i < verse.Split(' ').Length; i++) {
           string compareString = wordObjects[i].getWord();

           int lowerBound = 0;
           int upperBound = compareString.Length - 1;

           while (!Char.IsLetterOrDigit(compareString.Substring(lowerBound, 1).ToCharArray()[0])) {
               lowerBound++;
           }

           while (!Char.IsLetterOrDigit(compareString.Substring(upperBound, 1).ToCharArray()[0])) {
               upperBound--;
           }

           compareString = compareString.Substring(lowerBound, upperBound + 1 - lowerBound);

           if (compareString.Equals(inputBoxText.text, StringComparison.InvariantCultureIgnoreCase) && wordObjects[i].isBlankBool()) {
               //If it found a match
               wordObjects[i].changeBlankBool(false);
               foundMatch();
               return;
           }
       }
      
       AudioManager.PlaySound("wrongAnswer");
   }

   private void foundMatch() {
       progress.value++;
      
       if (verseRoundIsCompleted()) {
           if (!onLastSection) {
               AudioManager.PlaySound("levelUp");
           }
           roundFinished();
       } else {
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
       if (forceAllBlanks) {
           top = wordObjects.Count;
       }
      
       try {
           for (int i = bottom; i < top; i++) {
               wordObjects[randomInts[i]].changeBlankBool(true);
           }
       } catch {
           Debug.LogError("You'll need to do the verse again");
       }

       if (top + savedBlankCount > wordObjects.Count) {
           if ((verse.Split(' ').Length % savedBlankCount != 0 && top != wordObjects.Count) && wordObjects.Count > savedBlankCount) {
               top = wordObjects.Count;
           } else {
               onLastSection = true;
           }
       } else {
           top += savedBlankCount;
       }

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
   }

   private string toBlank(string input) {
       string returnString = "";
       int loopLength = input.Length;

       int lowerBound = 0;
       int upperBound = input.Length - 1;

       while (!Char.IsLetterOrDigit(input.Substring(lowerBound, 1).ToCharArray()[0])) {
           lowerBound++;
       }

       while (!Char.IsLetterOrDigit(input.Substring(upperBound, 1).ToCharArray()[0])) {
           upperBound--;
       }

       // Adds in the blanks
       for (int i = 0; i < loopLength; i++) {
           if (i < lowerBound || i > upperBound) {
               returnString += input.Substring(i, 1);
           } else {
               returnString += "_";
           }
       }

       return returnString;
   }

   private static void Shuffle(List<int> inputList) {
       var n = inputList.Count; 
       while (n > 1) { 
           n--; 
           var k = rng.Next(n + 1);
           (inputList[k], inputList[n]) = (inputList[n], inputList[k]);
       }
   }

   public void Reset() {
       inst.wordObjects = new List<Words>();
       inst.display = "";
       inst.randomInts = new List<int>();
       inst.bottom = 0;
       inst.top = 5;
       savedBlankCount = 5;
       inst.isFinished = false;
       inst.onLastSection = false;
       inst.currentPlayer = 1;
       theTime = savedTime;
       numPlayers = savedNumPlayers;
       inst.ticking = 1.0f;
       isStarting = false;

       PressMenu.ReturnToMain();
   }
}

