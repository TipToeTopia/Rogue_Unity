using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DoorScript : ItemType
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        levelManager.completedLevelTwo.OnValueChanged += levelManager.YouWin;
        levelManager.completedLevelOne.OnValueChanged += levelManager.LevelTwo;
    }

    public override void OnCollisionEnter(Collision Collision)
    {
        if (TouchedPlayer(Collision) == true)
        {
            if (UpdatePlayerIdentification == levelManager.keyCollected.Value)
            {
                if (levelManager.completedLevelOne.Value == false)
                {
                    levelManager.LevelTwoServerRpc();
                }
                else if (levelManager.completedLevelTwo.Value == false)
                {
                    levelManager.YouWinServerRpc();
                }

                DespawnObjectServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    protected override void DespawnObjectServerRpc()
    {
        levelManager.completedLevelOne.OnValueChanged -= levelManager.LevelTwo;
        levelManager.completedLevelTwo.OnValueChanged -= levelManager.YouWin;
        base.DespawnObjectServerRpc();
    }

}
