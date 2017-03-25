using UnityEngine;

public class Constants : ScriptableObjectSingleton<Constants>
{
	[Header("Grid")]
	public IntVector2 GRID_SIZE = new IntVector2(5, 5);
	public float GRID_OFFSET_X = 0f;
	public float GRID_OFFSET_Y = -0.5f;

	[Header("Start Button")]
	public float QUIT_RAISED_Y = -4f;
	public float QUIT_LOWERED_Y = -7f;
	public float QUIT_TWEEN_TIME = 0.75f;

	[Header("Score")]
	public int SCORE_MAX = 2147483647;
	public float SCORE_RAISED_Y = 6.5f;
	public float SCORE_LOWERED_Y = 0f;
	public float SCORE_TWEEN_TIME = 1f;

	[Header("Pieces")]
	public int STARTING_PIECES = 7;
	public float PIECE_MOVE_TIME = 0.25f;
}