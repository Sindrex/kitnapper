using UnityEngine;

/// <summary>
/// Singleton. Controls camera position.
/// </summary>
public class CameraController : MonoBehaviour
{
    public GameObject Player;
    public Animator FadeAnimator;
    public string FadeInAnimation;
    public string FadeOutAnimation;
    public bool FollowPlayer = true;

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
            this.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, -10);        
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
}
