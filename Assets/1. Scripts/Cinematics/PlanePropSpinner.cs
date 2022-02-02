using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePropSpinner : MonoBehaviour
{   
    [SerializeField] float rotationSpeed = 45f;
    
    
    float z;

    
    void Update()
    {
        z += rotationSpeed * Time.deltaTime;
        transform.Rotate(0, 0, z);
    }
}
