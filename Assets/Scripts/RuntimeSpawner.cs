 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[System.Flags]
public enum CardinalBoolDirections
{
    IsNothing = 0,
    IsUp = 1,
    IsDown = 2,
    IsLeft = 4,
    IsRight = 8,
    
}

public class RuntimeSpawner : NetworkBehaviour
{
    private TileSurroundings tileSurroundings;

    [SerializeField]
    private NetworkObject spawnerObject;

    public CardinalBoolDirections boolDirections;

    private Vector3 spawnOffsetUp = new Vector3(0.0f, 0.0f, 10.0f);
    private Vector3 spawnOffsetDown = new Vector3(0.0f, 0.0f, -10.0f);
    private Vector3 spawnOffsetLeft = new Vector3(-10.0f, 0.0f, 0.0f);
    private Vector3 spawnOffsetRight = new Vector3(10.0f, 0.0f, 0.0f);

    private const string TILE_SURROUNDINGS_TAG = "TileSurroundings";

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            tileSurroundings = GameObject.FindGameObjectWithTag(TILE_SURROUNDINGS_TAG).GetComponent<TileSurroundings>();
            tileSurroundings.roomCount.Add(this.gameObject);
            SpawnTileServerRpc();
        }
         
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnTileServerRpc()
    {
        // on each newly spawned tile, if their direction equals the enum flag, spawn a tile spawner on that side

        if (tileSurroundings.stopSpawning == false)
        {
            if (boolDirections.HasFlag(CardinalBoolDirections.IsUp))
            {
                SpawnObject(spawnerObject, spawnOffsetUp);
            }

            if (boolDirections.HasFlag(CardinalBoolDirections.IsDown))
            {
                SpawnObject(spawnerObject, spawnOffsetDown);
            }

            if (boolDirections.HasFlag(CardinalBoolDirections.IsLeft))
            {
                SpawnObject(spawnerObject, spawnOffsetLeft);
            }

            if (boolDirections.HasFlag(CardinalBoolDirections.IsRight))
            {
                SpawnObject(spawnerObject, spawnOffsetRight);
            }
        }
    }

    private void SpawnObject(NetworkObject SpawnObject, Vector3 SpawnOffset)
    {
        NetworkObject ObjectInstance = Instantiate(SpawnObject, transform.position + SpawnOffset, Quaternion.identity);
        ObjectInstance.Spawn();
    }
}
