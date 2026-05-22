using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

/// <summary>
/// Singleton. Controls some player interactions.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public Vector2 Position => new Vector2(this.transform.position.x, this.transform.position.y);

    public List<GameObject> InteractAnimationFrames;
    public float InteractAnimationTick;

    //movement
    public bool CanMove = true;
    public Rigidbody2D PlayerRigidbody;
    public const float Speed = 15000f;
    public float CurrentSpeed;
    public Animator PlayerAnimator;
    public string IdleAnimatorState;
    public string MoveAnimatorState;
    public bool IsMoving;
    public bool MoveAnimPlaying;

    //Catnip anim
    public GameObject CatnipAnimatorObject;
    public Animator CatnipAnimator;
    public string CatnipFastAnimatorState;
    public string CatnipSlowAnimatorState;

    //SFX
    public List<AudioLabel> WalkSFX = new List<AudioLabel> 
    { 
        AudioLabel.Walk1, AudioLabel.Walk2, AudioLabel.Walk3 
    };

    //singleton
    public static PlayerController Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            CurrentSpeed = Speed;
            return;
        }
        Destroy(this.gameObject);
    }

    void FixedUpdate()
    {
        var movementVector = GetMovementVector(CanMove);
        if(movementVector.x != 0 || movementVector.y != 0)
        {
            IsMoving = true;
            if (!MoveAnimPlaying)
            {
                PlayerAnimator.Play(MoveAnimatorState);
                MoveAnimPlaying = true;
            }
        }
        else
        {
            PlayerAnimator.Play(IdleAnimatorState);
            IsMoving = false;
            MoveAnimPlaying = false;
        }
        PlayerRigidbody.velocity = movementVector * CurrentSpeed * Time.deltaTime;
    }

    public static Vector2 GetMovementVector(bool canMove)
    {
        if (!canMove)
        {
            return new Vector2(0, 0);
        }

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

    public void ShowInteractHint()
    {
        StartCoroutine(StartInteractHint());
    }

    public void StopShowInteractHint()
    {
        StopAllCoroutines();
        foreach (var frame in InteractAnimationFrames)
        {
            frame.SetActive(false);
        }
    }

    public void PlayWalkSFX()
    {
        var randomNumber = new System.Random().Next(0, WalkSFX.Count - 1);
        var sfxClip = WalkSFX[randomNumber];
        AudioManager.Instance?.PlaySFXClip(sfxClip);
    }

    IEnumerator<WaitForSeconds> StartInteractHint()
    {
        //CLogger.Log("Started StartInteractHint!");
        var frame1 = InteractAnimationFrames[0];
        var frame2 = InteractAnimationFrames[1];
        var frame3 = InteractAnimationFrames[2];
        frame1.SetActive(true);
        frame2.SetActive(false);
        frame3.SetActive(false);
        yield return new WaitForSeconds(InteractAnimationTick);
        frame1.SetActive(false);
        frame2.SetActive(true);
        frame3.SetActive(false);
        yield return new WaitForSeconds(InteractAnimationTick);
        frame1.SetActive(false);
        frame2.SetActive(false);
        frame3.SetActive(true);
    }

    public void SetCatnipAnim(bool fast)
    {
        CatnipAnimatorObject.SetActive(true);
        if (fast)
        {
            CatnipAnimator.Play(CatnipFastAnimatorState);
        }
        else
        {
            CatnipAnimator.Play(CatnipSlowAnimatorState);
        }
    }

    public void StopCatnipAnim()
    {
        CatnipAnimatorObject.SetActive(false);
    }
}