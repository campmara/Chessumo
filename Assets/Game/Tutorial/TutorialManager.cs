using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour 
{
    public static TutorialManager Instance = null;

	[SerializeField] private TutorialPage[] pages;

	private ProgressDots dots;

	private int pageIndex = 0;
	private bool fingerLifted = true;

    public void SetVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);

        if (isVisible) 
        {
            Restart();
        }
    }

    private void Restart()
    {
        pageIndex = 0;
        dots.UpdateDots(pageIndex);

        for (int i = 0; i < pages.Length; i++)
        {
			// Disable all pages
			pages[i].gameObject.SetActive(false);
        }
		
		// Enable the first.
		pages[pageIndex].gameObject.SetActive(true);
    }

    private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

		dots = GetComponentInChildren(typeof(ProgressDots)) as ProgressDots;
		
		// Like reading any novel, we must turn to the first page.
		Initialize();
	}

	private void Update()
	{
#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0))
		{
			HandlePageTurn();
		}
#else
		if (fingerLifted && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
		{
			fingerLifted = false;

			HandlePageTurn();
		}

		if (!fingerLifted && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)
		{
			fingerLifted = true;
		}
#endif
	}

	private void Initialize()
	{
		pages[pageIndex].gameObject.SetActive(true);
	}

	private void HandlePageTurn()
	{
		// Increment the page index.
		pageIndex++;

		// Check for tutorial end.
		if (pageIndex > pages.Length - 1)
		{
			HandleEndTutorial(); 
			return;
		}
		// Disable the previous page.
		pages[pageIndex - 1].gameObject.SetActive(false);
		// Enable the new one.
		pages[pageIndex].gameObject.SetActive(true);

		// Update the dots.
		dots.UpdateDots(pageIndex);
	}

	private void HandleEndTutorial()
	{
        SaveDataManager.Instance.OnTutorialComplete();
        GameManager.Instance.SetVisibility(true);
        GameManager.Instance.restartButton.SetReadyForInput(true);
        SetVisibility(false);
	}
}
