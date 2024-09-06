using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int _id;

    public int Id { get; set; }

    public float moveSpeed = 5f;
    private Rigidbody rb;
    public float moveHorizontal;
    public float moveVertical;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector3(moveHorizontal * moveSpeed, rb.velocity.y, moveVertical * moveSpeed);
    }
}
