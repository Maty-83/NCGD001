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
    public bool OnlyRespectHorizontal;
    public float MovementVelocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
