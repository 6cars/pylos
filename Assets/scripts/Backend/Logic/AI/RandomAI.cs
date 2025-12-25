using UnityEngine;

public class RandomAI : IAIAlgorithm
{
    public PylosCoordinate GetMove(BoardModel board, BallColor myColor)
    {
        Debug.Log("AI‚ªl‚¦’†...");
        return new PylosCoordinate(0, 0, 0);
    }
}