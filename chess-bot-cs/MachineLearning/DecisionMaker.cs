using chess_bot_cs.ChessEngine;

namespace chess_bot_cs.MachineLearning
{
    public class DecisionMaker
    {
        private readonly MoveEvaluator evaluator;

        public DecisionMaker()
        {
            evaluator = new MoveEvaluator();
        }

        public Move DecideBestMove(Game game, int difficultyLevel)
        {
            // Higher difficulty level means deeper search
            int depth = 2 + difficultyLevel;
            return evaluator.SelectBestMove(game, depth);
        }
    }
}