using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPrefsData", menuName = "Datas/Player PrefsData")]
public class PlayerPrefsData : ScriptableObject
{
    [Header("Player Prefs Strings")]
    //
    public string money;
    public string level;

    public string earningAmount;
    public string health;
    
    public string upgradeCostEarning;
    public string upgradeCostHealth;

    public string totalPlayCount;
    public string totalFailCount;
    public string totalWinCount;
}
