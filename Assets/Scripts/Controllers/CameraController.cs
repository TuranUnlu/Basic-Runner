using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Camera Settings")]
    //
    [SerializeField] private Vector3 offSet;
    
    [SerializeField] private Vector3 upOffset;

    [SerializeField] private float smoothTime;

    [SerializeField] private float rotateSpeed;
    
    
    private Vector3 velocity = Vector3.zero;

    private Vector3 targetRotation;


    private Vector3 targetPos;
    
    

    private Transform target;

    private GameManager GM;
    
    #region OnEnable & OnDisable
    private void OnEnable()
    {
        ObjectManager.CameraController = this;

        EventManager.CameraTarget += SetTarget;
    }

    private void OnDisable()
    {
        EventManager.CameraTarget -= SetTarget;
    }
    
    #endregion

    private void SetTarget(Transform goalTarget)
    {
        target = goalTarget;
    }


    // Start is called before the first frame update
    void Start()
    {
        StartMethods();
    }
    
    #region Start Methods

    private void StartMethods()
    {
        GetGameManager();
    }

    private void GetGameManager()
    {
        GM = ObjectManager.GameManager;
    }
    
    #endregion

    void LateUpdate()
    {
        if (GM.gameState == GameManager.GameState.start)
        {
            FollowTarget(target);
            //LookTarget(target);
        }
        else if (GM.gameState == GameManager.GameState.win)
        {
            //LookTarget(target);
            EndGameMovement(target);
        }
    }

    private void FollowTarget(Transform goalTarget)
    {
        targetPos = goalTarget.position + offSet;
        targetPos.x = 0;
        transform.position = targetPos;
    }

    private void LookTarget(Transform goalTarget)
    {
        transform.LookAt(goalTarget.position + Vector3.up);
    }

    private void EndGameMovement(Transform goalTarget)
    {
        targetPos = goalTarget.position + offSet;           
        transform.position = new Vector3(transform.position.x, Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime).y, transform.position.z);

        targetRotation = goalTarget.position - transform.position + upOffset;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetRotation), Time.deltaTime * 3);

        transform.RotateAround(goalTarget.position, Vector3.up, rotateSpeed * Time.deltaTime);
    }
}
