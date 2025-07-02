using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using chess_bot_cs.ChessEngine;

namespace chess_bot_cs.UI
{
    public partial class ChessBoardControl : UserControl
    {
        private const int BoardSize = 8;
        private Rectangle[,] squares = new Rectangle[BoardSize, BoardSize];
        private TextBlock[,] pieceSymbols = new TextBlock[BoardSize, BoardSize];
        private Board currentBoard;

        public ChessBoardControl()
        {
            InitializeComponent();
        }

        public void InitializeBoard(Board board)
        {
            currentBoard = board;
            ChessGrid.Children.Clear();
            ChessGrid.ColumnDefinitions.Clear();
            ChessGrid.RowDefinitions.Clear();

            // Create grid columns and rows
            for (int i = 0; i < BoardSize; i++)
            {
                ChessGrid.ColumnDefinitions.Add(new ColumnDefinition());
                ChessGrid.RowDefinitions.Add(new RowDefinition());
            }

            // Create squares and piece displays
            for (int file = 0; file < BoardSize; file++)
            {
                for (int rank = 0; rank < BoardSize; rank++)
                {
                    // Create square background
                    var square = new Rectangle();
                    square.Fill = (file + rank) % 2 == 0 ? Brushes.White : Brushes.LightGray;
                    square.Stroke = Brushes.Black;
                    square.StrokeThickness = 0.5;

                    Grid.SetColumn(square, file);
                    Grid.SetRow(square, BoardSize - 1 - rank); // Rank 0 at bottom

                    // Create piece symbol
                    var pieceSymbol = new TextBlock();
                    pieceSymbol.HorizontalAlignment = HorizontalAlignment.Center;
                    pieceSymbol.VerticalAlignment = VerticalAlignment.Center;
                    pieceSymbol.FontWeight = FontWeights.Bold;
                    pieceSymbol.FontSize = 24;

                    Grid.SetColumn(pieceSymbol, file);
                    Grid.SetRow(pieceSymbol, BoardSize - 1 - rank);

                    // Store references
                    squares[file, rank] = square;
                    pieceSymbols[file, rank] = pieceSymbol;

                    // Add to grid
                    ChessGrid.Children.Add(square);
                    ChessGrid.Children.Add(pieceSymbol);

                    // Add click handler
                    square.MouseDown += (s, e) => Square_MouseDown(file, rank);
                }
            }

            UpdateBoard(board);
        }

        public void UpdateBoard(Board board)
        {
            currentBoard = board;

            for (int file = 0; file < BoardSize; file++)
            {
                for (int rank = 0; rank < BoardSize; rank++)
                {
                    var piece = board.GetPieceAt(new Position(file, rank));
                    pieceSymbols[file, rank].Text = piece?.ToString() ?? "";
                    pieceSymbols[file, rank].Foreground = piece?.IsWhite == true ? Brushes.White : Brushes.Black;
                }
            }
        }

        public void HighlightSquare(int file, int rank, bool highlight)
        {
            squares[file, rank].Stroke = highlight ? Brushes.Red : Brushes.Black;
            squares[file, rank].StrokeThickness = highlight ? 3 : 0.5;
        }

        public void ClearHighlights()
        {
            for (int file = 0; file < BoardSize; file++)
            {
                for (int rank = 0; rank < BoardSize; rank++)
                {
                    HighlightSquare(file, rank, false);
                }
            }
        }

        private void Square_MouseDown(int file, int rank)
        {
            // Handle square selection and moves
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.HandleSquareClick(file, rank);
            }
        }

        private void ChessGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Adjust font size based on available space
            double newSize = e.NewSize.Width / BoardSize * 0.6;
            foreach (var symbol in pieceSymbols)
            {
                if (symbol != null)
                {
                    symbol.FontSize = newSize;
                }
            }
        }
    }
}