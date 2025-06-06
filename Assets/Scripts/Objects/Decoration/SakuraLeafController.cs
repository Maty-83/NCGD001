using UnityEngine;

public class SakuraLeafController : MonoBehaviour
{
    [Header("Leaf Behavior")]
    public float WindMultiplier = 1f;
    public float Lifetime = 10f;

    private Rigidbody2D rb;
    private SakuraFallController fallController;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Destroy(gameObject, Lifetime);
    }

    void FixedUpdate()
    {
        if (fallController != null)
        {
            rb.AddForce(fallController.CurrentWind * WindMultiplier);
        }
    }

    public void SetController(SakuraFallController fallController)
    {
        this.fallController = fallController;
    }
}
