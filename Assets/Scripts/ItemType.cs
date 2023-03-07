using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemType : NetworkBehaviour
{
    // base class for item objects

    protected LevelManager levelManager;
    protected string UpdatePlayerIdentification;

    public override void OnNetworkSpawn()
    {
        levelManager = LevelManager.Instance;
    }

    public virtual void OnCollisionEnter(Collision Collision)
    {

    }

    [ServerRpc(RequireOwnership = false)]
    protected virtual void DespawnObjectServerRpc()
    {
        Destroy(gameObject);
    }

    protected virtual bool TouchedPlayer(Collision Collider)
    {
        if (Collider.gameObject.tag == LevelManager.PLAYER_TAG)
        {
            UpdatePlayerIdentification = Collider.gameObject.GetComponent<NetworkObject>().OwnerClientId.ToString();

            return true;
        }

        return false;
    }
}
