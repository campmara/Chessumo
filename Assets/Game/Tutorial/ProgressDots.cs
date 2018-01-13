using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressDots : MonoBehaviour 
{
	[Header("Prefabs"), SerializeField] private GameObject dotPrefab;

	[Header("Colors"), SerializeField] private Color inactiveColor;
	[SerializeField] private Color activeColor;

	[Header("Properties"), SerializeField] private float dotDistance = 0.25f;

	private Image[] dots;

	void Awake()
	{
		CreateDots(5);
	}

	public void CreateDots(int numDots)
	{
		dots = new Image[numDots];
		Vector2 pos = Vector2.zero;
        float startX = (-(numDots / 2f) * 0.8333333f) * dotDistance;

		for (int i = 0; i < numDots; i++)
		{
            pos.x = startX + (dotDistance * i);
			GameObject obj = Instantiate(dotPrefab);
			obj.transform.parent = this.transform;
			obj.GetComponent<RectTransform>().anchoredPosition = pos;
			dots[i] = obj.GetComponent(typeof(Image)) as Image;
			dots[i].color = inactiveColor;
		}

		dots[0].color = activeColor;
	}

	public void UpdateDots(int index)
	{
        if (index == 0)
        {
            // Clear and reset dots to 0.
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].color = inactiveColor;
            }
            dots[index].color = activeColor;
        }
		else if (index <= dots.Length - 1)
		{
			dots[index - 1].color = inactiveColor;
			dots[index].color = activeColor;
		}
	}
}
