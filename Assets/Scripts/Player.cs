using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int _id;

    public int Id { get; set; }

    public float moveSpeed = 5f;
    public float moveHorizontal;
    public float moveVertical;


    void FixedUpdate()
    {
        transform.Translate(new Vector3(moveHorizontal * moveSpeed * Time.fixedDeltaTime, 0, moveVertical * moveSpeed * Time.fixedDeltaTime));
    }
}
