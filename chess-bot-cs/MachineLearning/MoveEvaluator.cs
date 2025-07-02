using chess_bot_cs.ChessEngine;
using System.Collections.Generic;
using System.Linq;

namespace chess_bot_cs.MachineLearning
{
    public class MoveEvaluator
    {
        private readonly Dictionary<PieceType, int> pieceValues = new Dictionary<PieceType, int>
        {
            { PieceType.Pawn, 100 },
            { PieceType.Knight, 320 },
            { PieceType.Bishop, 330 },
            { PieceType.Rook, 500 },
            { PieceType.Queen, 900 },
            { PieceType.King, 20000 }
        };

        private readonly int[,] pawnPositionScores = new int[8, 8]
        {
            { 0,  0,  0,  0,  0,  0,  0,  0 },
            { 50, 50, 50, 50, 50, 50, 50, 50 },
            { 10, 10, 20, 30, 30, 20, 10, 10 },
            { 5,  5, 10, 25, 25, 10,  5,  5 },
            { 0,  0,  0, 20, 20,  0,  0,  0 },
            { 5, -5,-10,  0,  0,-10, -5,  5 },
            { 5, 10, 10,-20,-20, 10, 10,  5 },
            { 0,  0,  0,  0,  0,  0,  0,  0 }
        };

        public int EvaluateBoard(Board board)
        {
            int score = 0;

            for (int file = 0; file < 8; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    var piece = board.GetPieceAt(new Position(file, rank));
                    if (piece != null)
                    {
                        int pieceValue = pieceValues[piece.Type];
                        int positionValue = GetPositionValue(piece, file, rank);

                        score += piece.IsWhite ? (pieceValue + positionValue) : -(pieceValue + positionValue);
                    }
                }
            }

            return score;
        }

        private int GetPositionValue(Piece piece, int file, int rank)
        {
            if (piece.Type == PieceType.Pawn)
            {
                return piece.IsWhite ? pawnPositionScores[rank, file] : pawnPositionScores[7 - rank, file];
            }
            // Add position tables for other pieces
            return 0;
        }

        public Move SelectBestMove(Game game, int depth)
        {
            var legalMoves = game.GetAllLegalMoves();
            if (legalMoves.Count == 0) return null;

            var scoredMoves = legalMoves.Select(move =>
            {
                // Simulate the move
                var originalBoard = game.Board.Squares.Clone() as Piece[,];
                game.Board.MakeMove(move);

                // Evaluate the position
                int score = -EvaluatePosition(game, depth - 1, int.MinValue, int.MaxValue, !game.Board.WhiteToMove);

                // Undo the move
                game.Board.Squares = originalBoard;
                game.Board.WhiteToMove = !game.Board.WhiteToMove;

                return new { Move = move, Score = score };
            }).ToList();

            // Return the move with the highest score
            return scoredMoves.OrderByDescending(x => x.Score).First().Move;
        }

        private int EvaluatePosition(Game game, int depth, int alpha, int beta, bool maximizingPlayer)
        {
            if (depth == 0 || game.State != GameState.InProgress)
            {
                return EvaluateBoard(game.Board);
            }

            var legalMoves = game.GetAllLegalMoves();

            if (maximizingPlayer)
            {
                int maxEval = int.MinValue;
                foreach (var move in legalMoves)
                {
                    // Simulate the move
                    var originalBoard = game.Board.Squares.Clone() as Piece[,];
                    game.Board.MakeMove(move);

                    int eval = EvaluatePosition(game, depth - 1, alpha, beta, false);

                    // Undo the move
                    game.Board.Squares = originalBoard;
                    game.Board.WhiteToMove = !game.Board.WhiteToMove;

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
                    // Simulate the move
                    var originalBoard = game.Board.Squares.Clone() as Piece[,];
                    game.Board.MakeMove(move);

                    int eval = EvaluatePosition(game, depth - 1, alpha, beta, true);

                    // Undo the move
                    game.Board.Squares = originalBoard;
                    game.Board.WhiteToMove = !game.Board.WhiteToMove;

                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (beta <= alpha)
                        break;
                }
                return minEval;
            }
        }
    }
}