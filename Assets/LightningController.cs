using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningController : MonoBehaviour
{
    public List<AudioClip> LightningStrikes;
    public AudioSource AudioSource;
    public Animator Animator;

    public void Init(AudioSource source)
    {
        AudioSource = source;
        OnDeath();
    }

    public virtual void OnDeath(bool destroy = true)
    {
        if (LightningStrikes.Count != 0)
        {
            AudioSource.PlayOneShot(LightningStrikes[Random.Range(0, LightningStrikes.Count)]);
        }
        StartCoroutine(HandleDeath(destroy));
    }

    private IEnumerator HandleDeath(bool destroy)
    {
        yield return new WaitForSeconds(GetAnimationLength("LightningStrike"));

        if (destroy)
            Destroy(gameObject);
    }

    float GetAnimationLength(string animationName)
    {
        AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
        int stateHash = stateInfo.shortNameHash;

        foreach (var clip in Animator.runtimeAnimatorController.animationClips)
        {
            if (Animator.StringToHash(clip.name) == stateHash)
            {
                return clip.length;
            }
        }

        return 0f;
    }
}
