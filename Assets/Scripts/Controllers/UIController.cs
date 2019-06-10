using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    [SerializeField] Text scoreText; 
    [SerializeField] GameObject gameOverPanel;
    public void GameOver() 
    {
        gameOverPanel.SetActive(true);
    }
    public void StartGame() 
    {
        gameOverPanel.SetActive(false);
        UpdateScore();
    }
    public void UpdateScore()
    {
        scoreText.text = string.Format($"Score: {GameController.Instance.Score}");
    }
}
