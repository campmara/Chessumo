using UnityEngine;
using DG.Tweening;

public class TopUIBar : MonoBehaviour 
{
	void OnEnable()
	{
		GameManager.Instance.GrowMeFromSlit(this.gameObject, 1f, Ease.OutBounce);
	}
}
