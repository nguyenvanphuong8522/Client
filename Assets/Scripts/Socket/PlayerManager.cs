using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Shared.Messages;

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
        if (!HasPlayer(data.Id))
        {
            Vector3 newPos = new Vector3(data.Position.X, data.Position.Y, data.Position.Z);
            Player newPlayer = spawnManager.GetPrefab(newPos);
            newPlayer.Id = data.Id;
            newPlayer.gameObject.name = $"Player{data.Id}";

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

    public Player GetPlayer(int id)
    {
        return listOfPlayer.Find(x => x.Id == id);
    }


    public void RemovePlayer(int id)
    {
        Player player = GetPlayer(id);
        DestroyPlayer(player);
        listOfPlayer.Remove(player);
    }

    public void DestroyAllPlayers()
    {
        foreach (Player player in listOfPlayer)
        {
            Destroy(player.gameObject);
        }
        listOfPlayer.Clear();
    }

    public void DestroyPlayer(Player player)
    {
        Destroy(player.gameObject);
    }
}
