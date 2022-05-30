using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "InteractableInfo", menuName = "Infos/Interactable Info")]
public class InteractableInfo : ScriptableObject
{
    public enum InteractableType
    {
        collectable,
        obstacle,
        particle
    }

    [Header("Interactable Type")]
    public InteractableType type;
    
    
    [Header("Pool Manager Settings")]
    public bool isInPoolManager;
    public string tagname;

    [Header("Object Settings")] 
    public Vector3 defaultScale;
    [Tooltip("Target Destroy Delay Time For Particles")]
    public float disableDelayTime;
}
