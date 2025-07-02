using System.Windows;
using System.Windows.Threading;
using chess_bot_cs.UI;

namespace chess_bot_cs
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Global exception handling
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            // Initialize and show main window
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Handle unhandled exceptions
            MessageBox.Show(
                $"An unexpected error occurred:\n\n{e.Exception.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            e.Handled = true;

            // Log the exception (you would implement your own logging)
            // FileLogger.LogError("Unhandled exception", e.Exception);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Cleanup resources
            // (Add any application-wide cleanup code here)

            base.OnExit(e);
        }
    }
}