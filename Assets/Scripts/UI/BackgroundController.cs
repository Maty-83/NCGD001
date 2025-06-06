using Assets.Helpers;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] AudioClip NormalClip;
    [SerializeField] AudioClip ScaryClip;

    [SerializeField] AudioSource AudioSource;
    [SerializeField] Light2D Light;
    [SerializeField] SpriteRenderer MaskLayer;

    [SerializeField] public float MaxIntensity = 0.65f;
    [SerializeField] public float LengthOfTransition = 2;

    public bool HasFinishedTransition {  get; private set; } = false;

    private bool transitionToNormal = false;
    private bool transitionToScary = false;


    private float actualTransition = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (transitionToNormal)
        {
            actualTransition += Time.deltaTime;
            if (actualTransition >= LengthOfTransition)
            {
                HasFinishedTransition = true;
                actualTransition = LengthOfTransition;
                SetTransitionState();
                transitionToNormal= false;
                return;
            }
            SetTransitionState();

        }

        if (transitionToScary)
        {
            actualTransition -= Time.deltaTime;
            if (actualTransition <= 0)
            {
                HasFinishedTransition = true;
                transitionToScary = true;
                actualTransition = 0;
                SetTransitionState();
                transitionToScary = false;
                return;
            }
            SetTransitionState();
        }
    }


    public void SwitchToScary()
    {
        HasFinishedTransition = false;
        transitionToNormal = false;
        transitionToScary = true;
        actualTransition = LengthOfTransition;
        AudioSource.clip = ScaryClip;
        AudioSource.Play();
    }


    public void SwitchToNormal()
    {
        HasFinishedTransition = false;
        transitionToScary = false;
        transitionToNormal = true;
        actualTransition = 0;
        AudioSource.clip = NormalClip;
        AudioSource.Play();
    }

    private void SetTransitionState()
    {
        var remaped = MathHelper.Remap(actualTransition, LengthOfTransition, 0, 1f, 0f);
        MaskLayer.color = new Color(
            MaskLayer.color.r,
            MaskLayer.color.g,
            MaskLayer.color.b,
            MathHelper.Remap(255 - (255 * remaped ), 255f, 0f, 1f, 0f)
        );

        Light.intensity = 1 * remaped;
    }
}
