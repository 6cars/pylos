using Pylos.Backend.Models;

public interface IAIRetrievalAlgorithm
{
    // 回収する自分の球の位置を決定する。回収しない（スキップ）の場合は Level = -1 を返す
    PylosCoordinate DecideRetrieval(BoardModel board, BallColor myColor);
}
