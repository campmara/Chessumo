using UnityEngine;

public class DebugStartButton : MonoBehaviour 
{
	[SerializeField] Sprite upSprite;
	[SerializeField] Sprite downSprite;

	SpriteRenderer spriteRenderer;

	void Awake()
	{
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		spriteRenderer.sprite = upSprite;
	}

	void OnEnable()
	{
		GameManager.Instance.GrowMe(this.gameObject, 1f);
	}

	void OnMouseDown()
	{
		spriteRenderer.sprite = downSprite;
		GameManager.Instance.BeginGame();
	}

	void OnMouseUp()
	{
		spriteRenderer.sprite = upSprite;
		
	}
}
