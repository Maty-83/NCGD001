using Assets.Scripts;
using UnityEngine;

public class SoulsDropController : MonoBehaviour
{
    private float Score = 0;

    public void Init(float storedScore)
    {
       Score = storedScore;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.Score = Score;
            Destroy(gameObject);

            GameManager.PlayerDeathPosition = Vector3.negativeInfinity;
            GameManager.DroppedScore = 0;
        }
    }
}
