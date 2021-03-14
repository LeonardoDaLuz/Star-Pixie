using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour {
	public float DestroyAfterSeconds=5f;
	// Use this for initialization
	void Start () {
		Destroy(gameObject, DestroyAfterSeconds); 
	}
}
