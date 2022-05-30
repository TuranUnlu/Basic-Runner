using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventManager
{
    #region Level Actions
    
    public static Action Start;
    public static Action Pause;
    public static Action Win;
    public static Action Fail;
    public static Action EndGame;
    #endregion
    
    #region Camera Actions

    public static Action<Transform> CameraTarget;

    #endregion

    public static int moneyInGame;
    public static Action IncreaseInGameMoney;

    public static int healthInGame;
    public static Action ChangeInGameHealth;


}
