using System;
using System.Collections.Generic;

namespace chess_bot_cs.ChessEngine
{
    public class Game
    {
        private Board _board;
        public Board Board
        {
            get => _board;
            internal set => _board = value; // Changed to internal setter
        }

        public RulesValidator Validator { get; private set; }
        public List<Move> MoveHistory { get; private set; }
        public GameState State { get; private set; }

        public event Action<Move> OnMoveMade;
        public event Action<GameState> OnGameStateChanged;

        public Game()
        {
            _board = new Board();
            _board.InitializeStandardSetup();
            Validator = new RulesValidator(_board);
            MoveHistory = new List<Move>();
            State = GameState.InProgress;
            OnMoveMade = delegate { };
            OnGameStateChanged = delegate { };
        }

        public bool MakeMove(Move move)
        {
            if (State != GameState.InProgress) return false;

            if (Validator.IsMoveLegal(move))
            {
                _board.MakeMove(move);
                MoveHistory.Add(move);

                OnMoveMade?.Invoke(move);

                UpdateGameState();

                return true;
            }

            return false;
        }

        private void UpdateGameState()
        {
            if (Validator.IsCheckmate(_board.WhiteToMove))
            {
                State = _board.WhiteToMove ? GameState.BlackWon : GameState.WhiteWon;
                OnGameStateChanged?.Invoke(State);
            }
            else if (Validator.IsStalemate(_board.WhiteToMove))
            {
                State = GameState.Draw;
                OnGameStateChanged?.Invoke(State);
            }
        }

        public List<Move> GetLegalMoves(Position from)
        {
            return Validator.GetLegalMovesForPiece(from);
        }

        public List<Move> GetAllLegalMoves()
        {
            var allMoves = new List<Move>();
            bool isWhite = _board.WhiteToMove;

            for (int file = 0; file < 8; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    var pos = new Position(file, rank);
                    var piece = _board.GetPieceAt(pos);

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