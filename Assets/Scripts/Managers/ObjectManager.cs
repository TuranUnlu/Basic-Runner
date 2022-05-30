using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectManager 
{
    #region Managers

    public static GameManager GameManager;
    public static LevelGenerationManager LevelGenerationManager;
    public static PoolManager PoolManager;

    #endregion
    
    #region Controllers

    public static PlayerController PlayerController;
    public static CameraController CameraController;
    public static UIController UIController;

    #endregion
    
    #region Datas

    public static PlayerPrefsData PPData;
    public static GameData GameData;

    #endregion

    #region GameObjects

    public static FinishTrigger FinishTrigger;


    #endregion
}
