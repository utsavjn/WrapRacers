using UnityEngine;
using System.Collections;
using Xengine.UnityExtensions;

public class UIController : MonoBehaviour {

	// store reference to canvas to set as parent
	public Transform canvas;
	public Animator gameOverScreenAnimator;

	public void ShowGameOver()
	{
		gameOverScreenAnimator.Animate("show");
	}

	public void HideGameOver()
	{
		gameOverScreenAnimator.Animate("hide");
	}
}
