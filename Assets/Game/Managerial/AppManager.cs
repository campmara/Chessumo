using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class AppManager : MonoBehaviour 
{
    Coroutine _screenshotRoutine = null;
    const string SCREENSHOT_SAVEFOLDERNAME = "Screenshots";

    private void Awake()
    {
        SetupScreenshotTech();

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

    #region SCREENSHOTS

    private void SetupScreenshotTech()
    {
        #if UNITY_EDITOR
		if( !Directory.Exists( Application.dataPath + "/../" + SCREENSHOT_SAVEFOLDERNAME ) )
		{
			Directory.CreateDirectory( Application.dataPath + "/../" + SCREENSHOT_SAVEFOLDERNAME );
		}
        #endif
    }

    #if UNITY_EDITOR
    private void Update()
    {
        if ( Input.GetKeyDown( KeyCode.S ) )
        {
            HandleScreenShot();
        }
    }

    void HandleScreenShot(int screenshotDetail = 1)
	{
		string screenshotPath = "";

        screenshotPath = Application.dataPath + "/../" + SCREENSHOT_SAVEFOLDERNAME + "/" + "Screenshot_" + System.DateTime.Now.ToString("MM_dd_yy_hhmmss") + ".png";

		ScreenCapture.CaptureScreenshot(screenshotPath, screenshotDetail);
	}
    #endif

    #endregion
}
