using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class TileSurroundings : NetworkBehaviour
{
    public NetworkObject[] topRooms;
    public NetworkObject[] bottomRooms;
    public NetworkObject[] rightRooms;
    public NetworkObject[] leftRooms;

    public List<GameObject> roomCount;

    public NetworkObject closedRoom;

    [HideInInspector]
    public bool stopSpawning = false;

    [SerializeField]
    public float waitTime = 0.5f;

    private bool spawnedKey = false;
    private bool spawnedItemTwo = false;
    private bool spawnedItemThree = false;
    private bool spawnedDoor = false;
    private bool spawnedAI = false;

    [SerializeField]
    private NetworkObject keyObject;
    [SerializeField]
    private NetworkObject itemTwoObject;
    [SerializeField]
    private NetworkObject itemThreeObject;
    [SerializeField]
    private NetworkObject doorObject;
    [SerializeField]
    private NetworkObject enemyAI;


    private Vector3 spawnKey;
    private Vector3 spawnItemTwo;
    private Vector3 spawnItemThree;
    private Vector3 spawnDoor;
    private Vector3 spawnAI;

    private Vector3 spawnObjectOffset = new Vector3(0.0f, 1.0f, 0.0f);

    private const int ZERO = 0;
    private const int LAST_INDEX = 1;
    private const int SECOND_TO_LAST_INDEX = 2;
    private const int THIRD_TO_LAST_INDEX = 3;
    private const int FOURTH_TO_LAST_INDEX = 4;
    private const int FIFTH_TO_LAST_INDEX = 5;



    void Update()
    {
        if (IsHost)
        {
            // once all tiles have spawned we go through the count and place items on the last indexes in the world

            if (LevelManager.Instance.stopTimer + waitTime <= ZERO)
            {

                for (int I = 0; I < roomCount.Count; I++)
                {

                    if (I == roomCount.Count - LAST_INDEX)
                    {
                        if (!spawnedKey)
                        {
                            spawnKey = roomCount[I - LAST_INDEX].transform.position;
                            SpawnKeyServerRpc();
                            spawnedKey = true;
                        }
                    }

                    if (I == roomCount.Count - SECOND_TO_LAST_INDEX)
                    {
                        if (!spawnedItemTwo)
                        {
                            spawnItemTwo = roomCount[I - SECOND_TO_LAST_INDEX].transform.position;
                            SpawnItemTwoServerRpc();
                            spawnedItemTwo = true;
                        }
                    }

                    if (I == roomCount.Count - THIRD_TO_LAST_INDEX)
                    {
                        if (!spawnedItemThree)
                        {
                            spawnItemThree = roomCount[I -  THIRD_TO_LAST_INDEX].transform.position;
                            SpawnItemThreeServerRpc();
                            spawnedItemThree = true;
                        }
                    }

                    if (I == roomCount.Count - FOURTH_TO_LAST_INDEX)
                    {
                        if (!spawnedDoor)
                        {
                            spawnDoor = roomCount[I -  FOURTH_TO_LAST_INDEX].transform.position;
                            SpawnDoorServerRpc();
                            spawnedDoor = true;
                        }
                    }

                    if (I == roomCount.Count - FIFTH_TO_LAST_INDEX)
                    {
                        if (!spawnedAI)
                        {
                            spawnAI = roomCount[I - FIFTH_TO_LAST_INDEX].transform.position;
                            SpawnAIServerRpc();
                            spawnedAI = true;
                        }
                    }
                }
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            // delay for the world size to spawn

            DelayServerRpc();
        }

    }

    [ServerRpc(RequireOwnership = false)]
    void DelayServerRpc()
    {
        StartCoroutine(Delay());
    }

    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(LevelManager.Instance.stopTimer);
        stopSpawning = true;
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnKeyServerRpc()
    {
        SpawnObject(keyObject, spawnKey);
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnItemTwoServerRpc()
    {
        SpawnObject(itemTwoObject, spawnItemTwo);
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnItemThreeServerRpc()
    {
        SpawnObject(itemThreeObject, spawnItemThree);
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnDoorServerRpc()
    { 
        SpawnObject(doorObject, spawnDoor);
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnAIServerRpc()
    {
        SpawnObject(enemyAI, spawnAI);
    }

    private void SpawnObject(NetworkObject SpawnObject, Vector3 SpawnPosition)
    {
        NetworkObject ObjectInstance = Instantiate(SpawnObject, SpawnPosition + spawnObjectOffset, Quaternion.identity);
        ObjectInstance.Spawn();
    }

    /*
     *  if (I == roomCount.Count - LAST_INDEX)
                    {
                        if (!spawnedKey)
                        {
                            spawnKey = roomCount[I - LAST_INDEX].transform.position;
                            SpawnKeyServerRpc();
                            spawnedKey = true;
                        }
                    }

                    if (I == roomCount.Count - SECOND_TO_LAST_INDEX)
                    {
                        if (!spawnedItemTwo)
                        {
                            spawnItemTwo = roomCount[I - SECOND_TO_LAST_INDEX].transform.position;
                            SpawnItemTwoServerRpc();
                            spawnedItemTwo = true;
                        }
                    }

                    if (I == roomCount.Count - THIRD_TO_LAST_INDEX)
                    {
                        if (!spawnedItemThree)
                        {
                            spawnItemThree = roomCount[I -  THIRD_TO_LAST_INDEX].transform.position;
                            SpawnItemThreeServerRpc();
                            spawnedItemThree = true;
                        }
                    }

                    if (I == roomCount.Count - FOURTH_TO_LAST_INDEX)
                    {
                        if (!spawnedDoor)
                        {
                            spawnDoor = roomCount[I -  FOURTH_TO_LAST_INDEX].transform.position;
                            SpawnDoorServerRpc();
                            spawnedDoor = true;
                        }
                    }

                    if (I == roomCount.Count - FIFTH_TO_LAST_INDEX)
                    {
                        if (!spawnedAI)
                        {
                            spawnAI = roomCount[I - FIFTH_TO_LAST_INDEX].transform.position;
                            SpawnAIServerRpc();
                            spawnedAI = true;
                        }
                    }
     
     */

}
