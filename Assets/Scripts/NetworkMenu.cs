using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class NetworkMenu : NetworkBehaviour
{
    [SerializeField]
    private Button hostButton;

    [SerializeField]
    private Button clientButton;

    [SerializeField]
    private GameObject Buttons;

    private void Awake()
    {
        hostButton.onClick.AddListener(() =>
        {
            HostButton();
        });

        clientButton.onClick.AddListener(() =>
        {
            ClientButton();
        });
    }

    private void HostButton()
    {
        NetworkManager.Singleton.StartHost();
        Buttons.SetActive(false);

    }

    private void ClientButton()
    {
        NetworkManager.Singleton.StartClient();
        Buttons.SetActive(false);

    } 
}
