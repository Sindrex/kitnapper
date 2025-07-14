using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    public static List<Vector2> CollisionVectors = new List<Vector2>();
    public Vector2 CollisionVector = new Vector2();
    public bool IsXWall;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        CollisionVector = CameraController.GetMovementVector();
        if (IsXWall)
        {
            CollisionVector.y = 0;
        }
        else
        {
            CollisionVector.x = 0;
        }
        CollisionVectors.Add(CollisionVector);
        //Debug.Log($"Wall OnTriggerStay2D Triggered: Added {CollisionVector}");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //Debug.Log($"Wall OnTriggerExit2D Triggered: Removing {CollisionVector}");
        CollisionVectors.Remove(CollisionVector);
        CollisionVector = new Vector2();
    }
}
