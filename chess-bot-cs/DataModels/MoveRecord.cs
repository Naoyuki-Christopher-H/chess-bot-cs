namespace chess_bot_cs.DataModels
{
    public class MoveRecord
    {
        public int MoveNumber { get; set; }
        public string WhiteMove { get; set; } = string.Empty;
        public string BlackMove { get; set; } = string.Empty;
        public string FenAfterMove { get; set; } = string.Empty;
    }
}