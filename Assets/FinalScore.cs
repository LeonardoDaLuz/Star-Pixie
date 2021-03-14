using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalScore : MonoBehaviour {
	public StarsRate[] Stars;
	public static FinalScore i;
	public Text scoreText;
	public Text pointsText;
	public float scoreCounterSpeed=50f;
	public Animator animator;
	[System.Serializable]
	public class StarsRate
    {
		public Image image;
		public int MinimumPoints = 1000;
	}
	// Use this for initialization
	void Start () {
		i = this;
		animator=GetComponent<Animator>();
		gameObject.SetActive(false);

	}
	
	public void ShowFinalScore()
    {
		MatchManager.i.disableControls = true;
		gameObject.SetActive(true);
		StartCoroutine(FinalScoreCo());
    }
	// Update is called once per frame
	void Update () {
		
	}

	public void Replay()
    {
		if (gameObject.activeInHierarchy)
		{
			animator.SetTrigger("exit");
			Invoke("WaitAnimationToReplay", 2f);
		} else
        {
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
    }

	public void WaitAnimationToReplay()
    {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	public void Exit()
    {
		if (gameObject.activeInHierarchy)
		{
			animator.SetTrigger("exit");
			Invoke("WaitAnimationToQuit", 2f);
		} else
        {
			Application.Quit();
		}
    }
	public void WaitAnimationToQuit()
	{
		Application.Quit();
	}
	IEnumerator FinalScoreCo()
    {
		scoreText.CrossFadeAlpha(0f, 0f, true);
		pointsText.text = "";

		for (int i = 0; i < Stars.Length; i++)
        {
            if (Points.i.currentPoints > Stars[i].MinimumPoints)
            {
				Stars[i].image.gameObject.SetActive(true);
				Stars[i].image.CrossFadeAlpha(1f, 0.25f, true);
				Stars[i].image.GetComponent<Animator>().SetTrigger("glance");
				yield return new WaitForSeconds(1f);
			}
        }
		scoreText.CrossFadeAlpha(1f, 1f, true);
		float _pointsCount = 0f;
		while(_pointsCount < Points.i.currentPoints - 1)
        {
			_pointsCount = Mathf.Lerp(_pointsCount, Points.i.currentPoints, scoreCounterSpeed * Time.deltaTime);
			pointsText.text = Mathf.CeilToInt(_pointsCount).ToString();
			yield return new WaitForEndOfFrame();
		}
		


	}
}
