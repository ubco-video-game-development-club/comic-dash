using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public string startSceneName = "Level1";
    public int totalLives = 3;

    private int remainingLives;

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
        remainingLives = totalLives;
    }

    public void RestartGame() {
        SceneManager.LoadScene(startSceneName);
    }

    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Die() {
        remainingLives--;
        if (remainingLives <= 0) {
            RestartGame();
        } else {
            RestartLevel();
        }
    }
}
