using chess_bot_cs.ChessEngine;
using System.Windows;

namespace chess_bot_cs.UI
{
    public partial class PromotionDialog : Window
    {
        public PieceType SelectedPiece { get; private set; } = PieceType.Queen;

        public PromotionDialog(bool isWhite)
        {
            InitializeComponent();
            InitializePieces(isWhite);
        }

        private void InitializePieces(bool isWhite)
        {
            // This would initialize the UI elements for promotion choice
            // In a complete implementation, you would have buttons/images for:
            // Queen, Rook, Bishop, Knight
        }

        private void Queen_Click(object sender, RoutedEventArgs e)
        {
            SelectedPiece = PieceType.Queen;
            DialogResult = true;
            Close();
        }

        private void Rook_Click(object sender, RoutedEventArgs e)
        {
            SelectedPiece = PieceType.Rook;
            DialogResult = true;
            Close();
        }

        private void Bishop_Click(object sender, RoutedEventArgs e)
        {
            SelectedPiece = PieceType.Bishop;
            DialogResult = true;
            Close();
        }

        private void Knight_Click(object sender, RoutedEventArgs e)
        {
            SelectedPiece = PieceType.Knight;
            DialogResult = true;
            Close();
        }
    }
}