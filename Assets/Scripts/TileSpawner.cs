using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[System.Flags]
public enum CardinalDirection
{
    IsNothing = 0,
    IsUp = 1,
    IsDown = 2,
    IsLeft = 4,
    IsRight = 8,
}

public class TileSpawner : NetworkBehaviour
{
    [SerializeField]
    private TileSurroundings tileSurroundings;

    private int randomTile;

    public bool hasSpawned = false;

    public CardinalDirection spawnDirection;

    private const int RANDOM_INITIAL_RANGE = 0;

    private const string SPAWN_POINT = "SpawnPoint";
    private const string INVOKE_SPAWN = "SpawnTileServerRpc";

    private const float WORLD_SPAWN_DELAY = 0.1f;
    private const float DESTROY_DELAY = 0.5f;

    // TO DO:  clean up of repetition

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        { 
            // Once spawned, do a timed delay and invoke to ensure all collisions, rigidbodies spawn
            Destroy(gameObject, DESTROY_DELAY);
            Invoke(INVOKE_SPAWN, WORLD_SPAWN_DELAY);
   
        }
    }

    void OnTriggerEnter(Collider Other)
    {
        if (Other.CompareTag(SPAWN_POINT))
        {
           hasSpawned = true;

           DestroySpawnerServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void DestroySpawnerServerRpc()
    {
        Destroy(gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnTileServerRpc()
    {
        // spawn new tiles around the point, depending on the cardinal direction of each tile (origin only)

        if (tileSurroundings.stopSpawning == false)
        {
            if (hasSpawned == false)
            {
                if (spawnDirection.HasFlag(CardinalDirection.IsDown)) // bottom
                {
                    randomTile = Random.Range(RANDOM_INITIAL_RANGE, tileSurroundings.bottomRooms.Length);
                    NetworkObject Tile = Instantiate(tileSurroundings.bottomRooms[randomTile], transform.position, tileSurroundings.bottomRooms[randomTile].transform.rotation);
                    Tile.Spawn();
                }
                if (spawnDirection.HasFlag(CardinalDirection.IsUp)) // top
                {
                    randomTile = Random.Range(RANDOM_INITIAL_RANGE, tileSurroundings.topRooms.Length);
                    NetworkObject Tile = Instantiate(tileSurroundings.topRooms[randomTile], transform.position, tileSurroundings.topRooms[randomTile].transform.rotation);
                    Tile.Spawn();
                }
                if (spawnDirection.HasFlag(CardinalDirection.IsLeft)) // left
                {
                    randomTile = Random.Range(RANDOM_INITIAL_RANGE, tileSurroundings.leftRooms.Length);
                    NetworkObject Tile = Instantiate(tileSurroundings.leftRooms[randomTile], transform.position, tileSurroundings.leftRooms[randomTile].transform.rotation);
                    Tile.Spawn();
                }
                if (spawnDirection.HasFlag(CardinalDirection.IsRight)) // right
                {
                    randomTile = Random.Range(RANDOM_INITIAL_RANGE, tileSurroundings.rightRooms.Length);
                    NetworkObject Tile = Instantiate(tileSurroundings.rightRooms[randomTile], transform.position, tileSurroundings.rightRooms[randomTile].transform.rotation);
                    Tile.Spawn();
                }

                hasSpawned = true;

            }
        }
    }

}
