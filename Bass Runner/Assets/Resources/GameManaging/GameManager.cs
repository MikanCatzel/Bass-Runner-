using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Space(15)]
    [Header("TileVariables")]

    #region TileVariables
    GameObject tileContainer;
    public GameObject tileBase;
    public GameObject tileRed;
    public Slider tileSpeedSlider;
    List<GameObject> allTiles;
    public int nWidthTiles;
    public int nLengthTiles;
    int tileCounter = 0;
    float[] tilePositions;    // store last row
    #endregion

    [Space(15)]
    [Header("EntityVariables")]

    #region EntityVariables
    public GameObject coinPrefab;
    GameObject entityContainer;
    GameObject coinContainer;
    List<GameObject> allMovingEntities;
    #endregion

    [Space(15)]
    [Header("GlobalVariables")]

    #region GlobalVariables
    public float globalSpeed = 5;
    [HideInInspector]
    public float playerScore = 0;
    #endregion

    [Space(15)]
    [Header("GUIVariables")]

    #region GUIVariables
    public Text scoreText;
    public Text onDeathText;
    #endregion

    [Space(15)]
    [Header("PlayerVariables")]

    #region PlayerVariables
    public GameObject playerObject;
    public GameObject playerPrefab;
    public LayerMask playerLayerMask;
    public bool allowForwardMovement;
    Vector3 playerRayPosition;
    #endregion


    void Start()
    {
<<<<<<< HEAD
        tileContainer = GameObject.Find("Canvas");
        coinContainer = GameObject.Find("Main Camera");
=======
        tileContainer = GameObject.Find("Bye");
        coinContainer = GameObject.Find("Hello");
>>>>>>> origin/master

        GenerateTiles();
        SetupPlayer();
    }


    void Update()
    {
        if(isPlayerAlive())
        {
            RunGUI();
            PlayerMovement();
            SpawnEntities();
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            Application.LoadLevel(1);
        }
    }

    #region GUIMethods
    public void RunGUI ()
    {
        globalSpeed = tileSpeedSlider.value;
        scoreText.text = playerScore.ToString();
    }
    #endregion

    #region PlayerMethods

    public bool isPlayerAlive ()
    {
        if (playerObject != null)
            return true;
        else
            return false;
    }

    public void PlayerMovement()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && allowForwardMovement)
            PlayerPositioning(playerObject.transform.position + new Vector3(0, 0, 1.05f));
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            PlayerPositioning(playerObject.transform.position + new Vector3(-1.05f, 0, 0));
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            PlayerPositioning(playerObject.transform.position + new Vector3(1.05f, 0, 0));
        else if (!Input.anyKeyDown)
            PlayerPositioning(playerObject.transform.position);
    }

    public void PlayerPositioning(Vector3 playerRay)
    {
        Ray ray = new Ray(playerRay, Vector3.down);
        RaycastHit rayInfo;

        bool hit = Physics.Raycast(ray, out rayInfo, 5, playerLayerMask);
        
        if (hit)
        {
            if (rayInfo.collider.tag == "Enemy")
            {
                Vector3 targetPosition = new Vector3(rayInfo.collider.gameObject.transform.position.x, playerObject.transform.position.y, rayInfo.collider.gameObject.transform.position.z);
                playerObject.transform.position = targetPosition;
                PlayerDeath();
            }
            else
            {
                Vector3 targetPosition = new Vector3(rayInfo.collider.gameObject.transform.position.x, playerObject.transform.position.y, rayInfo.collider.gameObject.transform.position.z);
                playerObject.transform.position = targetPosition;
            }
        }
    }

    public void SetupPlayer()
    {
        float xStart = (nWidthTiles % 2 == 0) ? .525f : 0;
        playerObject = Instantiate(playerPrefab, new Vector3(xStart, .5f, 4.2f), Quaternion.identity, GameObject.Find("EntityContainer").transform) as GameObject;
        onDeathText.gameObject.SetActive(false);
    }

    public void PlayerDeath()
    {
        // kill player somehow
        playerObject = null;
        onDeathText.gameObject.SetActive(true);
        onDeathText.text = "You died.\nScore: " + playerScore;
    }
    #endregion

    #region TileSpawning

    public void SpawnEntities ()
    {
        if (allTiles[0].transform.position.z <= 0f)
        {
            if(isPlayerAlive())
            {
                PlayerPositioning(playerObject.transform.position + new Vector3(0, 0, 1.05f));
            }
            playerScore++;
            for (int x = 0; x < nWidthTiles; x++)
            {
                GameObject toSpawn = tileBase;
                float rand = Random.Range(0.0f, 1.0f);
                if (rand <= 0.8f)
                    toSpawn = tileBase;
                else if (rand > 0.8f && rand < 0.9f)
                    toSpawn = tileRed;

                if(rand > 0.8f)
                {
                    GameObject coinGO = (GameObject)Instantiate(coinPrefab, new Vector3(tilePositions[x], 0.8f, allTiles[allTiles.Count - 1 - x].transform.position.z + 1.05f), Quaternion.identity, coinContainer.transform);

                    allMovingEntities.Add(coinGO);
                }

                GameObject tileGO = (GameObject)Instantiate(toSpawn, new Vector3(tilePositions[x], 0, allTiles[allTiles.Count - 1 - x].transform.position.z + 1.05f), Quaternion.identity, tileContainer.transform);
                allTiles.Add(tileGO);
                Destroy(allTiles[0]);
                allTiles.RemoveAt(0);

                tileGO.name = tileCounter + "";
                tileCounter++;
            }
        }
        else
        {
            foreach (GameObject go in allTiles)
            {
                MoveObject(go, Vector3.back, globalSpeed);
            }

            foreach (GameObject go in allMovingEntities)
            {
                MoveObject(go, Vector3.back, globalSpeed);
            }
        }
    }

    public void MoveObject (GameObject _go, Vector3 _dir, float _speed)
    {
        _go.transform.Translate(_dir * _speed * Time.deltaTime);
    }

    public void GenerateTiles()
    {
        allTiles = new List<GameObject>();
        allMovingEntities = new List<GameObject>();

        tilePositions = new float[nWidthTiles];
        for (int x = 0; x < nWidthTiles; x++)
        {
            float startPointX = -((nWidthTiles - 1) * 1.05f / 2);   // negative to start at left side, (nWidthTiles - 1) * scale of tile divided by 2
            float _x = startPointX + x * 1.05f;

            tilePositions[x] = _x;
        }

        for (int z = 0; z < nLengthTiles; z++)
        {
            for (int x = 0; x < nWidthTiles; x++)
            {
                float startPointX = -((nWidthTiles-1)*1.05f / 2);   // negative to start at left side, (nWidthTiles - 1) * scale of tile divided by 2
                float _x = startPointX + x * 1.05f;
                float _z = z * 1.05f;
                Vector3 pos = new Vector3(_x, 0, _z);
                GameObject go = (GameObject)Instantiate(tileBase, pos, Quaternion.identity, tileContainer.transform);

                go.name = tileCounter + "";
                tileCounter++;

                allTiles.Add(go);
            }
        }
    }
    #endregion

}