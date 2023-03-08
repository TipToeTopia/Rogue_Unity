using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemTwo : ItemType
{
    // item two will be to move objects

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        levelManager.itemTwoCollected.OnValueChanged += levelManager.UpdateItemTwoText;

    }

    public override void OnCollisionEnter(Collision Collision)
    {
        if (TouchedPlayer(Collision) == true)
        {
            levelManager.SetItemTwoTextServerRpc(UpdatePlayerIdentification);
            DespawnObjectServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    protected override void DespawnObjectServerRpc()
    {
        levelManager.itemTwoCollected.OnValueChanged -= levelManager.UpdateItemTwoText;
        base.DespawnObjectServerRpc();
    }
}
