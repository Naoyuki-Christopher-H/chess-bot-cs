using System;
using System.Collections.Generic;

namespace chess_bot_cs.ChessEngine
{
    public class Game
    {
        public Board Board { get; private set; }
        public RulesValidator Validator { get; private set; }
        public List<Move> MoveHistory { get; private set; }
        public GameState State { get; private set; }

        public event Action<Move> OnMoveMade;
        public event Action<GameState> OnGameStateChanged;

        public Game()
        {
            Board = new Board();
            Board.InitializeStandardSetup();
            Validator = new RulesValidator(Board);
            MoveHistory = new List<Move>();
            State = GameState.InProgress;
        }

        public bool MakeMove(Move move)
        {
            if (State != GameState.InProgress) return false;

            if (Validator.IsMoveLegal(move))
            {
                Board.MakeMove(move);
                MoveHistory.Add(move);

                OnMoveMade?.Invoke(move);

                // Check game state after move
                UpdateGameState();

                return true;
            }

            return false;
        }

        private void UpdateGameState()
        {
            if (Validator.IsCheckmate(Board.WhiteToMove))
            {
                State = Board.WhiteToMove ? GameState.BlackWon : GameState.WhiteWon;
                OnGameStateChanged?.Invoke(State);
            }
            else if (Validator.IsStalemate(Board.WhiteToMove))
            {
                State = GameState.Draw;
                OnGameStateChanged?.Invoke(State);
            }
            // Add other draw conditions (50-move rule, repetition, insufficient material)
        }

        public List<Move> GetLegalMoves(Position from)
        {
            return Validator.GetLegalMovesForPiece(from);
        }

        public List<Move> GetAllLegalMoves()
        {
            var allMoves = new List<Move>();
            bool isWhite = Board.WhiteToMove;

            for (int file = 0; file < 8; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    var pos = new Position(file, rank);
                    var piece = Board.GetPieceAt(pos);

                    if (piece != null && piece.IsWhite == isWhite)
                    {
                        allMoves.AddRange(Validator.GetLegalMovesForPiece(pos));
                    }
                }
            }

            return allMoves;
        }
    }

    public enum GameState
    {
        InProgress,
        WhiteWon,
        BlackWon,
        Draw
    }
}