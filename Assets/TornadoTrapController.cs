using UnityEngine;

public class TornadoTrapController : MonoBehaviour
{
    [SerializeField] GameObject Tornado;
    [SerializeField] RectTransform TornadoTrapLayer;
    [SerializeField] AudioSource audioSource;
    [SerializeField] bool IsHeadingLeft;
    [SerializeField] float Speed;
    [SerializeField] float Interval;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= Interval)
        {
            timer = 0f;
            SpawnTornado();
        }
    }

    void SpawnTornado()
    {
        Vector3 spawnPosition = TornadoTrapLayer.position;
        Vector2 trapSize = TornadoTrapLayer.rect.size;

        // Get left or right edge of the RectTransform in world space
        Vector3 worldLeft = TornadoTrapLayer.TransformPoint(new Vector3(-trapSize.x / 2f, 0, 0));
        Vector3 worldRight = TornadoTrapLayer.TransformPoint(new Vector3(trapSize.x / 2f, 0, 0));

        Vector3 startPosition = IsHeadingLeft ? worldRight : worldLeft;

        GameObject tornadoInstance = Instantiate(Tornado, startPosition, Quaternion.identity, TornadoTrapLayer);

        TornadoController controller = tornadoInstance.GetComponent<TornadoController>();
        controller.InitTrap(worldLeft.x, worldRight.x, IsHeadingLeft, Speed);

        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
