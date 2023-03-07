using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LevelKey : ItemType
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        levelManager.keyCollected.OnValueChanged += levelManager.UpdateKeyText;

    }

    public override void OnCollisionEnter(Collision Collision)
    {
        if (TouchedPlayer(Collision) == true)
        {
            levelManager.SetKeyTextServerRpc(UpdatePlayerIdentification);
            DespawnObjectServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    protected override void DespawnObjectServerRpc()
    {
        levelManager.keyCollected.OnValueChanged -= levelManager.UpdateKeyText;
        base.DespawnObjectServerRpc();
    }

}
