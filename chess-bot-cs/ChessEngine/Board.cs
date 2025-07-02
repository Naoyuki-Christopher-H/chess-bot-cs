using System;
using System.Collections.Generic;

namespace chess_bot_cs.ChessEngine
{
    public class Board
    {
        public Piece[,] Squares { get; private set; }
        public bool WhiteToMove { get; set; }
        public int HalfMoveClock { get; set; }
        public int FullMoveNumber { get; set; }
        public string CastlingRights { get; set; }
        public Position EnPassantTarget { get; set; }

        public Board()
        {
            Squares = new Piece[8, 8];
            WhiteToMove = true;
            HalfMoveClock = 0;
            FullMoveNumber = 1;
            CastlingRights = "KQkq";
            EnPassantTarget = null;
        }

        public void InitializeStandardSetup()
        {
            // Place pawns
            for (int file = 0; file < 8; file++)
            {
                Squares[file, 1] = new Piece(PieceType.Pawn, false);
                Squares[file, 6] = new Piece(PieceType.Pawn, true);
            }

            // Place rooks
            Squares[0, 0] = new Piece(PieceType.Rook, false);
            Squares[7, 0] = new Piece(PieceType.Rook, false);
            Squares[0, 7] = new Piece(PieceType.Rook, true);
            Squares[7, 7] = new Piece(PieceType.Rook, true);

            // Place knights
            Squares[1, 0] = new Piece(PieceType.Knight, false);
            Squares[6, 0] = new Piece(PieceType.Knight, false);
            Squares[1, 7] = new Piece(PieceType.Knight, true);
            Squares[6, 7] = new Piece(PieceType.Knight, true);

            // Place bishops
            Squares[2, 0] = new Piece(PieceType.Bishop, false);
            Squares[5, 0] = new Piece(PieceType.Bishop, false);
            Squares[2, 7] = new Piece(PieceType.Bishop, true);
            Squares[5, 7] = new Piece(PieceType.Bishop, true);

            // Place queens
            Squares[3, 0] = new Piece(PieceType.Queen, false);
            Squares[3, 7] = new Piece(PieceType.Queen, true);

            // Place kings
            Squares[4, 0] = new Piece(PieceType.King, false);
            Squares[4, 7] = new Piece(PieceType.King, true);
        }

        public Piece GetPieceAt(Position position)
        {
            if (position.File < 0 || position.File > 7 || position.Rank < 0 || position.Rank > 7)
                return null;

            return Squares[position.File, position.Rank];
        }

        public void MakeMove(Move move)
        {
            // Handle move logic including special moves like castling, en passant, promotion
            var piece = GetPieceAt(move.From);
            Squares[move.From.File, move.From.Rank] = null;
            Squares[move.To.File, move.To.Rank] = piece;

            // Handle special cases
            if (piece.Type == PieceType.Pawn && Math.Abs(move.To.Rank - move.From.Rank) == 2)
            {
                EnPassantTarget = new Position(move.From.File, (move.From.Rank + move.To.Rank) / 2);
            }
            else
            {
                EnPassantTarget = null;
            }

            // Handle castling
            if (piece.Type == PieceType.King && Math.Abs(move.To.File - move.From.File) == 2)
            {
                // Castle move - move the rook
                int rookFile = move.To.File > move.From.File ? 7 : 0;
                int newRookFile = move.To.File > move.From.File ? 5 : 3;
                var rook = GetPieceAt(new Position(rookFile, move.From.Rank));
                Squares[rookFile, move.From.Rank] = null;
                Squares[newRookFile, move.From.Rank] = rook;
            }

            // Handle promotion
            if (move.Promotion != PieceType.None)
            {
                Squares[move.To.File, move.To.Rank] = new Piece(move.Promotion, piece.IsWhite);
            }

            // Update castling rights if king or rook moves
            UpdateCastlingRights(move, piece);

            // Update move counters
            if (piece.Type == PieceType.Pawn || move.CapturedPiece != null)
            {
                HalfMoveClock = 0;
            }
            else
            {
                HalfMoveClock++;
            }

            if (!WhiteToMove)
            {
                FullMoveNumber++;
            }

            WhiteToMove = !WhiteToMove;
        }

        private void UpdateCastlingRights(Move move, Piece piece)
        {
            if (piece.Type == PieceType.King)
            {
                if (piece.IsWhite)
                {
                    CastlingRights = CastlingRights.Replace("K", "").Replace("Q", "");
                }
                else
                {
                    CastlingRights = CastlingRights.Replace("k", "").Replace("q", "");
                }
            }
            else if (piece.Type == PieceType.Rook)
            {
                if (piece.IsWhite)
                {
                    if (move.From.File == 0 && move.From.Rank == 0)
                        CastlingRights = CastlingRights.Replace("Q", "");
                    else if (move.From.File == 7 && move.From.Rank == 0)
                        CastlingRights = CastlingRights.Replace("K", "");
                }
                else
                {
                    if (move.From.File == 0 && move.From.Rank == 7)
                        CastlingRights = CastlingRights.Replace("q", "");
                    else if (move.From.File == 7 && move.From.Rank == 7)
                        CastlingRights = CastlingRights.Replace("k", "");
                }
            }
        }

        public string GetFenString()
        {
            // Implementation to generate FEN string
            // This would create a string representing the current board state
            // according to Forsyth-Edwards Notation
            return "";
        }
    }
}