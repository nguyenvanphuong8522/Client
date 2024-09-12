using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int _id;

    public int Id { get; set; }

    public float moveSpeed = 5f;

    public float horizontalInput;

    public float verticalInput;

    
    void FixedUpdate()
    {
        float x = horizontalInput * moveSpeed * Time.fixedDeltaTime;

        float y = verticalInput * moveSpeed * Time.fixedDeltaTime;

        transform.Translate(new Vector3(x, 0, y));
    }

    public void UpdatePosition(Vector3 newPos)
    {
        transform.position = newPos;
    }
}
