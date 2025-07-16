using UnityEngine;

/// <summary>
/// Singleton. Controls camera movement and zoom.
/// </summary>
public class CameraController : MonoBehaviour
{
    public float Speed = 20.0f;
    public int MaxZoom = 30;
    public int MinZoom = 3;

    //Flags
    public bool CanMove = true;

    public Vector2 Position => new Vector2(this.transform.position.x, this.transform.position.y);

    //singleton
    public static CameraController Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        Destroy(this.gameObject);
    }

    private void Update()
    {
        if (!CanMove) return;

        //WASD moving
        var movementVector = GetMovementVector();
        var modifierVectors = WallController.CollisionVectors;
        var modifierVector = new Vector2();
        foreach (var vector in modifierVectors)
        {
            modifierVector += vector;
        }
        modifierVector = Flatten(modifierVector);
        movementVector -= modifierVector;
        movementVector = Flatten(movementVector);
        transform.Translate(movementVector.x * Speed * Time.deltaTime, movementVector.y * Speed * Time.deltaTime, 0.0f);
    }

    public static Vector2 Flatten(Vector2 vector2)
    {
        if (vector2.x > 1) vector2.x = 1;
        if (vector2.x < -1) vector2.x = -1;
        if (vector2.y > 1) vector2.y = 1;
        if (vector2.y < -1) vector2.y = -1;

        return vector2;
    }

    public static Vector2 GetMovementVector()
    {
        var x = 0;
        var y = 0;
        if (InputController.GetInput(InputPurpose.MOVE_LEFT))
        {
            x--;
        }
        if (InputController.GetInput(InputPurpose.MOVE_RIGHT))
        {
            x++;
        }
        if (InputController.GetInput(InputPurpose.MOVE_DOWN))
        {
            y--;
        }
        if (InputController.GetInput(InputPurpose.MOVE_UP))
        {
            y++;
        }
        return new Vector2(x, y);
    }
}
