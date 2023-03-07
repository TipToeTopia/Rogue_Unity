using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemThree : ItemType
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        levelManager.itemThreeCollected.OnValueChanged += levelManager.UpdateItemThreeText;

    }
    public override void OnCollisionEnter(Collision Collision)
    {
        if (TouchedPlayer(Collision) == true)
        {
            levelManager.SetItemThreeTextServerRpc(UpdatePlayerIdentification);
            DespawnObjectServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    protected override void DespawnObjectServerRpc()
    {
        levelManager.itemThreeCollected.OnValueChanged -= levelManager.UpdateItemThreeText;
        base.DespawnObjectServerRpc();
    }

}
