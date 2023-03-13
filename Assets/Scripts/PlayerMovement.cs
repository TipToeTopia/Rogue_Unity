
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
using System;

public class PlayerMovement : NetworkBehaviour
{
    [HideInInspector]
    public float movementSpeed;

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
    public bool isInvisible = false;

    public override void OnNetworkSpawn()
    {
        movementSpeed = LevelManager.DEFAULT_PLAYER_SPEED;

        LevelManager.Instance.PlayersInGame.Add(this);

        this.transform.position = new Vector3(transform.position.x, CONSTANT_Y_AXIS, transform.position.z);
    }

    public override void OnNetworkDespawn()
    {
        LevelManager.Instance.PlayersInGame.Remove(this);
    }

    void Update()
    {
        if (!IsOwner || isDowned == true)
        {
            return;
        }

        // if owner and has not been downed by enemy, we can move

        float HorizontalInput = Input.GetAxis(HORIZONTAL_AXIS);
        float VerticalInput = Input.GetAxis(VERTICAL_AXIS);

        Vector3 MovementDirection = new Vector3(HorizontalInput, Y_AXIS_MOVEMENT, VerticalInput);
        MovementDirection.Normalize();

        transform.Translate(MovementDirection * movementSpeed * Time.deltaTime, Space.World);
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

