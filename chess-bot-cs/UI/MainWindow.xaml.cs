using chess_bot_cs.ChessEngine;
using chess_bot_cs.MachineLearning;
using System.Windows;
using System.Windows.Controls;

namespace chess_bot_cs.UI
{
    public partial class MainWindow : Window
    {
        private Game? game;
        private DecisionMaker? bot;
        private bool playerIsWhite = true;
        private Position? selectedSquare = null;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            game = new Game();
            bot = new DecisionMaker();

            game.OnMoveMade += Game_OnMoveMade;
            game.OnGameStateChanged += Game_OnGameStateChanged;

            ChessBoard.InitializeBoard(game.Board);
            UpdateGameInfo();
            MoveHistoryList.Items.Clear();

            if (!playerIsWhite)
            {
                MakeBotMove();
            }
        }

        public void HandleSquareClick(int file, int rank)
        {
            if (game == null || game.State != GameState.InProgress) return;
            if (game.Board.WhiteToMove != playerIsWhite) return;

            var position = new Position(file, rank);
            var piece = game.Board.GetPieceAt(position);

            if (selectedSquare == null)
            {
                if (piece != null && piece.IsWhite == playerIsWhite)
                {
                    selectedSquare = position;
                    ChessBoard.HighlightSquare(file, rank, true);
                    HighlightLegalMoves(file, rank);
                }
                return;
            }

            if (selectedSquare.File == file && selectedSquare.Rank == rank)
            {
                selectedSquare = null;
                ChessBoard.ClearHighlights();
                return;
            }

            if (piece != null && piece.IsWhite == playerIsWhite)
            {
                selectedSquare = position;
                ChessBoard.ClearHighlights();
                ChessBoard.HighlightSquare(file, rank, true);
                HighlightLegalMoves(file, rank);
                return;
            }

            var move = new Move(selectedSquare, position);
            var movingPiece = game.Board.GetPieceAt(selectedSquare);

            if (movingPiece?.Type == PieceType.Pawn && (rank == 0 || rank == 7))
            {
                move.Promotion = PieceType.Queen;
            }

            if (game.MakeMove(move))
            {
                selectedSquare = null;
                ChessBoard.ClearHighlights();
            }
        }

        private void HighlightLegalMoves(int file, int rank)
        {
            if (game == null) return;

            var legalMoves = game.GetLegalMoves(new Position(file, rank));
            foreach (var move in legalMoves)
            {
                ChessBoard.HighlightSquare(move.To.File, move.To.Rank, false);
            }
        }

        private void MakeBotMove()
        {
            if (game == null || bot == null) return;
            if (game.State != GameState.InProgress) return;
            if (game.Board.WhiteToMove == playerIsWhite) return;

            int difficulty = DifficultyCombo.SelectedIndex;
            var move = bot.DecideBestMove(game, difficulty + 1);

            if (move != null)
            {
                var movingPiece = game.Board.GetPieceAt(move.From);
                if (movingPiece?.Type == PieceType.Pawn &&
                    (move.To.Rank == 0 || move.To.Rank == 7))
                {
                    move.Promotion = PieceType.Queen;
                }

                game.MakeMove(move);
            }
        }

        private void Game_OnMoveMade(Move move)
        {
            Dispatcher.Invoke(() =>
            {
                if (game == null) return;

                ChessBoard.UpdateBoard(game.Board);
                UpdateGameInfo();
                AddMoveToHistory(move);

                if (game.State == GameState.InProgress && game.Board.WhiteToMove != playerIsWhite)
                {
                    MakeBotMove();
                }
            });
        }

        private void Game_OnGameStateChanged(GameState state)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateGameInfo();
                if (state != GameState.InProgress)
                {
                    ChessBoard.ClearHighlights();
                    selectedSquare = null;
                }
            });
        }

        private void UpdateGameInfo()
        {
            if (game == null) return;

            TurnText.Text = game.Board.WhiteToMove ? "White's turn" : "Black's turn";

            switch (game.State)
            {
                case GameState.WhiteWon:
                    StatusText.Text = playerIsWhite ? "You win!" : "Bot wins!";
                    break;
                case GameState.BlackWon:
                    StatusText.Text = playerIsWhite ? "Bot wins!" : "You win!";
                    break;
                case GameState.Draw:
                    StatusText.Text = "Draw!";
                    break;
                default:
                    StatusText.Text = "Game in progress";
                    break;
            }
        }

        private void AddMoveToHistory(Move move)
        {
            if (game == null) return;

            string moveText = move.ToString();
            string moveNumber = (game.MoveHistory.Count / 2 + 1).ToString();

            if (game.Board.WhiteToMove)
            {
                if (MoveHistoryList.Items.Count % 2 == 1)
                {
                    int lastIndex = MoveHistoryList.Items.Count - 1;
                    string existing = MoveHistoryList.Items[lastIndex].ToString() ?? "";
                    MoveHistoryList.Items[lastIndex] = $"{existing} {moveText}";
                }
                else
                {
                    MoveHistoryList.Items.Add($"{moveNumber}. {moveText}");
                }
            }
            else
            {
                MoveHistoryList.Items.Add($"{moveNumber}. {moveText}");
            }

            MoveHistoryList.ScrollIntoView(MoveHistoryList.Items[MoveHistoryList.Items.Count - 1]);
        }

        private void NewGameBtn_Click(object sender, RoutedEventArgs e)
        {
            playerIsWhite = !playerIsWhite;
            InitializeGame();
        }

        private void UndoMoveBtn_Click(object sender, RoutedEventArgs e)
        {
            InitializeGame();
        }

        private void DifficultyCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (game != null && game.State == GameState.InProgress &&
                game.Board.WhiteToMove != playerIsWhite)
            {
                MakeBotMove();
            }
        }
    }
}