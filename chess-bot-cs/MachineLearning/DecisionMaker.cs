using chess_bot_cs.ChessEngine;

namespace chess_bot_cs.MachineLearning
{
    public class DecisionMaker
    {
        private readonly MoveEvaluator evaluator = new MoveEvaluator();

        public Move? DecideBestMove(Game game, int difficultyLevel)
        {
            if (game == null) return null;

            int depth = 2 + difficultyLevel;
            return evaluator.SelectBestMove(game, depth);
        }
    }
}