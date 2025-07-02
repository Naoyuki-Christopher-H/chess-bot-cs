namespace chess_bot_cs.ChessEngine
{
    public class RulesValidator
    {
        private readonly Board board;

        public RulesValidator(Board board)
        {
            this.board = board ?? throw new ArgumentNullException(nameof(board));
        }

        public bool IsMoveLegal(Move move)
        {
            if (move == null || move.From == null || move.To == null)
                return false;

            var piece = board.GetPieceAt(move.From);
            if (piece == null || piece.IsWhite != board.WhiteToMove)
                return false;

            var legalMoves = GetLegalMovesForPiece(move.From);
            return legalMoves.Any(m => m.To.Equals(move.To) &&
                (piece.Type != PieceType.Pawn ||
                 (move.To.Rank != 0 && move.To.Rank != 7) ||
                 move.Promotion != PieceType.None));
        }

        public List<Move> GetLegalMovesForPiece(Position position)
        {
            var piece = board.GetPieceAt(position);
            if (piece == null) return new List<Move>();

            var moves = new List<Move>();
            switch (piece.Type)
            {
                case PieceType.Pawn:
                    GetPawnMoves(position, piece.IsWhite, moves);
                    break;
                case PieceType.Knight:
                    GetKnightMoves(position, piece.IsWhite, moves);
                    break;
                case PieceType.Bishop:
                    GetBishopMoves(position, piece.IsWhite, moves);
                    break;
                case PieceType.Rook:
                    GetRookMoves(position, piece.IsWhite, moves);
                    break;
                case PieceType.Queen:
                    GetQueenMoves(position, piece.IsWhite, moves);
                    break;
                case PieceType.King:
                    GetKingMoves(position, piece.IsWhite, moves);
                    break;
            }

            return FilterMovesThatLeaveKingInCheck(moves, piece.IsWhite);
        }

        private List<Move> FilterMovesThatLeaveKingInCheck(List<Move> moves, bool isWhite)
        {
            var validMoves = new List<Move>();
            var originalBoard = board.Clone();

            foreach (var move in moves)
            {
                board.MakeMove(move);
                if (!IsKingInCheck(isWhite))
                {
                    validMoves.Add(move);
                }
                board.Squares = originalBoard.Squares;
                board.WhiteToMove = originalBoard.WhiteToMove;
                board.CastlingRights = originalBoard.CastlingRights;
                board.EnPassantTarget = originalBoard.EnPassantTarget;
            }

            return validMoves;
        }

        private void GetPawnMoves(Position position, bool isWhite, List<Move> moves)
        {
            int direction = isWhite ? -1 : 1;
            int startRank = isWhite ? 6 : 1;

            // Single move forward
            var forwardOne = new Position(position.File, position.Rank + direction);
            if (board.GetPieceAt(forwardOne) == null)
            {
                moves.Add(new Move(position, forwardOne));

                // Double move from starting position
                if (position.Rank == startRank)
                {
                    var forwardTwo = new Position(position.File, position.Rank + 2 * direction);
                    if (board.GetPieceAt(forwardTwo) == null)
                    {
                        moves.Add(new Move(position, forwardTwo));
                    }
                }
            }

            // Captures
            for (int fileOffset = -1; fileOffset <= 1; fileOffset += 2)
            {
                if (position.File + fileOffset >= 0 && position.File + fileOffset < 8)
                {
                    var capturePos = new Position(position.File + fileOffset, position.Rank + direction);
                    var targetPiece = board.GetPieceAt(capturePos);

                    if (targetPiece != null && targetPiece.IsWhite != isWhite)
                    {
                        moves.Add(new Move(position, capturePos));
                    }

                    // En passant
                    if (board.EnPassantTarget != null &&
                        capturePos.Equals(board.EnPassantTarget))
                    {
                        moves.Add(new Move(position, capturePos));
                    }
                }
            }
        }

        private void GetKnightMoves(Position position, bool isWhite, List<Move> moves)
        {
            int[] knightJumps = { -2, -1, 1, 2 };

            foreach (int fileJump in knightJumps)
            {
                foreach (int rankJump in knightJumps)
                {
                    if (Math.Abs(fileJump) != Math.Abs(rankJump))
                    {
                        int newFile = position.File + fileJump;
                        int newRank = position.Rank + rankJump;

                        if (newFile >= 0 && newFile < 8 && newRank >= 0 && newRank < 8)
                        {
                            var targetPos = new Position(newFile, newRank);
                            var targetPiece = board.GetPieceAt(targetPos);

                            if (targetPiece == null || targetPiece.IsWhite != isWhite)
                            {
                                moves.Add(new Move(position, targetPos));
                            }
                        }
                    }
                }
            }
        }

        private void GetSlidingMoves(Position position, bool isWhite, List<Move> moves, (int, int)[] directions)
        {
            foreach (var (fileStep, rankStep) in directions)
            {
                for (int distance = 1; distance < 8; distance++)
                {
                    int newFile = position.File + fileStep * distance;
                    int newRank = position.Rank + rankStep * distance;

                    if (newFile < 0 || newFile >= 8 || newRank < 0 || newRank >= 8)
                        break;

                    var targetPos = new Position(newFile, newRank);
                    var targetPiece = board.GetPieceAt(targetPos);

                    if (targetPiece == null)
                    {
                        moves.Add(new Move(position, targetPos));
                    }
                    else
                    {
                        if (targetPiece.IsWhite != isWhite)
                        {
                            moves.Add(new Move(position, targetPos));
                        }
                        break;
                    }
                }
            }
        }

        private void GetBishopMoves(Position position, bool isWhite, List<Move> moves)
        {
            GetSlidingMoves(position, isWhite, moves, new[] { (-1, -1), (-1, 1), (1, -1), (1, 1) });
        }

        private void GetRookMoves(Position position, bool isWhite, List<Move> moves)
        {
            GetSlidingMoves(position, isWhite, moves, new[] { (-1, 0), (1, 0), (0, -1), (0, 1) });
        }

        private void GetQueenMoves(Position position, bool isWhite, List<Move> moves)
        {
            GetSlidingMoves(position, isWhite, moves,
                new[] { (-1, -1), (-1, 1), (1, -1), (1, 1), (-1, 0), (1, 0), (0, -1), (0, 1) });
        }

        private void GetKingMoves(Position position, bool isWhite, List<Move> moves)
        {
            // Normal king moves
            for (int fileOffset = -1; fileOffset <= 1; fileOffset++)
            {
                for (int rankOffset = -1; rankOffset <= 1; rankOffset++)
                {
                    if (fileOffset == 0 && rankOffset == 0) continue;

                    int newFile = position.File + fileOffset;
                    int newRank = position.Rank + rankOffset;

                    if (newFile >= 0 && newFile < 8 && newRank >= 0 && newRank < 8)
                    {
                        var targetPos = new Position(newFile, newRank);
                        var targetPiece = board.GetPieceAt(targetPos);

                        if (targetPiece == null || targetPiece.IsWhite != isWhite)
                        {
                            moves.Add(new Move(position, targetPos));
                        }
                    }
                }
            }

            // Castling
            if (!IsKingInCheck(isWhite))
            {
                string castlingRights = isWhite ? board.CastlingRights.ToUpper() : board.CastlingRights.ToLower();

                // Kingside
                if (castlingRights.Contains(isWhite ? 'K' : 'k'))
                {
                    if (board.GetPieceAt(new Position(5, position.Rank)) == null &&
                        board.GetPieceAt(new Position(6, position.Rank)) == null &&
                        !IsSquareUnderAttack(new Position(5, position.Rank), !isWhite) &&
                        !IsSquareUnderAttack(new Position(6, position.Rank), !isWhite))
                    {
                        moves.Add(new Move(position, new Position(6, position.Rank)));
                    }
                }

                // Queenside
                if (castlingRights.Contains(isWhite ? 'Q' : 'q'))
                {
                    if (board.GetPieceAt(new Position(3, position.Rank)) == null &&
                        board.GetPieceAt(new Position(2, position.Rank)) == null &&
                        board.GetPieceAt(new Position(1, position.Rank)) == null &&
                        !IsSquareUnderAttack(new Position(3, position.Rank), !isWhite) &&
                        !IsSquareUnderAttack(new Position(2, position.Rank), !isWhite))
                    {
                        moves.Add(new Move(position, new Position(2, position.Rank)));
                    }
                }
            }
        }

        public bool IsKingInCheck(bool isWhite)
        {
            Position kingPos = FindKingPosition(isWhite);
            return IsSquareUnderAttack(kingPos, !isWhite);
        }

        public bool IsCheckmate(bool isWhite)
        {
            if (!IsKingInCheck(isWhite)) return false;

            for (int file = 0; file < 8; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    var pos = new Position(file, rank);
                    var piece = board.GetPieceAt(pos);

                    if (piece != null && piece.IsWhite == isWhite)
                    {
                        if (GetLegalMovesForPiece(pos).Count > 0)
                            return false;
                    }
                }
            }

            return true;
        }

        public bool IsStalemate(bool isWhite)
        {
            if (IsKingInCheck(isWhite)) return false;

            for (int file = 0; file < 8; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    var pos = new Position(file, rank);
                    var piece = board.GetPieceAt(pos);

                    if (piece != null && piece.IsWhite == isWhite)
                    {
                        if (GetLegalMovesForPiece(pos).Count > 0)
                            return false;
                    }
                }
            }

            return true;
        }

        private Position FindKingPosition(bool isWhite)
        {
            for (int file = 0; file < 8; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    var piece = board.GetPieceAt(new Position(file, rank));
                    if (piece != null && piece.Type == PieceType.King && piece.IsWhite == isWhite)
                    {
                        return new Position(file, rank);
                    }
                }
            }
            throw new InvalidOperationException("King not found on board");
        }

        private bool IsSquareUnderAttack(Position square, bool byWhite)
        {
            for (int file = 0; file < 8; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    var pos = new Position(file, rank);
                    var piece = board.GetPieceAt(pos);

                    if (piece != null && piece.IsWhite == byWhite)
                    {
                        var moves = GetPseudoLegalMoves(pos);
                        if (moves.Any(m => m.To.Equals(square)))
                            return true;
                    }
                }
            }

            return false;
        }

        private List<Move> GetPseudoLegalMoves(Position position)
        {
            var piece = board.GetPieceAt(position);
            if (piece == null) return new List<Move>();

            var moves = new List<Move>();
            switch (piece.Type)
            {
                case PieceType.Pawn:
                    GetPawnMoves(position, piece.IsWhite, moves);
                    break;
                case PieceType.Knight:
                    GetKnightMoves(position, piece.IsWhite, moves);
                    break;
                case PieceType.Bishop:
                    GetBishopMoves(position, piece.IsWhite, moves);
                    break;
                case PieceType.Rook:
                    GetRookMoves(position, piece.IsWhite, moves);
                    break;
                case PieceType.Queen:
                    GetQueenMoves(position, piece.IsWhite, moves);
                    break;
                case PieceType.King:
                    GetKingMoves(position, piece.IsWhite, moves);
                    break;
            }

            return moves;
        }
    }
}