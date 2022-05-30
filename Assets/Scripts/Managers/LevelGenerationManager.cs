using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerationManager : MonoBehaviour
{

    [SerializeField] private Transform levelContainer;

    private GameManager GameManager;
    private GameData GameData;
    private PoolManager PoolManager;

    [Header("Interactable Infos")]
    //
    [Tooltip("Obstacle Info Scriptable Object")]
    [SerializeField]
    private InteractableInfo infoObstacle;

    [Tooltip("Collectable Info Scriptable Object")] [SerializeField]
    private InteractableInfo infoCollectable;

    [Header("Random Level Generation Settings")]
    //
    [Tooltip("Min X Value for Objects Positions")]
    [SerializeField]
    private float xMin;

    [Tooltip("Max X Value for Objects Positions")] [SerializeField]
    private float xMax;

    private float xMid;

    [SerializeField] private float minZSpaceObstacles;
    [SerializeField] private float minZSpaceCollectables;

    [SerializeField] private float distanceZStart;
    [SerializeField] private float distanceZFinish;
    [SerializeField] private float zBegining;


    [Serializable]
    public class CollectablePatterns
    {
        public List<Vector3Int> info = new List<Vector3Int>();
    }

    public List<CollectablePatterns> CollectablePatternsList;


    /*
     *
     *   Prefab Objects Scales Must Be Arrange For 3 Line For Avoiding Future Level Desing Issues 
     * 
     */





    private void Awake()
    {
        ObjectManager.LevelGenerationManager = this;
    }

    #region OnEnable & OnDisable

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        StartMethods();

        //Vector3Int asda = new Vector3Int(1, 0, 3);
        //Debug.Log(asda.y);
    }

    #region Start Methods

    private void StartMethods()
    {
        GetGameManager();
        GetPoolManager();
        GetDatas();
        CheckLevel();
    }

    private void GetGameManager()
    {
        GameManager = ObjectManager.GameManager;
    }

    private void GetDatas()
    {
        GameData = ObjectManager.GameData;
    }

    private void GetPoolManager()
    {
        PoolManager = ObjectManager.PoolManager;
    }


    #endregion

    private void CheckLevel()
    {
        
        //All HandMade Levels Played Condition
        if (GameData.level >= levelContainer.childCount)
        {
            GenerateRandomLevel();
        }
        else
        {
            GenerateLevel(levelContainer.GetChild(GameData.level));
        }
        
    }

    //Level Generation Activated All Hand Made Level Played
    private void GenerateLevel(Transform levelInfosParent)
    {
        //Debug For Level Desing Mistakes
        if (levelInfosParent.childCount < 2)
        {
            Debug.LogWarning("Level Infos Missing for Level " + GameData.level.ToString() + " !");
            return;
        }

        //Generating Collectables
        Transform tempParent = levelInfosParent.GetChild(0);
        GenerateObjects(infoCollectable.tagname, tempParent);

        //Generating Obstacles
        tempParent = levelInfosParent.GetChild(1);
        GenerateObjects(infoObstacle.tagname, tempParent);

    }

    private void GenerateObject(string tagName, Vector3 spawnPos)
    {
        PoolManager.SpawnFromPool(tagName, spawnPos);
    }

    private void GenerateObjects(string tagName, Transform positionsParent)
    {
        for (int i = 0; i < positionsParent.childCount; i++)
        {
            PoolManager.SpawnFromPool(tagName, positionsParent.GetChild(i).position);
        }
    }

    private void GenerateRandomLevel()
    {
        xMid = (xMax + xMin) / 2f;

        float diffX = xMax - xMid;
        
        
        float zMax = ObjectManager.FinishTrigger.transform.position.z - distanceZFinish;
        float zMin = zBegining + distanceZStart;

        float diffZ = zMax - zMin;

        int lineObjectCounter;
        int targetObjectCount;

        float gcd = GreatestCommonDivisor(minZSpaceObstacles, minZSpaceCollectables);
        int[,] spawnInfo = new int[3, (int) (diffZ / gcd)];
        
        /*
         *
         * 1 as a Obstacle 2 as a Collectable
         * 
        */
            
        
        float indexMult;
        int counter = 0;
            
        Vector3 tempSpawnPos = Vector3.zero;
        

        #region Obtacle Generation

        indexMult = minZSpaceObstacles / gcd;
        
        //For X Dimension
        for (float i = zMin; i < zMax; i += minZSpaceObstacles)
        {
            lineObjectCounter = 0;
            targetObjectCount = Random.Range(0, 3);

            if (targetObjectCount != 0)
            {
                tempSpawnPos.z = zMin + minZSpaceObstacles * counter;
                //For X Dimension
                for (int j = 0; j < 3; j++)
                {
                    if (lineObjectCounter < targetObjectCount)
                    {
                        //Must Spawn Obstacle Here
                        if (3 - j <= targetObjectCount - lineObjectCounter)
                        {
                            tempSpawnPos.x = xMin + diffX * j;
                            GenerateObject(infoObstacle.tagname, tempSpawnPos);
                            lineObjectCounter++;
                            spawnInfo[j, Mathf.RoundToInt(counter * indexMult)] = 1;
                        }
                        //Spawning Obstacle According To Random
                        else
                        {
                            if (Random.Range(0, 3) < targetObjectCount)
                            {
                                tempSpawnPos.x = xMin + diffX * j;
                                GenerateObject(infoObstacle.tagname, tempSpawnPos);
                                lineObjectCounter++;
                                spawnInfo[j, Mathf.RoundToInt(counter * indexMult)] = 1;
                            }
                        }
                    }
                }
            }
            
            counter++;
        }
        
        #endregion
        
        #region CollectableGeneration

        indexMult = Mathf.RoundToInt(minZSpaceCollectables / gcd);
        counter = 0;
        tempSpawnPos.y = 0.5f;

        List<Vector3Int> tempPattern;
        Vector3Int tempVector;
        
        for (float i = zMin; i < zMax; i += 0)
        {
            //Setting Pattern Info to get Information From Hand Made Patterns
            tempPattern = CollectablePatternsList[Random.Range(0, CollectablePatternsList.Count)].info;
            
            //Travelling Vector3s
            for (int j = 0; j < tempPattern.Count; j++)
            {
                tempSpawnPos.z = zMin + minZSpaceCollectables * counter;

                if (tempSpawnPos.z >= zMax)
                {
                    i = tempSpawnPos.z;
                    break;
                }
                tempVector = tempPattern[j];
                
                //Traveling inside Vector 3 
                for (int k = 0; k < 3; k++)
                {
                    if (tempPattern[j][k] != 0 && spawnInfo[k, Mathf.RoundToInt(counter * indexMult)] == 0)
                    {
                        //Assigning Spawn Position X According to Vector3 Value
                        tempSpawnPos.x = xMin + diffX * k;
                        GenerateObject(infoCollectable.tagname, tempSpawnPos);
                        spawnInfo[k, Mathf.RoundToInt(counter * indexMult)] = 2;
                    }
                }
               
                /*
               // Debug.Log(tempVector.ToString());

                if (tempVector.x != 0 && spawnInfo[0, Mathf.RoundToInt(counter * indexMult)] == 0)
                {
                   // Debug.Log(tempVector.x.ToString());
                    tempSpawnPos.x = xMin;
                    GenerateObject(infoCollectable.tagname, tempSpawnPos);
                    spawnInfo[0, Mathf.RoundToInt(counter * indexMult)] = 2;
                }
                if (tempVector.y != 0 && spawnInfo[1, Mathf.RoundToInt(counter * indexMult)] == 0)
                {
                    tempSpawnPos.x = xMid;
                    GenerateObject(infoCollectable.tagname, tempSpawnPos);
                    spawnInfo[1, Mathf.RoundToInt(counter * indexMult)] = 2;
                }
                if (tempVector.z != 0 && spawnInfo[2, Mathf.RoundToInt(counter * indexMult)] == 0)
                {
                    tempSpawnPos.x = xMax;
                    GenerateObject(infoCollectable.tagname, tempSpawnPos);
                    spawnInfo[2, Mathf.RoundToInt(counter * indexMult)] = 2;
                }
                */
                counter++;
            }
            
            
        }
        
        
        
        
        #endregion
        
        
        
    }

    private float GreatestCommonDivisor(double firstValue, double secondValue)
    {
        if (firstValue < secondValue)
        {
            return GreatestCommonDivisor(secondValue, firstValue);
        }
        if (Math.Abs(secondValue) < 0.001f)
        {
            return (float)firstValue;
        }
        else
        {
            return GreatestCommonDivisor(secondValue, firstValue - Math.Floor(firstValue / secondValue) * secondValue);
        }
    }
    
    
}
