using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateUndefinedly : MonoBehaviour
{
    public float speed = 50f;


    // Update is called once per frame
    void Update()
    {
        var euler = transform.eulerAngles;
        transform.eulerAngles = new Vector3(euler.x, euler.y, euler.z + speed * Time.deltaTime);
    }
}
