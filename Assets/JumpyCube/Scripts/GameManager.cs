using System;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Player playerController;
    public ObstacleSpawner obstacleSpawner;

    private bool _isGameOver;

    private float startTime;
    private float elapsedTime;
    void Start()
    {
        if (playerController == null || obstacleSpawner == null)
            throw new Exception("Player controller or obstacleSpawner not assigned");

        GameOver();
    }

    void Update()
    {
        if (!_isGameOver)
        {
            elapsedTime = Time.time - startTime;
        }

        if (_isGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            GameStart();
        }
        else if (!playerController.isVisible() || playerController.isHit())
        {
            GameOver();
        }
    }

    public void GameStart()
    {
        playerController.enabled = true;
        playerController.Enable();
        _isGameOver = false;
        startTime = Time.time;

        obstacleSpawner.StartScript();
    }

    public void GameOver()
    {
        _isGameOver = true;
        playerController.Reset();
        playerController.enabled = false;

        obstacleSpawner.StopScript();
    }
}