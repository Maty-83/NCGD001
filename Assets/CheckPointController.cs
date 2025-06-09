using Assets.Scripts;
using System.Collections;
using UnityEngine;

public class CheckPointController : MonoBehaviour
{
    [SerializeField] private GameObject Flames;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clip;
    [SerializeField] private float ScaleSpeed = 1f; // units per second

    public bool isActivated = false;
    private Vector3 fullFlameScale;

    private void Start()
    {
        if (Flames != null)
        {
            fullFlameScale = Flames.transform.localScale;
            Flames.transform.localScale = Vector3.zero;
            Flames.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActivated) return;

        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            if(GameManager.CheckPointController != null)
            {
                GameManager.CheckPointController.isActivated = false;
                GameManager.CheckPointController.DeactivateFlames();
            }

            GameManager.CheckPointController = this;
            GameManager.LastCheckpoint = new Vector3(transform.position.x, transform.position.y, 0);
            if (Flames != null)
            {
                Flames.SetActive(true);
                StartCoroutine(ScaleFlames(fullFlameScale));
            }

            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }

            isActivated = true;
        }
    }

    public void DeactivateFlames()
    {
        if (Flames != null && Flames.activeSelf)
        {
            StartCoroutine(ScaleFlames(Vector3.zero, deactivateOnZero: true));
        }
    }

    private IEnumerator ScaleFlames(Vector3 targetScale, bool deactivateOnZero = false)
    {
        while (Flames.transform.localScale != targetScale)
        {
            Flames.transform.localScale = Vector3.MoveTowards(
                Flames.transform.localScale,
                targetScale,
                ScaleSpeed * Time.deltaTime
            );

            yield return null;
        }

        if (deactivateOnZero && targetScale == Vector3.zero)
        {
            Flames.SetActive(false);
        }
    }
}