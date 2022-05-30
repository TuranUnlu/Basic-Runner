using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Datas/Game Data")]
public class GameData : ScriptableObject
{
    public int levelForCanvas;
    public int level;
    
    [HideInInspector]
    public int moneyTotal;
    
    public int moneyMultiplier;
    public int healthAmount;

    public int healthDefault;
    public int healthCostDefault;

    public int moneyEarningDefault;
    public int moneyEarningCostDefault;

    [Serializable]
    public class PlayerData
    {
        
        
        
        [Serializable]
        public class AnimationParameters
        {
            public string idle;
            public string walk;
            public string cheer;
            public string fail;
        }

        public AnimationParameters anim;
    }

    public PlayerData playerData;

    [Serializable]
    public class Tags
    {
        public string player;
        public string obstacle;
        public string collectable;
        public string finish;
    }

    public Tags tags;
}
