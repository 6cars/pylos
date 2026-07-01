using Pylos.Backend.Models;

public interface IAIPlacementAlgorithm
{
    // 置くアクション（手元からか、盤面移動か）を決定する
    PylosMove DecidePlacement(BoardModel board, BallColor myColor, int remainingBalls);
}
