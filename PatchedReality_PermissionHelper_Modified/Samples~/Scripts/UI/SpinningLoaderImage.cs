using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningLoaderImage : MonoBehaviour
{
    
    public float degreesPerSecond = 45f;
    // Update is called once per frame
    void Update()
    {
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.z += Time.deltaTime*degreesPerSecond;
        transform.eulerAngles = currentRotation;
    }
}
