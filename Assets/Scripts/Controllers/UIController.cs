using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class UIController : MonoBehaviour
{
    #region Panel Tap To Start & Variables
    
    [Header("Tap To Start Panel")]
    //
    [SerializeField] private GameObject panelTapToStart;
    [SerializeField] private Button buttonTapToStart;
    [SerializeField] private TextMeshProUGUI textLevelTapToStart;
    [SerializeField] private TextMeshProUGUI textMoneyTotal;
    
    #endregion
    
    #region Upgrade Variables

    [Header("Upgrade Variables")]
    //
    [SerializeField] private Button buttonUpgradeHealth;
    [SerializeField] private Button buttonUpgradeEarning;

    [SerializeField] private TextMeshProUGUI textMaxHealthAmount;
    [SerializeField] private TextMeshProUGUI textEarningAmount;

    [SerializeField] private TextMeshProUGUI textUpgradeCostHealth;
    [SerializeField] private TextMeshProUGUI textUpgradeCostEarning;

    public enum UpgradeType
    {
        health,
        earning
    }

    #endregion
    
    #region Panel In Game & Variables

    [Header("In Game Panel")]
    //
    [SerializeField] private GameObject panelInGame;
    [SerializeField] private TextMeshProUGUI textMoneyInGame;
    [SerializeField] private TextMeshProUGUI textLevelCurrentInGame;
    [SerializeField] private TextMeshProUGUI textLevelGoal;
    [SerializeField] private Slider levelProgressBar;
    [SerializeField] private TextMeshProUGUI textHealthAmount;
    [Tooltip("Needed Time To Increase Money Amount 1 by 1")]
    [SerializeField] private float moneyTextIncreaseTime;

    [SerializeField] private Transform playerTransform;

    #endregion
    
    
    #region End Game Panel & Variables

    [Header("End Game Panel")]
    //
    [SerializeField] private GameObject panelEndGame;
    [SerializeField] private TextMeshProUGUI textMessageEndGame;
    [SerializeField] private TextMeshProUGUI textButtonEndGame;
    [SerializeField] private Button buttonEndGame;

    [SerializeField] private Sprite spriteEndGameButtonWin;
    [SerializeField] private Sprite spriteEndGAmeButtonFail;

    [SerializeField] private TextMeshProUGUI textEndGameMoneyMessage;
    [SerializeField] private TextMeshProUGUI textEndGameMoneyInGame;
    [SerializeField] private TextMeshProUGUI textEndGameMoneyTotal;

    [SerializeField] private List<string> messagesEndGameMainWin = new List<string>();
    [SerializeField] private List<string> messagesEndGameMainFail = new List<string>();
    [SerializeField] private List<string> messagesEndGameButtonWin = new List<string>();
    [SerializeField] private List<string> messagesEndGameButtonFail = new List<string>();

    #endregion
    
    
    private IEnumerator textMoneyRoutine;

    private GameData GameData;
    private PlayerPrefsData PPData;
    private GameManager GM;



    #region OnEnable & OnDisable
    private void OnEnable()
    {
        ObjectManager.UIController = this;

        EventManager.IncreaseInGameMoney += IncreaseInGameMoneyAmount;
        EventManager.ChangeInGameHealth  += UpdateHealthInGame;
        EventManager.Win += ShowPanelEndGame;
        EventManager.Fail += ShowPanelEndGame;
    }

    private void OnDisable()
    {
        EventManager.IncreaseInGameMoney -= IncreaseInGameMoneyAmount;
        EventManager.ChangeInGameHealth  -= UpdateHealthInGame;
        EventManager.Win  -= ShowPanelEndGame;
        EventManager.Fail -= ShowPanelEndGame;
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
        GetDatas();
        GetGameManager();
        ShowPanelTapToStart();
        SetProgresBarSettings();
    }

    private void GetDatas()
    {
        PPData = ObjectManager.PPData;
        GameData = ObjectManager.GameData;
    }

    private void GetGameManager()
    {
        GM = ObjectManager.GameManager;
    }

    private void SetProgresBarSettings()
    {
        playerTransform = ObjectManager.PlayerController.transform;
        levelProgressBar.minValue = playerTransform.position.z;
        levelProgressBar.maxValue = ObjectManager.FinishTrigger.transform.position.z;
        levelProgressBar.value = playerTransform.position.z;
    }
    
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (GM.gameState == GameManager.GameState.start)
        {
            UpdateProgressBar();
        }
    }
    
    #region Panel Tap To Start Methods

    private void ShowPanelTapToStart()
    {
        CheckUpgradesFirstTimeValues();
        UpdatePanelTapToStart();
        UpdateMoneyAmountAtBegining();
        panelTapToStart.SetActive(true);
    }
    
    //Called From TapToStart Button
    public void ButtonActionTapToStart()
    {
        buttonTapToStart.enabled = false;
        ClosePanelTapToStart();
        EventManager.Start();
    }

    private void ClosePanelTapToStart()
    {
        panelTapToStart.SetActive(false);
        ShowPanelInGame();
    }

    private void UpdateMoneyAmountAtBegining()
    {
        textMoneyTotal.text = GameData.moneyTotal.ToString();
    }

    private void UpdatePanelTapToStart()
    {
        textLevelTapToStart.text = "Stage "+ GameData.levelForCanvas.ToString();
        GameData.moneyTotal = PlayerPrefs.GetInt(PPData.money);
        //textMoneyTotal.text = GameData.moneyTotal.ToString();
        
        UpdateUpgrades();
    }

    #endregion
    
    #region Upgrade Methods

    private void CheckUpgradesFirstTimeValues()
    {
        CheckVariablesFirstValue(PPData.health, GameData.healthDefault);
        CheckVariablesFirstValue(PPData.upgradeCostHealth, GameData.healthCostDefault);
        CheckVariablesFirstValue(PPData.earningAmount, GameData.moneyEarningDefault);
        CheckVariablesFirstValue(PPData.upgradeCostEarning, GameData.moneyEarningCostDefault);
    }

    private void CheckVariablesFirstValue(string ppName, int defaultValue)
    {
        if (PlayerPrefs.GetInt(ppName) < defaultValue)
        {
            PlayerPrefs.SetInt(ppName, defaultValue);
        }
    }

    private void UpdateUpgrades()
    {
        GameData.moneyMultiplier = PlayerPrefs.GetInt(PPData.earningAmount);
        GameData.healthAmount = PlayerPrefs.GetInt(PPData.health);

        textMaxHealthAmount.text = GameData.healthAmount.ToString();
        textEarningAmount.text = GameData.moneyMultiplier.ToString();
        
        CheckUpgradeButtons();
    }

    private void CheckUpgradeButtons()
    {
        UpdateUpgradeButtonNText(buttonUpgradeEarning, textUpgradeCostEarning, PlayerPrefs.GetInt(PPData.upgradeCostEarning));
        UpdateUpgradeButtonNText(buttonUpgradeHealth, textUpgradeCostHealth, PlayerPrefs.GetInt(PPData.upgradeCostHealth));
    }

    private void UpdateUpgradeButtonNText(Button targetButton, TextMeshProUGUI targetTextField, int cost)
    {
        targetTextField.text = cost.ToString();
        if (cost > GameData.moneyTotal)
        {
            targetButton.interactable = false;
        }
        else
        {
            targetButton.interactable = true;
        }
    }

   
    //Called By Buttons
    public void ButtonUpgradeActions(int targetUpgradeType)
    {
        if (targetUpgradeType ==(int) UpgradeType.earning)
        {
            UpdatePlayerPrefs(PPData.earningAmount, 1);
            UpdatePlayerPrefs(PPData.money, -PlayerPrefs.GetInt(PPData.upgradeCostEarning));
            UpdatePlayerPrefs(PPData.upgradeCostEarning, 100);
        }
        else if (targetUpgradeType ==(int) UpgradeType.health)
        {
            UpdatePlayerPrefs(PPData.health, 1);
            UpdatePlayerPrefs(PPData.money, -PlayerPrefs.GetInt(PPData.upgradeCostHealth));
            UpdatePlayerPrefs(PPData.upgradeCostHealth, 100);
        }
        
        UpdatePanelTapToStart();
        
        UpdateMoneyAmount(textMoneyTotal, GameData.moneyTotal, moneyTextIncreaseTime);
    }

    private void UpdatePlayerPrefs(string targetPPString, int amount)
    {
        PlayerPrefs.SetInt(targetPPString, PlayerPrefs.GetInt(targetPPString) + amount);
    }
    
    #endregion
    
    #region Panel In Game Methods

    private void ShowPanelInGame()
    {
        PreparePanelInGame();
        panelInGame.SetActive(true);
    }

    private void UpdateProgressBar()
    {
        levelProgressBar.value = playerTransform.position.z;
    }

    private void ClosePanelInGame()
    {
        panelInGame.SetActive(false);
    }

    private void PreparePanelInGame()
    {
        textLevelCurrentInGame.text = GameData.levelForCanvas.ToString();
        textLevelGoal.text = (GameData.levelForCanvas + 1).ToString();
    }
    
    #endregion

    #region Update Money Amount
    private void IncreaseInGameMoneyAmount()
    {
        UpdateMoneyAmount(textMoneyInGame, EventManager.moneyInGame, moneyTextIncreaseTime);
    }

    private void UpdateMoneyAmount(TextMeshProUGUI targetText, int targetValue, float changeTime)
    {
        if (textMoneyRoutine != null)
        {
            StopCoroutine(textMoneyRoutine);
        }

        textMoneyRoutine = UpdateMoneyAmountRoutine(targetText, targetValue, changeTime);
        StartCoroutine(textMoneyRoutine);
    }

    private IEnumerator UpdateMoneyAmountRoutine(TextMeshProUGUI targetText, int targetValue, float changeTime)
    {
        int currentValue = Int32.Parse(targetText.text);;
        //int increaseAmount = (int) ((targetValue - currentValue) / (changeTime * GameData.moneyMultiplier));
        if (currentValue < targetValue)
        {
            while (currentValue < targetValue)
            {
                //currentValue = currentValue + increaseAmount > targetValue ? targetValue : currentValue + increaseAmount;
                currentValue++;
                targetText.text = currentValue.ToString();
                yield return null;
            }
        }
        else
        {
            while (currentValue > targetValue)
            {
                //currentValue = currentValue + increaseAmount > targetValue ? targetValue : currentValue + increaseAmount;
                currentValue--;
                targetText.text = currentValue.ToString();
                yield return null;
            }
        }
        
    }
    #endregion
    
    #region UpdateHealth

    private void UpdateHealthInGame()
    {
        textHealthAmount.text = EventManager.healthInGame.ToString();
    }
    
    
    #endregion
    
    #region Panel End Game

    private void ShowPanelEndGame()
    {
        ClosePanelInGame();
        PreparePanelEndGame();
        panelEndGame.SetActive(true);
    }

    private void PreparePanelEndGame()
    {
        Debug.Log(GM.gameState);
        if (GM.gameState == GameManager.GameState.win)
        {
            buttonEndGame.image.sprite = spriteEndGameButtonWin;
            textButtonEndGame.text = messagesEndGameButtonWin[Random.Range(0, messagesEndGameButtonWin.Count)];
            textMessageEndGame.text = messagesEndGameMainWin[Random.Range(0, messagesEndGameButtonWin.Count)];
        }
        else if (GM.gameState == GameManager.GameState.fail)
        {
            buttonEndGame.image.sprite = spriteEndGAmeButtonFail;
            textButtonEndGame.text = messagesEndGameButtonFail[Random.Range(0, messagesEndGameButtonFail.Count)];
            textMessageEndGame.text = messagesEndGameMainFail[Random.Range(0, messagesEndGameMainFail.Count)];
        }
        
        EndGameMoneyAction();
    }

    private void EndGameMoneyAction()
    {
        textEndGameMoneyTotal.text = GameData.moneyTotal.ToString();
        textEndGameMoneyInGame.text = EventManager.moneyInGame.ToString();
        
        if (GM.gameState == GameManager.GameState.win)
        {
            PlayerPrefs.SetInt(PPData.money, (PlayerPrefs.GetInt(PPData.money) + EventManager.moneyInGame));
        }

        StartCoroutine(EndGameMoneyRoutine(GM.gameState == GameManager.GameState.win));

    }

    private IEnumerator EndGameMoneyRoutine(bool isWin)
    {
        yield return null;

        int changeAmount = (int) (EventManager.moneyInGame / 100f) > 1 ? (int) (EventManager.moneyInGame / 100f) : 1;
        int targetTotalAmount = GameData.moneyTotal + EventManager.moneyInGame;

        yield return new WaitForSeconds(2f);
        
        while (EventManager.moneyInGame > 0)
        {
            EventManager.moneyInGame = EventManager.moneyInGame - changeAmount > 0 ? EventManager.moneyInGame - changeAmount : 0;

            textEndGameMoneyInGame.text = EventManager.moneyInGame.ToString();

            if (isWin)
            {
                GameData.moneyTotal = GameData.moneyTotal + changeAmount > targetTotalAmount ? targetTotalAmount : GameData.moneyTotal + changeAmount;
                textEndGameMoneyTotal.text = GameData.moneyTotal.ToString();
            }

            yield return null;
        }
    }

    public void ButtonActionEndGame()
    {
        buttonEndGame.interactable = false;
        EventManager.EndGame();
    }
    
    #endregion
    
}
