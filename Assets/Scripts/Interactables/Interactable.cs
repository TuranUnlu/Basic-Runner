using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Interactable : MonoBehaviour
{
    private PoolManager PM;
    public InteractableInfo info;

    [SerializeField] private InteractableInfo targetParticleInfo;

    private IEnumerator particleRoutine;
    private WaitForSeconds delay;
    
    #region OnEnable & OnDisable

    private void OnEnable()
    {
        if (info != null && info.type == InteractableInfo.InteractableType.particle)
        {
            StartParticleRoutine();
        }
    }

    private void OnDisable()
    {
        /*
        if (info != null && info.type == InteractableInfo.InteractableType.particle)
        {
            StopParticleRoutine();
        }
        */

        
    }
    
    #endregion

    private void Start()
    {
        StartMethods();
    }
    
    #region Start Methods

    private void StartMethods()
    {
        GetPoolManager();
    }

    private void GetPoolManager()
    {
        PM = ObjectManager.PoolManager;
    }

    #endregion

    public void DisableInteractable()
    {
        if (info == null || !info.isInPoolManager)
        {
            gameObject.SetActive(false);
        }
        else
        {
            if (info.type == InteractableInfo.InteractableType.obstacle)
            {
                SpawnParticle();
            }
            PM.BackToThePool(info.tagname, gameObject);       
        }
    }

    private void SpawnParticle()
    {
        PM.SpawnFromPool(targetParticleInfo.tagname, transform.position + Vector3.up);
    }

    private void StartParticleRoutine()
    {
        if (particleRoutine != null)
        {
            StopParticleRoutine();
        }
        
        particleRoutine = DelayedDisable();

        StartCoroutine(particleRoutine);
    }

    private void StopParticleRoutine()
    {
        StopCoroutine(particleRoutine);
    }

    private IEnumerator DelayedDisable()
    {
        if (delay == null)
        {
            delay = new WaitForSeconds(info.disableDelayTime);
        }
        yield return delay;
        //gameObject.SetActive(false);
        if (PM == null)
        {
            PM = ObjectManager.PoolManager;
        }
        
        PM.BackToThePool(info.tagname, gameObject);
        
    }
}
