using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton. Controls some player interactions.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public List<GameObject> InteractAnimationFrames;
    public float InteractAnimationTick;

    //singleton
    public static PlayerController Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        Destroy(this.gameObject);
    }

    void Start()
    {
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
}