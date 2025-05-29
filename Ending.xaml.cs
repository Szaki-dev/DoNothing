using Microsoft.VisualBasic;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace DoNothing
{
    public partial class Ending : Window
    {
        private double finalScore;
        private const string LeaderboardFile = "leaderboard.txt";

        public Ending(double score)
        {
            InitializeComponent();
            finalScore = score;
            LoadScoreDisplay();
        }

        private void LoadScoreDisplay()
        {
            ScoreText.Text = $"Final Score: {finalScore:F1}";
            TimeText.Text = $"Time Survived: {finalScore:F1} seconds";
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string playerName = Interaction.InputBox("Enter your name for the leaderboard:", "Leaderboard Entry", "Player"); ;
            while (string.IsNullOrWhiteSpace(playerName))
            {
                MessageBox.Show("Jó fura nevet adtak neked a szüleid, próbáljuk csak meg újra", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                playerName = Interaction.InputBox("Enter your name for the leaderboard: \n(de mostmár tényleg)", "Leaderboard Entry", "Player");
            }
            try
            {
                string scoreEntry = $"{playerName};{finalScore:F1}";
                File.AppendAllText(LeaderboardFile, scoreEntry + Environment.NewLine);
                LeaderboardWindow leaderboardWindow = new LeaderboardWindow();
                leaderboardWindow.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving score: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RetryButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LeaderboardButton_Click(object sender, RoutedEventArgs e)
        {
            LeaderboardWindow leaderboard = new LeaderboardWindow();
            leaderboard.ShowDialog();
        }
    }
}
