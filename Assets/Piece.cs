using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;


public class Piece : MonoBehaviour {
	public string Name;
	public string hash;
	public int positionX;
	public int positionY;
	[HideInInspector]
	public int LastComputation;
	public float speed = 3f;
	public Merge[] Merges;
	[HideInInspector]
	public bool willMerge;
	[HideInInspector]
	public Piece willMergeTo;
	[HideInInspector]
	public Piece mergeWillResultTo;
	[HideInInspector]
	public GameObject mergeEffect;
	[HideInInspector]
	Merge CurrentMerge;
	bool initialized;
	// Use this for initialization
	public void Start () {
		if (!initialized)
		{
			MatchManager.i.AddPiece(this);
			initialized = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDrawGizmosSelected()
    {
		if (hash == "")
			NewHash();
    }
	void NewHash()
    {
		var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		var stringChars = new char[20];
		var random = new System.Random();

		for (int i = 0; i < stringChars.Length; i++)
		{
			stringChars[i] = chars[random.Next(chars.Length)];
		}

		hash = new string(stringChars);
	}



	[System.Serializable]
	public class Merge
    {
		public Piece To;
		public Piece Result;
		public GameObject Effect;
		public int Points;
    }

	public bool checkToMerge(Piece piece)
    {
        for (int i = 0; i < Merges.Length; i++)
        {
			if (piece.hash == Merges[i].To.hash)
			{
				willMergeTo = piece;
				willMerge = true;
				mergeWillResultTo = Merges[i].Result;
				mergeEffect = Merges[i].Effect;
				CurrentMerge = Merges[i];
				//Debug.Log("WILL MERGE");
				return true;
			}
		}
		return false;		
    }
	public void merge()
    {
		//MatchManager.i.RemovePiece(willMergeTo);


		var obj = (GameObject)Instantiate(mergeWillResultTo.gameObject, transform.position, Quaternion.identity, transform.parent);
		obj.GetComponent<Piece>().Start();
		InstantiateMergeEffect();
		Destroy(willMergeTo.gameObject);
		Destroy(gameObject);
		Points.i.AddPoints(CurrentMerge.Points, transform.position);
    }

	public void InstantiateMergeEffect()
    {
        if (mergeEffect != null) 
			Instantiate(mergeEffect, willMergeTo.transform.position, Quaternion.identity);

		mergeEffect = null;
	}

}
