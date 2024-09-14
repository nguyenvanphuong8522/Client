using MyLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

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
        client.playerManager.myPlayer.horizontalInput = Input.GetAxis("Horizontal");
        client.playerManager.myPlayer.verticalInput = Input.GetAxis("Vertical");
    }
    private void FixedUpdate()
    {
        if (!client.canPlay) return;
        if (client.playerManager.myPlayer.horizontalInput != 0 || client.playerManager.myPlayer.verticalInput != 0)
        {
            Vector3 newPos = client.playerManager.myPlayer.transform.position;
            MessagePosition messagePosition = new MessagePosition(client.playerManager.myPlayer.Id, new MyVector3(newPos.x, newPos.y, newPos.z));
            byte[] data = MessagePackSerializer.Serialize(messagePosition);
            byte[] result = MyUtility.SendMessageConverted(MyMessageType.POSITION, data);
            client.socketManager.SendMessageToServer(result);
        }
    }
}
