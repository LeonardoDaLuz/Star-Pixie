using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleMovement : MonoBehaviour
{
    public AnimationCurve MovementCurve;
    public float time;
    public Vector3 initialScale;
    public float RandomizeTime = 1f;
    float animationLength;
    // Use this for initialization
    void Start()
    {
        initialScale = transform.localScale;
        time = Random.Range(0f, RandomizeTime);

        var key = MovementCurve.keys;
        if (key != null && key.Length > 0)
        {
            animationLength = key[key.Length - 1].time;
        }
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > animationLength)
        {
            time = 0f;
        }
        transform.localScale = initialScale * MovementCurve.Evaluate(time);
    }




}
