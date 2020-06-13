using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public string startSceneName = "Level1";
    public int startTime = 120;
    public int totalLives = 3;

    private int level;
    private int score;
    private int time;
    private int lives;

    void Awake() {
        // Set up the GameController as a singleton
        if (instance != null) {
            Destroy(gameObject);
            return;
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start() {
        SetLives(totalLives);
        SetTime(startTime);
        StartCoroutine(LevelTimer());
    }

    public void RestartGame() {
        SceneManager.LoadScene(startSceneName);
        SetLives(totalLives);
        SetTime(startTime);
    }

    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        SetTime(startTime);
    }

    public void AddScore(int score) {
        SetScore(this.score + score);
    }

    public void SetLevel(int level) {
        this.level = level;
        HUD.instance.SetLevel(this.level);
    }

    public void SetScore(int score) {
        this.score = score;
        HUD.instance.SetLives(this.score);
    }

    public void SetTime(int time) {
        this.time = time;
        HUD.instance.SetTime(this.time);
    }

    public void SetLives(int lives) {
        this.lives = lives;
        HUD.instance.SetLives(this.lives);
    }

    public void Die() {
        SetLives(lives - 1);
        if (lives <= 0) {
            RestartGame();
        } else {
            RestartLevel();
        }
    }

    private IEnumerator LevelTimer() {
        while (time > 0) {
            yield return new WaitForSeconds(1);
            SetTime(time - 1);
        }
        Die();
    }
}
