using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BallView : MonoBehaviour
{
    public void MoveTo(Vector3 target)
    {
        transform.position = target;
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}


