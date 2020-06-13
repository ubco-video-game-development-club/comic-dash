using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD instance;

    public Text levelText;
    public Text scoreText;
    public Text timeText;
    public Text livesText;

    void Awake() {
        // Set up the HUD as a singleton
        if (instance != null) {
            Destroy(gameObject);
            return;
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SetLevel(int level) {
        levelText.text = "LEVEL\n" + level;
    }

    public void SetScore(int score) {
        scoreText.text = "SCORE\n" + score;
    }

    public void SetTime(int time) {
        timeText.text = "TIME\n" + time;
    }

    public void SetLives(int lives) {
        livesText.text = "LIVES\n" + lives;
    }
}
