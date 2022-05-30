using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    private void Awake()
    {
        ObjectManager.FinishTrigger = this;
    }
}
