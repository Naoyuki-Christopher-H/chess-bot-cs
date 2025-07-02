using System;
using System.Collections.Generic;
using System.Linq;
using chess_bot_cs.ChessEngine;

namespace chess_bot_cs.MachineLearning
{
    public class MoveEvaluator
    {
        public Move? SelectBestMove(Game game, int depth)
        {
            var legalMoves = game.GetAllLegalMoves();
            if (legalMoves.Count == 0) return null;

            var scoredMoves = legalMoves.Select(move =>
            {
                var originalBoard = game.Board.Clone();
                game.MakeMove(move);
                int score = -EvaluatePosition(game, depth - 1, int.MinValue, int.MaxValue, !game.Board.WhiteToMove);
                game.Board = originalBoard;
                return new { Move = move, Score = score };
            }).ToList();

            return scoredMoves.OrderByDescending(x => x.Score).First().Move;
        }

        private int EvaluatePosition(Game game, int depth, int alpha, int beta, bool maximizingPlayer)
        {
            if (depth == 0 || game.State != GameState.InProgress)
                return EvaluateBoard(game.Board);

            var legalMoves = game.GetAllLegalMoves();

            if (maximizingPlayer)
            {
                int maxEval = int.MinValue;
                foreach (var move in legalMoves)
                {
                    var originalBoard = game.Board.Clone();
                    game.MakeMove(move);
                    int eval = EvaluatePosition(game, depth - 1, alpha, beta, false);
                    game.Board = originalBoard;

                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha)
                        break;
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                foreach (var move in legalMoves)
                {
                    var originalBoard = game.Board.Clone();
                    game.MakeMove(move);
                    int eval = EvaluatePosition(game, depth - 1, alpha, beta, true);
                    game.Board = originalBoard;

                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (beta <= alpha)
                        break;
                }
                return minEval;
            }
        }

        private int EvaluateBoard(Board board)
        {
            int score = 0;
            for (int file = 0; file < 8; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    var piece = board.GetPieceAt(new Position(file, rank));
                    if (piece != null)
                    {
                        int value = piece.Type switch
                        {
                            PieceType.Pawn => 100,
                            PieceType.Knight => 320,
                            PieceType.Bishop => 330,
                            PieceType.Rook => 500,
                            PieceType.Queen => 900,
                            PieceType.King => 20000,
                            _ => 0
                        };
                        score += piece.IsWhite ? value : -value;
                    }
                }
            }
            return score;
        }
    }
}