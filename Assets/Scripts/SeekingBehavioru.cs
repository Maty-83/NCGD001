using UnityEngine;

public class SeekingBehaviour : MonoBehaviour
{
    public bool enableTracking;
    public float maxSpeed;
    public GameObject trackedGameObject;
    public float maxSeekStartDist;

    private Rigidbody2D ownRB=null;   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ownRB = GetComponent<Rigidbody2D>();
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
        }
        else if (enableTracking)
        {
            Debug.Log("Tracking fail. Either no rigidbody is present, or tracked object not present");
        }
    }
}
