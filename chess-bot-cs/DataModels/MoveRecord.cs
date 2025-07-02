namespace chess_bot_cs.DataModels
{
    public class MoveRecord
    {
        public int MoveNumber { get; set; }
        public string WhiteMove { get; set; }
        public string BlackMove { get; set; }
        public string FenAfterMove { get; set; }
    }
}