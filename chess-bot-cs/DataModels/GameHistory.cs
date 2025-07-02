using System.Collections.Generic;

namespace chess_bot_cs.DataModels
{
    public class GameHistory
    {
        public List<MoveRecord> Moves { get; set; } = new List<MoveRecord>();
        public string PlayerName { get; set; } = string.Empty;
        public string OpponentName { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public string DatePlayed { get; set; } = string.Empty;
    }
}