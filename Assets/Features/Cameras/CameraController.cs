using System;
using UnityEngine;

/// <summary>
/// Singleton. Controls camera position.
/// </summary>
public class CameraController : MonoBehaviour
{
    public const int CAMERA_Z = -10;
    public GameObject Player;
    public Animator FadeAnimator;
    public string FadeInAnimation;
    public string FadeOutAnimation;
    [SerializeField]
    private bool FollowPlayer = true;

    //smooth transition
    public bool SmoothTransition;
    public Vector3 DesiredPosition;
    public Vector3 CurrentVelocity;
    public float MovementSmoothingValue = 25f;
    public float MinDelta = 0.1f;

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
        if (FollowPlayer)
        {
            this.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, CAMERA_Z);        
        }
        else if (SmoothTransition)
        {
            this.transform.position = Vector3.SmoothDamp(this.transform.position, DesiredPosition, ref CurrentVelocity, MovementSmoothingValue * Time.deltaTime);

            //check if close enough
            var newCameraPosition = this.transform.position;
            var goingLeftWithinMinDelta = Math.Abs(newCameraPosition.x - DesiredPosition.x) <= MinDelta
                                            && Math.Abs(newCameraPosition.y - DesiredPosition.y) <= MinDelta
                                            && Math.Abs(newCameraPosition.z - DesiredPosition.z) <= MinDelta;
            var goingRightWithinMinDelta = Math.Abs(DesiredPosition.x - newCameraPosition.x) <= MinDelta
                                            && Math.Abs(DesiredPosition.y - newCameraPosition.y) <= MinDelta
                                            && Math.Abs(DesiredPosition.z - newCameraPosition.z) <= MinDelta; 
            if(goingLeftWithinMinDelta || goingRightWithinMinDelta)
            {
                SmoothTransition = false;
            }
        }
    }

    public void FadeIn()
    {
        FadeAnimator.Play(FadeInAnimation);
    }

    public void FadeOut()
    {
        FadeAnimator.Play(FadeOutAnimation);
    }

    public void SetPosition(Vector2 position, bool smoothTransition = false)
    {
        FollowPlayer = false;
        if (smoothTransition)
        {
            SmoothTransition = true;
            DesiredPosition = new Vector3(position.x, position.y, CAMERA_Z);
            CurrentVelocity = new Vector3();
        }
        else
        {
            this.gameObject.transform.position = new Vector3(position.x, position.y, CAMERA_Z);
        }
    }

    public void SetFollowPlayer()
    {
        FollowPlayer = true;
        SmoothTransition = false;
    }
}
