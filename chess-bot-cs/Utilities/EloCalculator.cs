namespace chess_bot_cs.Utilities
{
    public static class EloCalculator
    {
        public static int CalculateNewRating(int playerRating, int opponentRating, double score, double kFactor = 32)
        {
            double expectedScore = 1 / (1 + System.Math.Pow(10, (opponentRating - playerRating) / 400.0));
            return (int)(playerRating + kFactor * (score - expectedScore));
        }

        public static (int, int) CalculateNewRatings(int player1Rating, int player2Rating, double player1Score)
        {
            int newPlayer1Rating = CalculateNewRating(player1Rating, player2Rating, player1Score);
            int newPlayer2Rating = CalculateNewRating(player2Rating, player1Rating, 1 - player1Score);

            return (newPlayer1Rating, newPlayer2Rating);
        }
    }
}