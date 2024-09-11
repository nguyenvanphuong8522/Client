using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> listOfPrefab;

    public Player GetPrefab(int index, Vector3 newPos)
    {

        GameObject newGO = Instantiate(listOfPrefab[index], newPos, Quaternion.identity);
        Player newPlayer = newGO.GetComponent<Player>();
        newPlayer.Id = Random.Range(-100, 100);
        return newPlayer;
    }
}
