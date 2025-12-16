using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// --- 表に基づいた共有データ定義 ---

// 座標クラス
[System.Serializable]
public struct Position
{
    public int x;
    public int y;
    public int z;

    public Position(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

// ボールの色
public enum BallType
{
    None = -1,
    White = 0,
    Black = 1
}

// プレイヤーのタイプ
public enum PlayerType
{
    CPU = 0,
    Player = 1
}

// ゲームフェーズ
public enum GamePhaseType
{
    PlayerSelect = 0,
    PutBall = 1,
    RetrievalBall = 2,
    GameEnd = 3
}

// 回収権クラス
[System.Serializable]
public class RetrievalBallTurn
{
    public int RetrievalBallTurn_Number; // 回収権の数
}