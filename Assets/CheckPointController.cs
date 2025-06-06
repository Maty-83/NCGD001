using Assets.Scripts;
using UnityEngine;

public class CheckPointController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            GameManager.LastCheckpoint = new Vector3 (transform.position.x, transform.position.y, 0);
        }
    }
}
