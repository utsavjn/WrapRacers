using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Xengine.UnityExtensions;

public class UIController : MonoBehaviour {

	// store reference to canvas to set as parent
	private Transform canvas;
	private Animator gameOverScreenAnimator;
    private Animator countDown;

    void Initialize()
    {
        canvas = GameObject.Find(Common.strMainCanvas).transform;
        gameOverScreenAnimator = canvas.FindChild(Common.strGameOverScreen).GetComponent<Animator>();
        countDown = canvas.FindChild(Common.strCountDown).GetComponent<Animator>();
    }
    
    private void Start()
    {
        Initialize();
    }

    public void ShowGameOver()
	{
		gameOverScreenAnimator.Animate("show");
	}

	public void HideGameOver()
	{
		gameOverScreenAnimator.Animate("hide");
	}

    public void ShowCoutDown(string stranim, bool anim = false)
    {
        Initialize();
        Debug.Log("countDown.Animate(Common.strCountDown);");
        //countDown.GetComponent<Image>().enabled = true;
        countDown.Animate(stranim, anim);
    }
}
