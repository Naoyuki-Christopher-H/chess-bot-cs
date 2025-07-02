using System.Collections.Generic;

namespace chess_bot_cs.DataModels
{
    public class GameHistory
    {
        public List<MoveRecord> Moves { get; set; }
        public string PlayerName { get; set; }
        public string OpponentName { get; set; }
        public string Result { get; set; }
        public string DatePlayed { get; set; }

        public GameHistory()
        {
            Moves = new List<MoveRecord>();
        }
    }
}