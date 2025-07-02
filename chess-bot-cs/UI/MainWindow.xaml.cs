using chess_bot_cs.ChessEngine;
using chess_bot_cs.MachineLearning;
using System.Windows;

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
            NewGame();
        }

        private void NewGame()
        {
            game = new Game();
            bot = new DecisionMaker();

            game.OnMoveMade += Game_OnMoveMade;
            game.OnGameStateChanged += Game_OnGameStateChanged;

            ChessBoard.InitializeBoard(game.Board);
            UpdateGameInfo();
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
            });
        }

        private void UpdateGameInfo()
        {
            TurnText.Text = game.Board.WhiteToMove ? "White's turn" : "Black's turn";

            switch (game.State)
            {
                case GameState.WhiteWon:
                    StatusText.Text = "White wins!";
                    break;
                case GameState.BlackWon:
                    StatusText.Text = "Black wins!";
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
            string moveText = $"{game.MoveHistory.Count / 2 + 1}. {move}";
            if (game.Board.WhiteToMove) // Last move was black's
            {
                MoveHistoryList.Items.Add(moveText);
            }
            else if (MoveHistoryList.Items.Count > 0 && game.MoveHistory.Count % 2 == 1)
            {
                // First white move in the game
                moveText = $"{game.MoveHistory.Count / 2 + 1}. {move}";
                MoveHistoryList.Items.Add(moveText);
            }
            else
            {
                // Append black move to white move
                int lastIndex = MoveHistoryList.Items.Count - 1;
                string existing = MoveHistoryList.Items[lastIndex].ToString();
                MoveHistoryList.Items[lastIndex] = $"{existing} {move}";
            }

            MoveHistoryList.ScrollIntoView(MoveHistoryList.Items[MoveHistoryList.Items.Count - 1]);
        }

        private void MakeBotMove()
        {
            int difficulty = DifficultyCombo.SelectedIndex;
            var move = bot.DecideBestMove(game, difficulty);

            if (move != null)
            {
                game.MakeMove(move);
            }
        }

        private void NewGameBtn_Click(object sender, RoutedEventArgs e)
        {
            playerIsWhite = !playerIsWhite;
            MoveHistoryList.Items.Clear();
            NewGame();

            if (!playerIsWhite)
            {
                MakeBotMove();
            }
        }

        private void UndoMoveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Implement undo functionality if needed
        }
    }
}