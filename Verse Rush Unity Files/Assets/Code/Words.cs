using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Words {
    string word, blank;
    bool isBlank;
    int assignedID;

    public Words(string word, string blank, int assignedID, bool isBlank) {
        this.word = word;
        this.blank = blank;
        this.assignedID = assignedID;
        this.isBlank = isBlank;
    }

    public void changeBlankBool(bool newBlankBool) {
        isBlank = newBlankBool;
    }

    public string getWord() {
        return word;
    }

    public string getBlank() {
        return blank;
    }

    public int getID() {
        return assignedID;
    }

    public bool isBlankBool() {
        return isBlank;
    }
}
