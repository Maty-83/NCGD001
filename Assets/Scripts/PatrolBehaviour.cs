using System.Collections.Generic;
using UnityEngine;

public enum OnPatrolEndBehaviour
{
    Loop, Bounce, Kill
}

public class PatrolBehaviour : MonoBehaviour
{
    public List<Vector3> PatrolPoints;//Should this be vectors or Transforms?
    public List<float> PointDelays;
    public OnPatrolEndBehaviour OnPatrolEnd;
    public bool OnlyRespectHorizontal;//Only respects X coordinate of the movement, ignoring Y
    public float MovementVelocity;
    public int CurrentPatrolPoint = 0;
    private bool MoveInReverse = false;
    private float WaitTimeRemaining = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //We do position updates
        //TL;DR: We move towards the position (Or X coord) of the current patrol point. On reaching it, we wait if any is present.

        //Process any waiting we might be experiencing.
        float moveTimeDelta = 0;
        WaitTimeRemaining-=Time.deltaTime;
        if (WaitTimeRemaining < 0)
        {
            moveTimeDelta = -WaitTimeRemaining;
            WaitTimeRemaining = 0;
        }
        else return;
        //Calculate distance to next point.
        float dist;
        if (OnlyRespectHorizontal)
        {
            dist = Mathf.Abs(transform.position.x - PatrolPoints[CurrentPatrolPoint].x);
        }
        else 
        {
            dist=Vector3.Magnitude(transform.position-PatrolPoints[CurrentPatrolPoint]);
        }

        //Move
        if (dist > moveTimeDelta * MovementVelocity)
        {
            ExecMove(moveTimeDelta*MovementVelocity);
        }
        else
        {
            //If distance less than time*velocity, use only appropriate time
            moveTimeDelta-=dist/MovementVelocity;
            ExecMove(dist);

            if (MoveInReverse) CurrentPatrolPoint--;
            else CurrentPatrolPoint++;

            //if reached patrol point, advance patrol and if end of patrol, execute ProcessEndPoint.
            if (CurrentPatrolPoint>=PatrolPoints.Count || CurrentPatrolPoint < 0)
            {
                ProcessEndPoint();
                if (OnPatrolEnd == OnPatrolEndBehaviour.Kill) return;
            }
            //Add wait time back
            WaitTimeRemaining += PointDelays[CurrentPatrolPoint];
            if (WaitTimeRemaining >= moveTimeDelta)
            {
                WaitTimeRemaining -= moveTimeDelta;
                return;
            }
            else
            {
                moveTimeDelta-=WaitTimeRemaining;
                ExecMove(moveTimeDelta*MovementVelocity);
            }
            //if any time still remains, use it to move to next patrol point

        }
    }
    void ExecMove(float dist)
    {
        if (OnlyRespectHorizontal)
        {
            if (transform.position.x > PatrolPoints[CurrentPatrolPoint].x)
            {
                MoveInDir(Vector3.right, dist);
            }
            else
            {
                MoveInDir(Vector3.left, dist);
            }
        }
        else
        {
            MoveInDir(PatrolPoints[CurrentPatrolPoint] - transform.position, dist);
        }
    }

    void MoveInDir(Vector3 dir, float distance)
    {
        transform.position += dir * distance/Mathf.Abs(dir.magnitude);//A slightly convoluted way of allowing movement in any direction without normalization potentially aliasing vectors.
    }

    void ProcessEndPoint()
    {
        switch (OnPatrolEnd)
        {
            case OnPatrolEndBehaviour.Loop:
                CurrentPatrolPoint = 0;
                break;
            case OnPatrolEndBehaviour.Bounce:                
                if (MoveInReverse)
                {
                    CurrentPatrolPoint=1;//We were at zero
                }
                else
                {
                    CurrentPatrolPoint=PatrolPoints.Count-2; //We were at max, this is the one before max.
                }
                MoveInReverse = !MoveInReverse;
                break;
            case OnPatrolEndBehaviour.Kill:
                GameObject.Destroy(gameObject);
                break;
            default:
                Debug.Log("Incorrect endpoint, terminating gameobject");
                GameObject.Destroy(gameObject);
                break ;
            }
    }
}
