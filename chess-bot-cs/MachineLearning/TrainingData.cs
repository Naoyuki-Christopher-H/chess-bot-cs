using System.Collections.Generic;

namespace chess_bot_cs.MachineLearning
{
    public class TrainingData
    {
        public List<GameRecord> GameRecords { get; set; }

        public TrainingData()
        {
            GameRecords = new List<GameRecord>();
        }

        public void AddGameRecord(GameRecord record)
        {
            GameRecords.Add(record);
        }
    }

    public class GameRecord
    {
        public List<MoveRecord> Moves { get; set; }
        public GameResult Result { get; set; }

        public GameRecord()
        {
            Moves = new List<MoveRecord>();
        }
    }

    public class MoveRecord
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Piece { get; set; }
        public string CapturedPiece { get; set; }
        public double Evaluation { get; set; }
    }

    public enum GameResult
    {
        WhiteWin,
        BlackWin,
        Draw
    }
}