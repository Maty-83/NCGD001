using Assets.Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinishController : MonoBehaviour
{
    [SerializeField] private string PortalDestination;
    [SerializeField] private float TriggerDistance = 5f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clip;

    [SerializeField] private GameObject Portal;
    [SerializeField] private float ScaleSpeed = 1f; // Units per second

    [SerializeField] private bool IsTeleporting = true;

    private Transform playerTransform;
    private Vector3 fullScale;
    private Vector3 currentTargetScale;
    private bool isPortalVisible = false;

    void Start()
    {
        playerTransform = GameManager.Instance.PlayerController.transform;
        fullScale = Portal.transform.localScale;
        Portal.transform.localScale = Vector3.zero;
        Portal.SetActive(false);
        currentTargetScale = Vector3.zero;
    }

    void Update()
    {
        if (playerTransform == null) return;
        if (GameManager.Instance.Lairs.Count != 0 ) return;

        float distance = Vector3.Distance(playerTransform.position, transform.position);
        bool shouldScaleUp = distance <= TriggerDistance;
        currentTargetScale = shouldScaleUp ? fullScale : Vector3.zero;

        if (shouldScaleUp && !isPortalVisible)
        {
            Portal.SetActive(true);
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        Portal.transform.localScale = Vector3.MoveTowards(
            Portal.transform.localScale,
            currentTargetScale,
            ScaleSpeed * Time.deltaTime
        );

        if (!shouldScaleUp && Portal.transform.localScale == Vector3.zero && isPortalVisible)
        {
            Portal.SetActive(false);
        }

        isPortalVisible = Portal.activeSelf;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance.Lairs.Count != 0) return;

        var controller = other.gameObject.GetComponent<PlayerController>();
        if (controller != null && PortalDestination != "" && IsTeleporting)
        {
            StartCoroutine(TransitionToScene());
        }
    }

    private IEnumerator TransitionToScene()
    {
        if (GameManager.Instance != null)
        {
            yield return GameManager.Instance.StartCoroutine(GameManager.Instance.FadeMask(0f, 1f));
        }

        SceneManager.LoadScene(PortalDestination);
    }
}