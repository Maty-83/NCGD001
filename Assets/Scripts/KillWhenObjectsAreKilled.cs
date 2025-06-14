using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class KillWhenObjectsAreKilled : MonoBehaviour
{
    public List<GameObject> MustStayAlive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = MustStayAlive.Count - 1; i >= 0; i--)
        {
            if(MustStayAlive[i] == null)
            {
                MustStayAlive.RemoveAt(i);
            }
        }
        if( MustStayAlive.Count == 0)
        {
            Destroy(gameObject);
        }
    }
}
