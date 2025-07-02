namespace chess_bot_cs.ChessEngine
{
    public class Position
    {
        public int File { get; } // 0-7 (a-h)
        public int Rank { get; } // 0-7 (1-8)

        public Position(int file, int rank)
        {
            File = file;
            Rank = rank;
        }

        public override string ToString()
        {
            return $"{(char)('a' + File)}{Rank + 1}";
        }
    }

    public class Move
    {
        public Position From { get; }
        public Position To { get; }
        public Piece CapturedPiece { get; set; }
        public PieceType Promotion { get; set; }

        public Move(Position from, Position to)
        {
            From = from;
            To = to;
            Promotion = PieceType.None;
        }

        public override string ToString()
        {
            return $"{From}-{To}";
        }
    }
}