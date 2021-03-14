using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Points : MonoBehaviour {
	float MaxXSize;
	public int MaxPoints=9000;
	public int currentPoints;
	int _currentPoints;
	public int animationSpeed = 20;
	public static Points i;
	Image imageUI;
	public Text text;
	public GameObject PointsDropUpPrefab;
	// Use this for initialization
	void Awake () {
		i = this;
		imageUI = GetComponent<Image>();
		//text = GetComponentInChildren<Text>();
		MaxXSize = imageUI.rectTransform.sizeDelta.x;
		text.text = currentPoints.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		if(currentPoints != _currentPoints)
        {
			if(_currentPoints< currentPoints)
            {
				_currentPoints = Mathf.Clamp(_currentPoints+ animationSpeed, -999999999, currentPoints);
			}
			else if (_currentPoints < currentPoints)
			{
				_currentPoints = Mathf.Clamp(_currentPoints - animationSpeed, currentPoints, 999999999);
			}
			//Mathf.Lerp(_currentPoints, currentPoints, 1f*Time.deltaTime);
        }

		imageUI.rectTransform.sizeDelta = new Vector2(Mathf.Clamp(((float)_currentPoints / (float)MaxPoints) * MaxXSize, 0f, MaxXSize), imageUI.rectTransform.sizeDelta.y);
		text.text = _currentPoints.ToString();
	}

	public void AddPoints(int points, Vector3 position)
    {
		var obj = (GameObject)Instantiate(PointsDropUpPrefab, position, Quaternion.identity);
		var textChild = obj.GetComponentInChildren<Text>();
		textChild.text = points.ToString();
		currentPoints += points;
	}
}
