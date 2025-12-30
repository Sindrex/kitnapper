using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FinaleMomentEventController : EventBase
{
    public string Id;
    public GameObject PlayerTeleportPosition;
    public Animator CreditsAnimator;
    public string CreditsAnimation;
    public float CreditsSeconds;
    public float FadeWaitSeconds;
    public Vector3 PlayerStartPosition;

    // Start is called before the first frame update
    public void Setup()
    {
        Id = gameObject.name;
    }

    public override void Activate(bool activateNextEvent)
    {
        CLogger.Log($"Event \"{Id}\" activated!");
        //fade out game
        //credits
        //TP player, lock movement
        //Play credit sequence (one animation)
        //Names of all characters the player has interacted with, rest as ???.
        StartCoroutine(StartCreditsAnimation());
    }

    IEnumerator<WaitForSeconds> StartCreditsAnimation()
    {
        CameraController.Instance.FadeOut();
        yield return new WaitForSeconds(FadeWaitSeconds);
        CameraController.Instance.FadeIn();
        PlayerController.Instance.CanMove = false;
        var newPos = PlayerTeleportPosition.transform.position;
        CameraController.Instance.gameObject.transform.position = new Vector3(newPos.x, newPos.y, -10);
        CameraController.Instance.Player.SetActive(false);
        CreditsAnimator.Play(CreditsAnimation);
        yield return new WaitForSeconds(CreditsSeconds);
        CameraController.Instance.FadeOut();
        yield return new WaitForSeconds(FadeWaitSeconds);
        CameraController.Instance.FadeIn();
        PlayerController.Instance.CanMove = true;
        CameraController.Instance.gameObject.transform.localPosition = PlayerStartPosition;
        CameraController.Instance.Player.SetActive(true);
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        string customName = "EventGizmo.png";
        Gizmos.DrawIcon(transform.position, customName, true);
    }
#endif
}
