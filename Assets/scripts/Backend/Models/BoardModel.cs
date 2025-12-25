public class BoardModel
{
    // Notion仕様: 盤面のデータ
    public BallColor[][,] BallGrid { get; private set; }

    public BoardModel()
    {
        BallGrid = new BallColor[4][,];
        BallGrid[0] = new BallColor[4, 4];
        BallGrid[1] = new BallColor[3, 3];
        BallGrid[2] = new BallColor[2, 2];
        BallGrid[3] = new BallColor[1, 1];
    }

    public void PlaceBall(PylosCoordinate coord, BallColor color)
    {
        BallGrid[coord.Level][coord.X, coord.Y] = color;
    }

    public void RemoveBall(PylosCoordinate coord)
    {
        BallGrid[coord.Level][coord.X, coord.Y] = BallColor.None;
    }

    public bool IsValidCoordinate(PylosCoordinate coord)
    {
        if (coord.Level < 0 || coord.Level > 3) return false;
        int max = 3 - coord.Level;
        return coord.X >= 0 && coord.X <= max && coord.Y >= 0 && coord.Y <= max;
    }

    public BallColor GetColor(PylosCoordinate coord)
    {
        if (!IsValidCoordinate(coord)) return BallColor.None;
        return BallGrid[coord.Level][coord.X, coord.Y];
    }
}