using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace DoNothing
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer gameTimer;
        private DispatcherTimer buttonGeneratorTimer;
        private DateTime gameStartTime;
        private bool gameActive = false;
        private List<Button> distractionButtons = new List<Button>();
        private Random random = new Random();
        private const string LeaderboardFile = "leaderboard.txt";

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimers();
            LoadLeaderboard();
        }

        private void InitializeTimers()
        {
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(100);
            gameTimer.Tick += GameTimer_Tick;

            buttonGeneratorTimer = new DispatcherTimer();
            buttonGeneratorTimer.Interval = TimeSpan.FromSeconds(2);
            buttonGeneratorTimer.Tick += ButtonGeneratorTimer_Tick;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {

            if (gameActive)
            {
                var elapsed = DateTime.Now - gameStartTime;
                TimerDisplay.Text = elapsed.TotalSeconds.ToString("F1");
            }
            else
            {
                var countdown = 3 - (DateTime.Now - gameStartTime).TotalSeconds;
                if (countdown <= 0)
                {
                    gameActive = true;
                    TimerDisplay.Text = "0,0";
                    StartGame();
                }
                else
                {
                    TimerDisplay.Text = countdown.ToString("F1");
                }
            }
        }

        private void ButtonGeneratorTimer_Tick(object sender, EventArgs e)
        {
            if (gameActive && GameCanvas.ActualWidth > 0 && GameCanvas.ActualHeight > 0)
            {
                CreateRandomButton();
            }
        }

        private void CreateRandomButton()
        {
            Button distractionButton = new Button();
            distractionButton.Content = GetRandomButtonText();
            distractionButton.Width = random.Next(80, 150);
            distractionButton.Height = random.Next(30, 60);
            distractionButton.Background = GetRandomBrush();
            distractionButton.FontSize = random.Next(12, 18);

            double maxX = Math.Max(0, GameCanvas.ActualWidth - distractionButton.Width);
            double maxY = Math.Max(0, GameCanvas.ActualHeight - distractionButton.Height);

            if (maxX > 0 && maxY > 0)
            {
                double left = random.NextDouble() * maxX;
                double top = random.NextDouble() * maxY;

                Point cursorPosition = Mouse.GetPosition(GameCanvas);
                if (left < cursorPosition.X && left + distractionButton.Width > cursorPosition.X)
                {
                    left = cursorPosition.X + 10;
                }

                Canvas.SetLeft(distractionButton, left);
                Canvas.SetTop(distractionButton, top);

                GameCanvas.Children.Add(distractionButton);
                distractionButtons.Add(distractionButton);

                DispatcherTimer removeTimer = new DispatcherTimer();
                removeTimer.Interval = TimeSpan.FromSeconds(random.Next(3, 8));
                removeTimer.Tick += (s, e) =>
                {
                    removeTimer.Stop();
                    if (GameCanvas.Children.Contains(distractionButton))
                    {
                        GameCanvas.Children.Remove(distractionButton);
                        distractionButtons.Remove(distractionButton);
                    }
                };
                removeTimer.Start();
            }
        }

        private string GetRandomButtonText()
        {
            string[] texts = {
                "Nyomj már!", "Sok pénz", "NYOMJ", "GazdagSág",
                "NYOMJ IDE", "MOST!", "GYORSAN", "Megéri!!", "Ötös!"
            };
            return texts[random.Next(texts.Length)];
        }

        private Brush GetRandomBrush()
        {
            Color[] colors = {
                Colors.Red, Colors.Blue, Colors.Green, Colors.Yellow,
                Colors.Orange, Colors.Purple, Colors.Pink, Colors.Cyan,
                Colors.Magenta, Colors.Lime, Colors.Gold, Colors.Crimson
            };
            return new SolidColorBrush(colors[random.Next(colors.Length)]);
        }

        private void StartGame()
        {
            gameStartTime = DateTime.Now;
            foreach (Button btn in distractionButtons.ToList())
            {
                GameCanvas.Children.Remove(btn);
            }
            distractionButtons.Clear();
            gameTimer.Start();
            buttonGeneratorTimer.Start();
            StartButton.IsEnabled = false;
            LeaderboardButton.IsEnabled = false;
        }

        private void EndGame()
        {
            if (!gameActive) return;

            gameActive = false;
            gameTimer.Stop();
            buttonGeneratorTimer.Stop();

            var elapsed = DateTime.Now - gameStartTime;
            double finalScore = elapsed.TotalSeconds;


            Ending endingWindow = new Ending(finalScore);
            endingWindow.ShowDialog();

            StartButton.IsEnabled = true;
            LeaderboardButton.IsEnabled = true;

            foreach (Button btn in distractionButtons.ToList())
            {
                GameCanvas.Children.Remove(btn);
            }
            distractionButtons.Clear();
        }

        private void LoadLeaderboard()
        {
            try
            {
                if (!File.Exists(LeaderboardFile))
                {
                    File.Create(LeaderboardFile).Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating leaderboard file: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void DoingSmth(object sender, MouseEventArgs e)
        {
            if (gameActive)
            {
                EndGame();
            }
        }

        private void DoingSmth(object sender, KeyEventArgs e)
        {
            if (gameActive)
            {
                EndGame();
            }
        }


        private void DoingSmth(object sender, MouseButtonEventArgs e)
        {
            if (gameActive)
            {
                EndGame();
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void LeaderboardButton_Click(object sender, RoutedEventArgs e)
        {
            if (!gameActive)
            {
                LeaderboardWindow leaderboardWindow = new LeaderboardWindow();
                leaderboardWindow.ShowDialog();
            }
        }
    }
}
