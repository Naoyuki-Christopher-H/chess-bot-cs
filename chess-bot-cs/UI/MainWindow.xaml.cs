using System;
using System.Windows;
using System.Windows.Controls;
using chess_bot_cs.ChessEngine;
using chess_bot_cs.MachineLearning;

namespace chess_bot_cs.UI
{
    public partial class MainWindow : Window
    {
        private Game game;
        private DecisionMaker bot;
        private bool playerIsWhite = true;
        private Position selectedSquare = null;

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

            // If player is black, let bot make first move
            if (!playerIsWhite)
            {
                MakeBotMove();
            }
        }

        public void HandleSquareClick(int file, int rank)
        {
            if (game.State != GameState.InProgress) return;
            if (game.Board.WhiteToMove != playerIsWhite) return; // Not player's turn

            var position = new Position(file, rank);
            var piece = game.Board.GetPieceAt(position);

            // If no square is selected and clicked square has player's piece
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

            // If clicking on already selected square, deselect it
            if (selectedSquare.File == file && selectedSquare.Rank == rank)
            {
                selectedSquare = null;
                ChessBoard.ClearHighlights();
                return;
            }

            // If clicking on another of player's pieces, select that instead
            if (piece != null && piece.IsWhite == playerIsWhite)
            {
                selectedSquare = position;
                ChessBoard.ClearHighlights();
                ChessBoard.HighlightSquare(file, rank, true);
                HighlightLegalMoves(file, rank);
                return;
            }

            // Try to make move
            var move = new Move(selectedSquare, position);
            var movingPiece = game.Board.GetPieceAt(selectedSquare);

            // Handle pawn promotion
            if (movingPiece?.Type == PieceType.Pawn && (rank == 0 || rank == 7))
            {
                var promotionDialog = new PromotionDialog(playerIsWhite);
                if (promotionDialog.ShowDialog() == true)
                {
                    move.Promotion = promotionDialog.SelectedPiece;
                }
                else
                {
                    return; // User canceled promotion
                }
            }

            if (game.MakeMove(move))
            {
                selectedSquare = null;
                ChessBoard.ClearHighlights();
            }
        }

        private void HighlightLegalMoves(int file, int rank)
        {
            var legalMoves = game.GetLegalMoves(new Position(file, rank));
            foreach (var move in legalMoves)
            {
                ChessBoard.HighlightSquare(move.To.File, move.To.Rank, false);
            }
        }

        private void MakeBotMove()
        {
            if (game.State != GameState.InProgress) return;
            if (game.Board.WhiteToMove == playerIsWhite) return; // Not bot's turn

            int difficulty = DifficultyCombo.SelectedIndex;
            var move = bot.DecideBestMove(game, difficulty + 1); // +1 to make easy=1, medium=2, hard=3

            if (move != null)
            {
                // Handle pawn promotion for bot (always promote to queen)
                var movingPiece = game.Board.GetPieceAt(move.From);
                if (movingPiece?.Type == PieceType.Pawn &&
                    (move.To.Rank == 0 || move.To.Rank == 7) &&
                    move.Promotion == PieceType.None)
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
                ChessBoard.UpdateBoard(game.Board);
                UpdateGameInfo();
                AddMoveToHistory(move);

                // If it's the bot's turn, make a move
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
            string moveText = move.ToString();
            string moveNumber = (game.MoveHistory.Count / 2 + 1).ToString();

            if (game.Board.WhiteToMove) // Last move was black's
            {
                if (MoveHistoryList.Items.Count % 2 == 1)
                {
                    // Add black move to existing white move
                    int lastIndex = MoveHistoryList.Items.Count - 1;
                    string existing = MoveHistoryList.Items[lastIndex].ToString();
                    MoveHistoryList.Items[lastIndex] = $"{existing} {moveText}";
                }
                else
                {
                    // Shouldn't happen as white moves first
                    MoveHistoryList.Items.Add($"{moveNumber}. {moveText}");
                }
            }
            else // Last move was white's
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
            // Simple undo implementation - just restart the game
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