using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    #region Movement Settings & Variables

    [Header("Movement Settings & Variables")]
    //
    [SerializeField] private Rigidbody RB;

    [SerializeField] private float speedForward;

    private float speedSide;

    [SerializeField] private float smoothValue;

    [SerializeField] private float screenSizeDivideAmount;

    private float mousePosXLastFrame;
    
    private float mousePosX;

    private float mousePosDif;

    private float deltaTime;

    private float swipeValue;

    private Vector3 targetPos;

    private float mouseFirstPosX;

    private float firstPosX;
    
    [Tooltip("Minimum X Value For Player")]
    [SerializeField] private float limitMinX;

    [Tooltip("Maximum X Value For Player")]
    [SerializeField] private float limitMaxX;

    private enum InputType
    {
        none,
        left,
        right
    }

    private InputType inputType;
    
    
    #endregion

    [Header("Animator")]
    //
    [SerializeField] private Animator anim;

    [Space]
    [SerializeField] private float magnetSpeed;

    private List<GameObject> collectableList = new List<GameObject>();
    private GameObject tempCollectable;
    private GameObject tempObstacle;

    private Vector3 diff;

    private GameManager GM;
    private PlayerPrefsData PPData;
    private GameData GameData;
    private GameData.Tags Tags;
    private GameData.PlayerData playerData;
    private GameData.PlayerData.AnimationParameters animParameters;
    
    #region OnEnable & OnDisable
    private void OnEnable()
    {
        ObjectManager.PlayerController = this;

        EventManager.Start += AnimWalk;
        EventManager.Start += SetHealth;
        EventManager.Fail += AnimFail;
        EventManager.Win += AnimCheer;
    }

    private void OnDisable()
    {
        EventManager.Start -= AnimWalk;
        EventManager.Start -= SetHealth;
        EventManager.Fail -= AnimFail;
        EventManager.Win -= AnimCheer;
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
        GetGameManager();
        GetDatas();
        GetTags();
        SetCameraTarget();
        AnimIdle();
        ResetInGameMoney();
        SetSideSpeed();
    }

    private void GetGameManager()
    {
        GM = ObjectManager.GameManager;
    }

    private void GetDatas()
    {
        GameData = ObjectManager.GameData;
        playerData = GameData.playerData;
        animParameters = playerData.anim;
        PPData = ObjectManager.PPData;
    }

    private void GetTags()
    {
        Tags = GameData.tags;
    }

    private void SetCameraTarget()
    {
        EventManager.CameraTarget(transform);
    }

    private void ResetInGameMoney()
    {
        EventManager.moneyInGame = 0;
        EventManager.IncreaseInGameMoney();
    }

    private void SetSideSpeed()
	{
        speedSide = Screen.width / screenSizeDivideAmount;
    }
    
    #endregion
    
    // Update is called once per frame
    void Update()
    {
        if (GM.gameState == GameManager.GameState.start)
        {
            GetInput();
            
            MagnetAction();
        }
    }
    
    #region Input

    private void GetInput()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            mousePosXLastFrame = Input.mousePosition.x;
        }
        else if (Input.GetMouseButton(0))
        {
            mousePosX = Input.mousePosition.x;

            mousePosDif = mousePosXLastFrame - mousePosX;

            if (mousePosDif == 0)
            {
                inputType = InputType.none;
            }
            else if (mousePosDif > 0)
            {
                inputType = InputType.left;
            }
            else if (mousePosDif < 0)
            {
                inputType = InputType.right;
            }

            mousePosXLastFrame = mousePosX;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            inputType = InputType.none;
        }
        */
        targetPos = transform.position;

        if (Input.GetMouseButtonDown(0))
        {
            mouseFirstPosX = Input.mousePosition.x;
            firstPosX = transform.position.x;
        }
        else if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition.x != mouseFirstPosX)
            {
                targetPos.x = firstPosX + (Input.mousePosition.x - mouseFirstPosX) / speedSide;
                targetPos.x = targetPos.x < limitMinX ? limitMinX : targetPos.x;
                targetPos.x = targetPos.x > limitMaxX ? limitMaxX : targetPos.x;
            }
        }
    }
    
    #endregion
    
    #region Magnet

    private void MagnetAction()
    {
        for (int i = 0; i < collectableList.Count; i++)
        {
            if (i < collectableList.Count)
            {
                diff = transform.position - collectableList[i].transform.position;
                diff = diff.magnitude > 1 ? diff.normalized * 1.5f : diff;

                collectableList[i].transform.position += diff * deltaTime * magnetSpeed;
            }
        }
    }
    
    #endregion

    private void FixedUpdate()
    {
        if (GM.gameState == GameManager.GameState.start)
        {
            Movement();
        }
    }
    
    #region Movement

    private void Movement()
    {
        /*
        deltaTime = Time.deltaTime;
        targetPos = transform.position;
        MovementForward();
        MovementSide();
        AssingMovement();
        */



        targetPos.z += speedForward;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothValue);
    }

    private void MovementForward()
    {
        targetPos += transform.forward * speedForward * deltaTime;
    }

    private void MovementSide()
    {
        if (inputType == InputType.left)
        {
            targetPos -= transform.right * speedSide * deltaTime;
        }
        else if (inputType == InputType.right)
        {
            targetPos += transform.right * speedSide * deltaTime;
        }
        
        //Limitation

        targetPos.x = targetPos.x < limitMinX ? limitMinX : targetPos.x;
        targetPos.x = targetPos.x > limitMaxX ? limitMaxX : targetPos.x;

    }

    private void AssingMovement()
    {
        transform.position = targetPos;
    }
    
    #endregion
    
    #region Collision & Trigger

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.collectable))
        {
            
            tempCollectable = other.gameObject;
            if (!collectableList.Contains(tempCollectable))
            {
                collectableList.Add(tempCollectable);
            }
            
        }
        else if (other.CompareTag(Tags.obstacle))
        {
            tempObstacle = other.gameObject;
            tempObstacle.GetComponent<Interactable>().DisableInteractable();
            DecreaseHealth();
        }
        else if (other.CompareTag(Tags.finish))
        {
            other.gameObject.SetActive(false);
            EventManager.Win();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(Tags.collectable))
        {
            tempCollectable = collision.collider.gameObject;
            if (collectableList.Contains(tempCollectable))
            {
                collectableList.Remove(tempCollectable);
            }
            tempCollectable.GetComponent<Interactable>().DisableInteractable();
            IncreaseMoney();
        }
        
    }

    #endregion

    private void IncreaseMoney()
    {
        EventManager.moneyInGame += GameData.moneyMultiplier;
        EventManager.IncreaseInGameMoney();
    }

    private void SetHealth()
    {
        EventManager.healthInGame = GameData.healthAmount;
        EventManager.ChangeInGameHealth();
    }

    private void DecreaseHealth()
    {
        EventManager.healthInGame--;
        EventManager.ChangeInGameHealth();
        if (EventManager.healthInGame <= 0)
        {
            EventManager.healthInGame = 0;
            EventManager.Fail();
        }
    }
    
    #region Animation Methods

    private void AnimIdle()
    {
        anim.SetBool(animParameters.walk, false);
        anim.SetBool(animParameters.idle, true);
    }

    private void AnimWalk()
    {
        anim.SetBool(animParameters.walk, true);
        anim.SetBool(animParameters.idle, false);
    }

    private void AnimCheer()
    {
        anim.SetBool(animParameters.cheer, true);
        anim.SetBool(animParameters.walk, false);
    }

    private void AnimFail()
    {
        //anim.SetBool(animParameters.walk, false);
        //anim.SetBool(animParameters.fail, true);
        AnimIdle();
    }
    
    #endregion
    
    
    
}
