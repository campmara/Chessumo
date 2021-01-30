using UnityEngine;

public class Constants : ScriptableObjectSingleton<Constants> {
    [Header("Gameplay")]
    public IntVector2 GridSize = new IntVector2(5, 5);
    public int StartingPieceCount = 7;
    public int ScoreOneAmount = 1;
    public int ScoreTwoAmount = 4;
    public int ScoreThreeAmount = 9;

    [Header("Grid Specifics")]
    public float GridOffsetX = 0f;
    public float GridOffsetY = -0.5f;

    [Header("Pieces")]
    public float PieceMoveTime = 0.25f;

    [Header("Start Button")]
    public float StartButtonRaisedY = -4f;
    public float StartButtonLoweredY = -7f;
    public float StartButtonTweenTime = 0.75f;

    [Header("Score")]
    public int MaxScore = 2147483647;
    public float ScoreRaisedY = 6.5f;
    public float ScoreLoweredY = 0f;
    public float ScoreTweenTime = 1f;
}