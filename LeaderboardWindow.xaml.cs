using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace DoNothing
{
    public partial class LeaderboardWindow : Window
    {
        private const string LeaderboardFile = "leaderboard.txt";
        private List<ScoreEntry> scores = new List<ScoreEntry>();

        public LeaderboardWindow()
        {
            InitializeComponent();
            LoadScores();
        }

        private void LoadScores()
        {
            scores.Clear();

            try
            {
                if (File.Exists(LeaderboardFile))
                {
                    string[] lines = File.ReadAllLines(LeaderboardFile);

                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string[] parts = line.Split(';');
                            if (parts.Length == 2)
                            {
                                if (double.TryParse(parts[1], out double score))
                                {
                                    scores.Add(new ScoreEntry { Name = parts[0], Score = score });
                                }
                            }
                        }
                    }
                }

                var sortedScores = scores.OrderByDescending(s => s.Score).ToList();

                LeaderboardListBox.ItemsSource = sortedScores;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading leaderboard: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadScores();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear all scores? This cannot be undone!",
                                       "Confirm Clear", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    File.WriteAllText(LeaderboardFile, string.Empty);
                    LoadScores();
                    MessageBox.Show("All scores have been cleared.", "Cleared",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error clearing scores: {ex.Message}", "Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class ScoreEntry
    {
        public string Name { get; set; } = "";
        public double Score { get; set; }
    }
}
