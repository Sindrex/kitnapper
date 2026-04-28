using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Reaction
{
    Exclamation,
    Question,
    Happy,
    Sad,
    Angry
}

public class ReactionController : MonoBehaviour
{
    public Animator ReactionAnimator;
    public string IdleAnim = "Idle";
    public string ExclamationAnimation;
    public string QuestionAnimation;
    public string HappyAnimation;
    public string SadAnimation;
    public string AngryAnimation;

    public GameObject ExclamationGameObject;
    public GameObject QuestionGameObject;
    public GameObject HappyGameObject;
    public GameObject SadGameObject;
    public GameObject AngryGameObject;

    public AudioLabel ReactionSFX;

    public float Timing = 1f;

    // Start is called before the first frame update
    void Start()
    {
        ExclamationGameObject.SetActive(false);
        QuestionGameObject.SetActive(false);
        HappyGameObject.SetActive(false);
        SadGameObject.SetActive(false);
        AngryGameObject.SetActive(false);
    }

    public void StartReaction(Reaction reaction)
    {
        CLogger.Log($"StartReaction with reaction: {reaction}");
        AudioManager.Instance.PlaySFXClip(ReactionSFX);
        switch(reaction)
        {
            case Reaction.Exclamation:
                ExclamationGameObject.SetActive(true);
                ReactionAnimator.Play(ExclamationAnimation);
                break;
            case Reaction.Question:
                QuestionGameObject.SetActive(true);
                ReactionAnimator.Play(QuestionAnimation);
                break;
            case Reaction.Happy:
                HappyGameObject.SetActive(true);
                ReactionAnimator.Play(HappyAnimation);
                break;
            case Reaction.Sad:
                SadGameObject.SetActive(true);
                ReactionAnimator.Play(SadAnimation);
                break;
            case Reaction.Angry:
                AngryGameObject.SetActive(true);
                ReactionAnimator.Play(AngryAnimation);
                break;
        }

        StartCoroutine(StopReaction());
    }

    IEnumerator StopReaction()
    {
        yield return new WaitForSeconds(Timing);
        ExclamationGameObject.SetActive(false);
        QuestionGameObject.SetActive(false);
        HappyGameObject.SetActive(false);
        SadGameObject.SetActive(false);
        AngryGameObject.SetActive(false);
        ReactionAnimator.Play(IdleAnim);
    }
}
