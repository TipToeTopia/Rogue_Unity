using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ReviveTrigger : NetworkBehaviour
{
    private void OnTriggerEnter(Collider Other)
    {
        if (Other.gameObject.tag == LevelManager.PLAYER_TAG)
        {
            if (!IsOwner)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("REVIVE PLAYER");
                
            }
        }
    }
}
