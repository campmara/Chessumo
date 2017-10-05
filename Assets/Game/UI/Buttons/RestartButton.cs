using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RestartButton : MonoBehaviour 
{
    [SerializeField] private SpriteRenderer touchDesignator;

    private const float MIN_S = 4f;
    private const float MAX_S = 5f;

    private const float FOLLOW_SPEED = 5f;

    private bool isTakingInput = false;

    private Tween fadeTween = null;
    private Tween pulseTween = null;

    Vector3 followDir;

    private void Awake()
    {
        touchDesignator.gameObject.SetActive(false);
        touchDesignator.color = new Color(touchDesignator.color.r, touchDesignator.color.g, touchDesignator.color.b, 1f);
        touchDesignator.transform.localScale = new Vector3(MIN_S, MIN_S, MIN_S);
    }

    private void ResetEffect()
    {
        touchDesignator.color = new Color(touchDesignator.color.r, touchDesignator.color.g, touchDesignator.color.b, 1f);
        touchDesignator.transform.localScale = new Vector3(MIN_S, MIN_S, MIN_S);
    }

    public void SetReadyForInput(bool isReady)
    {
        Debug.Log("Ready for input? " + isReady);

        pulseTween.Kill();
        fadeTween.Kill();

        if (isReady)
        {
            touchDesignator.gameObject.SetActive(true);

            touchDesignator.transform.position = new Vector3(Constants.I.GridOffsetX, Constants.I.GridOffsetY, 0f);
            ResetEffect();

			pulseTween = touchDesignator.transform.DOScale(Vector3.one * MAX_S, 2f).SetLoops(-1);
            fadeTween = touchDesignator.DOFade(0f, 2f).SetLoops(-1);
        }

        isTakingInput = isReady;
        GetComponent<BoxCollider>().enabled = isReady;
    }

	void OnMouseDown()
	{
        if (!isTakingInput) return;

        pulseTween.Kill();
        fadeTween.Kill();

        pulseTween = touchDesignator.transform.DOScale(Vector3.one * MIN_S, 0.5f);
        fadeTween = touchDesignator.DOFade(1f, 0.5f);
	}

	void OnMouseUp()
	{
        if (!isTakingInput) return;

		touchDesignator.transform.DOScale(Vector3.one * MAX_S, 1f);
        touchDesignator.DOFade(0f, 1f).OnComplete(() => touchDesignator.gameObject.SetActive(false));

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			if (hit.collider.gameObject == this.gameObject)
			{
				OnPress();
			}
		}
	}

	void OnPress() 
    {
		GameManager.Instance.BeginGame();
    }
}
