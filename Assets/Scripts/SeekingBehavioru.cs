using Assets.Scripts;
using Assets.Scripts.Entities;
using UnityEngine;

public class SeekingBehaviour : MonoBehaviour
{
    public bool enableTracking;
    public float maxSpeed;
    public GameObject trackedGameObject;
    public float maxSeekStartDist;

    private Rigidbody2D ownRB=null;
    private BasicEnemyController enemyController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyController = gameObject.GetComponent<BasicEnemyController>();
        ownRB = GetComponent<Rigidbody2D>();
        if(trackedGameObject == null )
            trackedGameObject = GameManager.Instance.PlayerController.gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (trackedGameObject != null && !enableTracking)
        {
            if ((transform.position - trackedGameObject.transform.position).magnitude <= maxSeekStartDist)
            {
                enableTracking = true;
            }        
        }
        if (enableTracking) {
            FrameSeek();
        }
    }
    void FrameSeek()
    {
        if (enableTracking && ownRB != null && trackedGameObject != null)
        {
            Vector2 positionDiff = trackedGameObject.transform.position - transform.position;
            ownRB.linearVelocity = positionDiff.normalized*maxSpeed;

            if (enemyController != null)
            {
                if (positionDiff.x < 0)
                    enemyController.IsMovingRight = false;
                else if(positionDiff.x > 0)
                    enemyController.IsMovingRight = true;
            }
        }
        else if (enableTracking)
        {
            Debug.Log("Tracking fail. Either no rigidbody is present, or tracked object not present");
        }
    }
}
