using UnityEngine;

public class ShootingBehaviour : MonoBehaviour
{
    public bool enableTracking;
    public float maxSpeed;
    public GameObject trackedGameObject;
    public float maxSeekStartDist;
    public float maxRotationPerSec;//By how many degrees can the item rotate IN RADIANS

    private Rigidbody2D ownRB=null;   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ownRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
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
            var velVect = ownRB.linearVelocity;
            Vector2 positionDiff = trackedGameObject.transform.position - transform.position;

            float signedAngle = Vector2.SignedAngle(velVect, positionDiff);//Huh, this is useful.
            var deltaT = Time.deltaTime;
            float rotatableAngle = Mathf.Clamp(signedAngle, -maxRotationPerSec * deltaT, maxRotationPerSec * deltaT);
            var rotatedVec = util_rotate(velVect, signedAngle);
            ownRB.linearVelocity.Set(rotatedVec.x*maxSpeed, rotatedVec.y*maxSpeed);

        }
        else if (enableTracking)
        {
            Debug.Log("Tracking fail. Either no rigidbody is present, or tracked object not present");
        }
    }
    //Simple tool to rotate a vector by a given amount. Written because it's a few lines and I couldn't find the function.
    private Vector2 util_rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }
}
