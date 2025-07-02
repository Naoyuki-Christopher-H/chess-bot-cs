namespace chess_bot_cs.MachineLearning
{
    public class TrainingData
    {
        public List<GameRecord> GameRecords { get; set; } = new List<GameRecord>();
    }

    public class GameRecord
    {
        public List<MoveRecord> Moves { get; set; } = new List<MoveRecord>();
        public GameResult Result { get; set; }
    }

    public class MoveRecord
    {
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string Piece { get; set; } = string.Empty;
        public string CapturedPiece { get; set; } = string.Empty;
        public double Evaluation { get; set; }
    }

    public enum GameResult
    {
        WhiteWin,
        BlackWin,
        Draw
    }
}