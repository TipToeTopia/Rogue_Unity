using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
using Unity.Collections;
using System;
using UnityEngine.SceneManagement;

public class LevelManager : NetworkBehaviour
{ 
    [SerializeField]
    private TMP_Text keyText;
    [SerializeField]
    private Image keyImage;

    [SerializeField]
    private TMP_Text itemTwoText;
    [SerializeField]
    private Image itemTwoImage;

    [SerializeField]
    private TMP_Text itemThreeText;
    [SerializeField]
    private Image itemThreeImage;

    [SerializeField]
    private GameObject youLoseUI;
    [SerializeField]
    private GameObject youWinUI;

    public const string PLAYER_TAG = "Player";

    private const string TILES_TAG = "Tiles";

    private const float DELAY = 0.5f;

    public List<PlayerMovement> PlayersInGame;

    [HideInInspector]
    public NetworkVariable<FixedString64Bytes> keyCollected = new NetworkVariable<FixedString64Bytes>();
    [HideInInspector]
    public NetworkVariable<FixedString64Bytes> itemTwoCollected = new NetworkVariable<FixedString64Bytes>();
    [HideInInspector]
    public NetworkVariable<FixedString64Bytes> itemThreeCollected = new NetworkVariable<FixedString64Bytes>();

    [HideInInspector]
    public NetworkVariable<bool> completedLevelOne = new NetworkVariable<bool>(false);
    [HideInInspector]
    public NetworkVariable<bool> completedLevelTwo = new NetworkVariable<bool>(false);

    [HideInInspector]
    public NetworkVariable<bool> allPlayersDead = new NetworkVariable<bool>(false);

    private Vector3 originPosition = new Vector3(7.5f, 0.0f, 9.0f);

    [SerializeField]
    private NetworkObject spawnerObject;

    [SerializeField]
    private NetworkObject newTileSurroundings;

    [SerializeField]
    private NetworkObject tileSurroundings;

    private bool HasDestroyedLevelOne = false;
    private bool HasGeneratedLevelTwo = false;

    // series of moditfiables for each level

    [HideInInspector]
    public float stopTimer = 0.5f;
    [HideInInspector]
    public float enemyVisionAngle = 40.0f;
    [HideInInspector]
    public float enemySpeed = 3.5f;
    [HideInInspector]
    public float enemyTargetRange = 10.0f;

    private const float LEVEL_TWO_MAP_SIZE_TIMER = 1.0f;

    private const float LEVEL_TWO_ENEMY_VISION = 70.0f;
    private const float LEVEL_TWO_ENEMY_SPEED = 5.0f;
    private const float LEVEL_TWO_ENEMY_TARGET_RANGE = 15.0f;

    [HideInInspector]
    public const float DEFAULT_PLAYER_SPEED = 5.0f;

    private static LevelManager instance = null;

    public static LevelManager Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void YouWin(bool PreviousValue, bool NewValue)
    {
        youWinUI.SetActive(true);
    }

    public void AreAllPlayersDead()
    {
        // return if any player is found that is not downed

        if (PlayerDownedState(false) == true)
        {
            AllPlayersDeadServerRpc();
        }
    }

    // we can use this function to check for player downed states in the network
    public bool PlayerDownedState(bool AreSomeDowned)
    {
        for (int I = 0; I < PlayersInGame.Count; I++)
        {
            // if isDowned = false, check if all downed, else check if we find more then zero downed

            if (PlayersInGame[I].isDowned == AreSomeDowned)
            {
                if (AreSomeDowned == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        if (AreSomeDowned == true)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void YouLose(bool PreviousValue, bool NewValue)
    {
        youLoseUI.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AllPlayersDeadServerRpc()
    {
        allPlayersDead.Value = true;
    }

    public void LevelTwo(bool PreviousValue, bool NewValue)
    {
        // teleport players and reset players

        DisableInventoryUI();
        
        foreach (PlayerMovement Player in PlayersInGame)
        {

           Player.transform.position = new Vector3(originPosition.x, Player.transform.position.y, originPosition.z);

           Player.isInvisible = false;
           Player.isDowned = false;

           Player.movementSpeed = DEFAULT_PLAYER_SPEED;

        }

        DestroyWorldServerRpc();
        StartCoroutine(SpawnNewMapDelay());
    }

    private void DisableInventoryUI()
    {
        keyImage.enabled = false;
        itemThreeImage.enabled = false;
        itemTwoImage.enabled = false;

        keyCollected = new NetworkVariable<FixedString64Bytes>();
        itemTwoCollected = new NetworkVariable<FixedString64Bytes>();
        itemThreeCollected = new NetworkVariable<FixedString64Bytes>();

        itemThreeText.text = "";
        itemTwoText.text = "";
        keyText.text = "";
    }

    IEnumerator SpawnNewMapDelay()
    {
        yield return new WaitForSeconds(DELAY);
        SpawnLevelTwoServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnLevelTwoServerRpc()
    {
        // after a delay, spawn a new tile surrounding and origin tile for new runtime generation

        if (!HasGeneratedLevelTwo)
        {
            stopTimer = LEVEL_TWO_MAP_SIZE_TIMER;

            enemyVisionAngle = LEVEL_TWO_ENEMY_VISION;
            enemySpeed = LEVEL_TWO_ENEMY_SPEED;
            enemyTargetRange = LEVEL_TWO_ENEMY_TARGET_RANGE;

            SpawnNetworkedObjects(newTileSurroundings);
            SpawnNetworkedObjects(spawnerObject);

            HasGeneratedLevelTwo = true;
        } 
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyWorldServerRpc()
    {
        // get all objects in scene and destroy

        if (HasDestroyedLevelOne == false)
        {
            GameObject[] Objects;

            Objects = GameObject.FindGameObjectsWithTag(TILES_TAG);

            foreach (GameObject Object in Objects)
            {
                Object.GetComponent<NetworkObject>().Despawn();
            }

            tileSurroundings.Despawn();

            HasDestroyedLevelOne = true;

        }
    }

    private void SpawnNetworkedObjects(NetworkObject SpawnObject)
    {
        NetworkObject ObjectInstance = Instantiate(SpawnObject, transform.position, Quaternion.identity);
        ObjectInstance.Spawn();
    }

    public void UpdateItemThreeText(FixedString64Bytes PreviousValue, FixedString64Bytes NewValue)
    {
        itemThreeText.text = itemThreeCollected.Value.ToString();
        itemThreeImage.enabled = true;
    }

    public void UpdateItemTwoText(FixedString64Bytes PreviousValue, FixedString64Bytes NewValue)
    {
        itemTwoText.text = itemTwoCollected.Value.ToString();
        itemTwoImage.enabled = true;
    }

    public void UpdateKeyText(FixedString64Bytes PreviousValue, FixedString64Bytes NewValue)
    {
        keyText.text = keyCollected.Value.ToString();
        keyImage.enabled = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void YouWinServerRpc()
    {
        completedLevelTwo.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void LevelTwoServerRpc()
    {
        completedLevelOne.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetKeyTextServerRpc(string ClientID)
    {
        keyCollected.Value = ClientID;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetItemTwoTextServerRpc(string ClientID)
    {
        itemTwoCollected.Value = ClientID;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetItemThreeTextServerRpc(string ClientID)
    {
        itemThreeCollected.Value = ClientID;
    }
}
