using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using MyLibrary;

public class PlayerManager : MonoBehaviour
{
    public List<Player> listOfPlayer;

    [SerializeField] private SpawnManager spawnManager;

    public Player myPlayer;

    [SerializeField] private Client client;

    private void Awake()
    {
        listOfPlayer = new List<Player>();
    }

    public Player CreatePlayer(MessagePosition data)
    {
        if (!HasPlayer(data.id))
        {
            Vector3 newPos = new Vector3(data.Position.x, data.Position.y, data.Position.z);
            Player newPlayer = spawnManager.GetPrefab(0, newPos);
            newPlayer.Id = data.id;

            if (myPlayer == null)
            {
                myPlayer = newPlayer;
            }
            listOfPlayer.Add(newPlayer);
            Debug.LogWarning("Created New Player");
            client.canPlay = true;
            return newPlayer;
        }
        return null;
    }

    public bool HasPlayer(int id)
    {
        return listOfPlayer.Exists(x => x.Id == id);
    }

    public void NotifyPlayerDestroyed(Client socketManager, Player myPlayer)
    {
        MessagePosition newMessagePosition = new MessagePosition { id = myPlayer.Id };
        MyDataRequest dataRequest = new MyDataRequest
        {
            Content = JsonConvert.SerializeObject(newMessagePosition),
            Type = MyMessageType.DESTROY
        };
        client.SendMessageToServer(JsonConvert.SerializeObject(dataRequest));
    }
}
