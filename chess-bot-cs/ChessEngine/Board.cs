using System;

namespace chess_bot_cs.ChessEngine
{
    public class Board
    {
        private Piece[,] squares;
        public Piece[,] Squares
        {
            get => squares;
            internal set => squares = value;
        }

        public bool WhiteToMove { get; set; }
        public int HalfMoveClock { get; set; }
        public int FullMoveNumber { get; set; }
        public string CastlingRights { get; set; }
        public Position? EnPassantTarget { get; set; } // Made nullable

        public Board()
        {
            squares = new Piece[8, 8];
            WhiteToMove = true;
            HalfMoveClock = 0;
            FullMoveNumber = 1;
            CastlingRights = "KQkq";
            EnPassantTarget = null;
        }

        public void InitializeStandardSetup()
        {
            // Clear the board
            squares = new Piece[8, 8];

            // Place pawns
            for (int file = 0; file < 8; file++)
            {
                squares[file, 1] = new Piece(PieceType.Pawn, false);
                squares[file, 6] = new Piece(PieceType.Pawn, true);
            }

            // Place rooks
            squares[0, 0] = new Piece(PieceType.Rook, false);
            squares[7, 0] = new Piece(PieceType.Rook, false);
            squares[0, 7] = new Piece(PieceType.Rook, true);
            squares[7, 7] = new Piece(PieceType.Rook, true);

            // Place knights
            squares[1, 0] = new Piece(PieceType.Knight, false);
            squares[6, 0] = new Piece(PieceType.Knight, false);
            squares[1, 7] = new Piece(PieceType.Knight, true);
            squares[6, 7] = new Piece(PieceType.Knight, true);

            // Place bishops
            squares[2, 0] = new Piece(PieceType.Bishop, false);
            squares[5, 0] = new Piece(PieceType.Bishop, false);
            squares[2, 7] = new Piece(PieceType.Bishop, true);
            squares[5, 7] = new Piece(PieceType.Bishop, true);

            // Place queens
            squares[3, 0] = new Piece(PieceType.Queen, false);
            squares[3, 7] = new Piece(PieceType.Queen, true);

            // Place kings
            squares[4, 0] = new Piece(PieceType.King, false);
            squares[4, 7] = new Piece(PieceType.King, true);
        }

        public Piece? GetPieceAt(Position position)
        {
            if (position.File < 0 || position.File > 7 || position.Rank < 0 || position.Rank > 7)
                return null;

            return squares[position.File, position.Rank];
        }

        public void MakeMove(Move move)
        {
            if (move == null) return;

            var piece = GetPieceAt(move.From);
            if (piece == null) return;

            // Handle capture
            move.CapturedPiece = GetPieceAt(move.To);

            // Move the piece
            squares[move.From.File, move.From.Rank] = null!;
            squares[move.To.File, move.To.Rank] = piece;

            // Handle special cases
            if (piece.Type == PieceType.Pawn)
            {
                // Reset half-move clock on pawn moves
                HalfMoveClock = 0;

                // Handle en passant
                if (Math.Abs(move.To.Rank - move.From.Rank) == 2)
                {
                    EnPassantTarget = new Position(move.From.File, (move.From.Rank + move.To.Rank) / 2);
                }
                else if (EnPassantTarget != null && move.To.Equals(EnPassantTarget))
                {
                    // Capture the en passant pawn
                    int capturedPawnRank = piece.IsWhite ? move.To.Rank + 1 : move.To.Rank - 1;
                    squares[move.To.File, capturedPawnRank] = null!;
                    move.CapturedPiece = new Piece(PieceType.Pawn, !piece.IsWhite);
                }

                // Handle promotion
                if ((move.To.Rank == 0 || move.To.Rank == 7) && move.Promotion != PieceType.None)
                {
                    squares[move.To.File, move.To.Rank] = new Piece(move.Promotion, piece.IsWhite);
                }
            }
            else
            {
                // Increment half-move clock for non-pawn moves
                if (move.CapturedPiece == null)
                {
                    HalfMoveClock++;
                }
                else
                {
                    HalfMoveClock = 0;
                }
            }

            // Handle castling
            if (piece.Type == PieceType.King && Math.Abs(move.To.File - move.From.File) == 2)
            {
                // Determine rook positions
                int rookFromFile = move.To.File > move.From.File ? 7 : 0;
                int rookToFile = move.To.File > move.From.File ? 5 : 3;
                int rank = piece.IsWhite ? 0 : 7;

                // Move the rook
                var rook = squares[rookFromFile, rank];
                squares[rookFromFile, rank] = null!;
                squares[rookToFile, rank] = rook!;
            }

            // Update castling rights if king or rook moves
            UpdateCastlingRights(move, piece);

            // Update full move number after black's move
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
            string fen = "";

            // Piece placement
            for (int rank = 0; rank < 8; rank++)
            {
                int emptyCount = 0;

                for (int file = 0; file < 8; file++)
                {
                    var piece = squares[file, rank];
                    if (piece == null)
                    {
                        emptyCount++;
                    }
                    else
                    {
                        if (emptyCount > 0)
                        {
                            fen += emptyCount;
                            emptyCount = 0;
                        }
                        fen += piece.ToString();
                    }
                }

                if (emptyCount > 0)
                {
                    fen += emptyCount;
                }

                if (rank < 7)
                {
                    fen += "/";
                }
            }

            // Side to move
            fen += " " + (WhiteToMove ? "w" : "b");

            // Castling availability
            fen += " " + (string.IsNullOrEmpty(CastlingRights) ? "-" : CastlingRights);

            // En passant
            fen += " " + (EnPassantTarget?.ToString() ?? "-");

            // Halfmove clock
            fen += " " + HalfMoveClock;

            // Fullmove number
            fen += " " + FullMoveNumber;

            return fen;
        }

        public Board Clone()
        {
            var clone = new Board
            {
                squares = (Piece[,])squares.Clone(),
                WhiteToMove = WhiteToMove,
                HalfMoveClock = HalfMoveClock,
                FullMoveNumber = FullMoveNumber,
                CastlingRights = CastlingRights,
                EnPassantTarget = EnPassantTarget != null ? new Position(EnPassantTarget.File, EnPassantTarget.Rank) : null
            };
            return clone;
        }
    }
}