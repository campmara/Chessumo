using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour 
{
    private void Awake()
    {
        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
    {
        // Load tutorial, then set active / inactive if user has completed already.
        yield return StartCoroutine(AsyncLoad(1));
        TutorialManager.Instance.SetVisibility(false);

        // Load game, then set active / inactive.
        yield return StartCoroutine(AsyncLoad(2));
        GameManager.Instance.SetVisibility(false);

        if (SaveDataManager.Instance.IsTutorialComplete())
        {
            GameManager.Instance.SetVisibility(true);
        }
        else
        {
            GameManager.Instance.restartButton.SetReadyForInput(false);
            TutorialManager.Instance.SetVisibility(true);
        }
    }

    private IEnumerator AsyncLoad(int sceneIndex)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
		op.allowSceneActivation = false;

		while (!op.isDone)
		{
			// Loading completed
			if (Mathf.Abs(op.progress - 0.9f) < Mathf.Epsilon)
			{
				op.allowSceneActivation = true;
			}

			yield return null;
		}
    }

    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
