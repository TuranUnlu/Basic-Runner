using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        pending,
        start,
        fail,
        win
    }
    [Header("Game State")]
    public GameState gameState;

    [Header("Level")]
    public int level;

    [Header("Datas")]
    [Tooltip("Player Prefs Data Scriptable Object")]
    public PlayerPrefsData PPData;

    [Tooltip("Game Data Scriptable Object")]
    public GameData gameData;
    
    
    #region OnEnable & OnDisable
    private void OnEnable()
    {
        ObjectManager.GameManager = this;
        ObjectManager.PPData = PPData;
        ObjectManager.GameData = gameData;

        EventManager.Start += GameStart;
        EventManager.Pause += GamePause;
        EventManager.Fail  += GameFail;
        EventManager.Win   += GameWin;
        EventManager.EndGame += EndGameButtonAction;
    }

    private void OnDisable()
    {
        EventManager.Start -= GameStart;
        EventManager.Pause -= GamePause;
        EventManager.Fail  -= GameFail;
        EventManager.Win   -= GameWin;
        EventManager.EndGame -= EndGameButtonAction;
    }
    
    #endregion

    #region GameEvents
    private void GameStart()
    {
        gameState = GameState.start;
    }
    private void GameFail()
    {
        gameState = GameState.fail;
    }
    private void GameWin()
    {
        gameState = GameState.win;
    }
    private void GamePause()
    {
        gameState = GameState.pending;
    }
    #endregion

    private void Awake()
    {
        AwakeMethods();
    }
    
    #region AwakeMethods

    private void AwakeMethods()
    {
        SetLevel();
    }
    private void SetLevel()
    {
        level = PlayerPrefs.GetInt(PPData.level);
        gameData.levelForCanvas = level + 1;
        gameData.level = level;
    }

    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        StartMethods();
    }
    
    #region Start Methods

    private void StartMethods()
    {
        
    }

    #endregion

    private void EndGameButtonAction()
    {
        if (gameState == GameState.win)
        {
            PlayerPrefs.SetInt(PPData.level, (level + 1));
            PlayerPrefs.SetInt(PPData.totalWinCount, PlayerPrefs.GetInt(PPData.totalWinCount) + 1);
        }
        else
        {
            PlayerPrefs.SetInt(PPData.totalFailCount, PlayerPrefs.GetInt(PPData.totalFailCount) + 1);
        }
        
        PlayerPrefs.SetInt(PPData.totalPlayCount, PlayerPrefs.GetInt(PPData.totalPlayCount) + 1);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
