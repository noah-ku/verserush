using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioClip rightAnswer, wrongAnswer, levelUp, finished, tick;
    public static AudioSource audioSrc;
    private void Start() {
        rightAnswer = Resources.Load<AudioClip>("rightAnswer");
        wrongAnswer = Resources.Load<AudioClip>("wrongAnswer");
        levelUp = Resources.Load<AudioClip>("levelUp");
        finished = Resources.Load<AudioClip>("finished");
        tick = Resources.Load<AudioClip>("tick");

        audioSrc = GetComponent<AudioSource>();
    }

    public static void PlaySound(string clip) {
        switch (clip) {
            case "rightAnswer":
                audioSrc.PlayOneShot(rightAnswer);
                break;
            case "wrongAnswer":
                audioSrc.PlayOneShot(wrongAnswer);
                break;
            case "levelUp":
                audioSrc.PlayOneShot(levelUp);
                break;
            case "finished":
                audioSrc.PlayOneShot(finished);
                break;
            case "tick":
                audioSrc.PlayOneShot(tick);
                break;
        }
    }
}
