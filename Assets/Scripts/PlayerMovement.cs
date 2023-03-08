
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
using System;

public class PlayerMovement : NetworkBehaviour
{
    private const float MOVEMENT_SPEED = 10.0f;
    private const float Y_AXIS_MOVEMENT = 0.0f;

    private const string HORIZONTAL_AXIS = "Horizontal";
    private const string VERTICAL_AXIS = "Vertical";

    private const float CONSTANT_Y_AXIS = 1.0f;

    private Vector3 spawnOffset = new Vector3(2.0f, 0.0f, 0.0f);

    [SerializeField]
    private NetworkObject keyObject;

    [SerializeField]
    private GameObject reviveCanvas;

    public bool isDowned = false;

    public override void OnNetworkSpawn()
    {
        LevelManager.Instance.PlayersInGame.Add(this);

        this.transform.position = new Vector3(transform.position.x, CONSTANT_Y_AXIS, transform.position.z);
    }

    public override void OnNetworkDespawn()
    {
        LevelManager.Instance.PlayersInGame.Remove(this);
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (isDowned == true)
        {
            return;
        }

        // if owner and has not been downed by enemy, we can move

        float HorizontalInput = Input.GetAxis(HORIZONTAL_AXIS);
        float VerticalInput = Input.GetAxis(VERTICAL_AXIS);

        Vector3 MovementDirection = new Vector3(HorizontalInput, Y_AXIS_MOVEMENT, VerticalInput);
        MovementDirection.Normalize();

        transform.Translate(MovementDirection * MOVEMENT_SPEED * Time.deltaTime, Space.World);

        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnKeyServerRpc();

        }
    }

    [ServerRpc]
    void SpawnKeyServerRpc()
    {

        NetworkObject KeyObject = Instantiate(keyObject, transform.position + spawnOffset, Quaternion.identity);
        KeyObject.Spawn();

    }

    [ClientRpc]
    public void EnableOrDisableReviveUIClientRpc(bool TrueOrFalse)
    {
        reviveCanvas.SetActive(TrueOrFalse);
    }

    private void OnCollisionEnter(Collision Collision)
    {
        if (Collision.gameObject.tag == LevelManager.PLAYER_TAG)
        {
            if (isDowned == true)
            {
                EnableOrDisableReviveUIClientRpc(false);
                isDowned = false;
            }
        }
    }

}

