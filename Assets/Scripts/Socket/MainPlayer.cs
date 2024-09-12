using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayer : MonoBehaviour
{
    private Client client;

    private void Awake()
    {
        client = GetComponent<Client>();
    }

    private void Update()
    {
        if (!client.canPlay) return;
        client.myPlayer.horizontalInput = Input.GetAxis("Horizontal");
        client.myPlayer.verticalInput = Input.GetAxis("Vertical");
    }
    private void FixedUpdate()
    {
        if (!client.canPlay) return;
        if (client.myPlayer.horizontalInput != 0 || client.myPlayer.verticalInput != 0)
        {
            string result = client.ConvertToMyVector3(client.myPlayer, MyMessageType.POSITION);
            client.SendMessageToServer(result);
        }
    }
}
