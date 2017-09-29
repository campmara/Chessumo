using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressDots : MonoBehaviour 
{
	[Header("Prefabs"), SerializeField] private GameObject dotPrefab;

	[Header("Colors"), SerializeField] private Color inactiveColor;
	[SerializeField] private Color activeColor;

	[Header("Properties"), SerializeField] private float dotDistance = 0.25f;

	private SpriteRenderer[] dots;

	void Awake()
	{
		CreateDots(6);
	}

	public void CreateDots(int numDots)
	{
		dots = new SpriteRenderer[numDots];
		Vector3 pos = transform.position;
        float startX = (-(numDots / 2f) * 0.8333333f) * 0.25f;

		for (int i = 0; i < numDots; i++)
		{
            pos.x = startX + (dotDistance * i);
			GameObject obj = Instantiate(dotPrefab, pos, Quaternion.identity);
			obj.transform.parent = this.transform;
			dots[i] = obj.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
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
