public class PlayerModel
{
    public BallColor Color { get; private set; }
    public int RemainingBalls { get; set; }

    public PlayerModel(BallColor color, int maxBalls = 15)
    {
        Color = color;
        RemainingBalls = maxBalls;
    }
}