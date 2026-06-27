using Pylos.Backend.Models;

public interface IAIPlacementAlgorithm
{
    // 置く場所を決定する。置ける場所がなければ Level = -1 を返す
    PylosCoordinate DecidePlacement(BoardModel board, BallColor myColor);
}
