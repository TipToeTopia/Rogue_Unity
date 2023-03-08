using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

// inherited base class for item types

public class ItemType : NetworkBehaviour
{

    protected LevelManager levelManager;
    protected PlayerMovement playerMovement;

    protected string UpdatePlayerIdentification;

    private const float DESTROY_DELAY = 0.2f;
    
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
        StartCoroutine(DestroyDelay());
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

    private IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(DESTROY_DELAY);
        Destroy(gameObject);
    }
}
