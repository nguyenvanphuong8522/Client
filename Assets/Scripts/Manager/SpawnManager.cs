using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> listOfPrefab;
    public Player GetPrefab(Vector3 newPos, int index = 0)
    {
        
        GameObject newGO = Instantiate(listOfPrefab[0], newPos, Quaternion.identity);
        Player newPlayer = newGO.GetComponent<Player>();
        return newPlayer;
    }
}
