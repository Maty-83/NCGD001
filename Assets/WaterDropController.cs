using UnityEngine;

public class WaterDropController : MonoBehaviour
{
    [Header("Drop Behavior")]
    public float WindMultiplier = 1f;
    public float Lifetime = 10f;

    private Rigidbody2D rb;
    private StormTrapController fallController;

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

    public void SetController(StormTrapController fallController)
    {
        this.fallController = fallController;
    }
}
