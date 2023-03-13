using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemThree : ItemType
{
    private const float newPlayerSpeed = 10.0f;

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

            playerMovement = Collision.gameObject.GetComponent<PlayerMovement>();
            playerMovement.movementSpeed = newPlayerSpeed;

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
