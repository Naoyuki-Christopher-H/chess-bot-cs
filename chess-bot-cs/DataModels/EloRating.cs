namespace chess_bot_cs.DataModels
{
    public class EloRating
    {
        public string PlayerId { get; set; }
        public int Rating { get; set; }
        public int GamesPlayed { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
    }
}