using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace EducationalPractice02
{
    public partial class MainWindow : Window
    {
        private Random random = new Random();
        private List<(DateTime time, int value)> dataPoints = new List<(DateTime, int)>();
        private bool isPaused = true;
        private DispatcherTimer updateTimer; 
        private DateTime lastTime = DateTime.Now;

        public MainWindow()
        {
            InitializeComponent();
            GenerateDataPoints();
            DrawChart();

            // Инициализация таймера
            updateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) 
            };
            updateTimer.Tick += UpdateTimer_Tick; 
        }

        // Обработчик таймера для добавления новой точки
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            AddNewDataPoint();  
            DrawChart();  
        }

        // Метод для добавления новой точки
        private void AddNewDataPoint()
        {
            lastTime = lastTime.AddSeconds(5);  
            int hour = lastTime.Hour;  
            int value = GetValueForHour(hour);  

            // Добавляем новую точку
            dataPoints.Add((lastTime, value));

            // Удаляем старую точку, если их слишком много
            if (dataPoints.Count > 50)
            {
                dataPoints.RemoveAt(0);  
            }
        }


        private void GenerateDataPoints()
        {
            dataPoints.Clear(); 
            DateTime now = DateTime.Now;
            for (int i = 0; i < 24; i++)
            {
                DateTime hour = now.AddHours(-23 + i); 
                int value = GetValueForHour(hour.Hour);
                dataPoints.Add((hour, value));
            }
            UpdateAverageOnlineText(); 
        }

        private int GetValueForHour(int hour)
        {
            if (hour >= 1 && hour < 3)
                return 0;
            else if (hour >= 3 && hour < 6)
                return random.Next(1, 11);
            else if (hour >= 6 && hour < 8)
                return random.Next(10, 16);
            else if (hour >= 8 && hour < 11)
                return random.Next(15, 31);
            else if (hour >= 11 && hour < 13)
                return random.Next(30, 51);
            else if (hour >= 13 && hour < 15)
                return random.Next(50, 71);
            else if (hour >= 15 && hour < 17)
                return random.Next(70, 91);
            else if (hour >= 17 && hour < 19)
                return random.Next(90, 111);
            else if (hour >= 19 && hour < 23)
                return random.Next(110, 151);
            else
                return random.Next(90, 101);
        }

        private void GenerateMonthlyDataPoints()
        {
            dataPoints.Clear();
            DateTime now = DateTime.Now;
            for (int i = 0; i < 30; i++)
            {
                DateTime day = now.AddDays(-29 + i);
                int value = random.Next(110, 151); 
                dataPoints.Add((day, value));
            }
            UpdateAverageOnlineText(); 
        }

        private void GenerateYearlyDataPoints()
        {
            dataPoints.Clear(); 
            DateTime now = DateTime.Now;

            for (int i = 0; i < 12; i++)
            {
                DateTime month = now.AddMonths(-11 + i); 
                int value = random.Next(110, 151); 
                dataPoints.Add((month, value));
            }
            UpdateAverageOnlineText(); 
        }

        // Метод отрисовки графика
        private void DrawChart()
        {
            ChartCanvas.Children.Clear(); 

            if (dataPoints.Count < 2)
                return;  

            double canvasWidth = ChartCanvas.ActualWidth > 0 ? ChartCanvas.ActualWidth : ChartCanvas.Width;
            double canvasHeight = ChartCanvas.ActualHeight > 0 ? ChartCanvas.ActualHeight : ChartCanvas.Height;

            double maxValue = 151;  // Макс. значение для масштаба графика
            double minValue = 0;

            double pointSpacing = canvasWidth / (dataPoints.Count - 1);

            Polyline polyline = new Polyline
            {
                Stroke = (Brush)new BrushConverter().ConvertFromString("#4CAF50"),
                StrokeThickness = 2
            };

            for (int i = 0; i < dataPoints.Count; i++)
            {
                var (time, value) = dataPoints[i];

                double x = i * pointSpacing;
                double y = canvasHeight - (value - minValue) / (maxValue - minValue) * canvasHeight;

                polyline.Points.Add(new Point(x, y));

                // Рисуем точки
                Ellipse ellipse = new Ellipse
                {
                    Width = 6,
                    Height = 6,
                    Fill = (Brush)new BrushConverter().ConvertFromString("#4CAF50"),
                    Tag = (time, value) 
                };

                ellipse.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;  

                Canvas.SetLeft(ellipse, x - ellipse.Width / 2);
                Canvas.SetTop(ellipse, y - ellipse.Height / 2);
                ChartCanvas.Children.Add(ellipse);
            }

            // Добавляем линию графика
            ChartCanvas.Children.Add(polyline);

            UpdateAverageOnlineText(); 
        }

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Ellipse ellipse && ellipse.Tag is ValueTuple<DateTime, int> data)
            {
                var (time, value) = data;

                string message = $"\nОнлайн: {value}\nДата отрисовки: {time:yyyy-MM-dd HH:mm:ss}";

                MessageBox.Show(message, "Информация о точке", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ChartCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawChart();
        }

        private void SetActiveButton(Button activeButton)
        {
            OnlineDay.Style = (Style)FindResource("Inactive");
            OnlineMonth.Style = (Style)FindResource("Inactive");
            OnlineYear.Style = (Style)FindResource("Inactive");

            activeButton.Style = (Style)FindResource("Active");
        }

        private void DisableButton(Button activeButton)
        {
            OnlineDay.IsEnabled = true;
            OnlineMonth.IsEnabled = true;
            OnlineYear.IsEnabled = true;
        }

        private void OnlineDay_Click(object sender, RoutedEventArgs e)
        {
            DisableButton(OnlineDay);
            SetActiveButton(OnlineDay);
            GenerateDataPoints();
            DrawChart();

            OnlineDay.IsEnabled = false;
        }

        private void OnlineMonth_Click(object sender, RoutedEventArgs e)
        {
            DisableButton(OnlineMonth);
            SetActiveButton(OnlineMonth);
            GenerateMonthlyDataPoints();
            DrawChart();

            OnlineMonth.IsEnabled = false;
        }

        private void OnlineYear_Click(object sender, RoutedEventArgs e)
        {
            DisableButton(OnlineYear);
            SetActiveButton(OnlineYear);
            GenerateYearlyDataPoints();
            DrawChart();

            OnlineYear.IsEnabled = false;
        }

        private double CalculateAverageOnline()
        {
            if (dataPoints.Count == 0)
                return 0;

            double total = 0;
            foreach (var data in dataPoints)
            {
                total += data.value;
            }

            return total / dataPoints.Count;
        }

        private void UpdateAverageOnlineText()
        {
            double average = CalculateAverageOnline();
            TextBlock Average = (TextBlock)FindName("Average");
            if (Average != null)
            {
                Average.Text = $"Средний онлайн: {average:F0}";
            }
        }

        private void ChartPause_Click(object sender, RoutedEventArgs e)
        {
            if (isPaused)
            {
                ChartPause.Content = "Пауза";
                ChartPause.Style = (Style)FindResource("Pause");

                updateTimer.Start();
            }
            else
            {
                ChartPause.Content = "Старт";
                ChartPause.Style = (Style)FindResource("Active");

                updateTimer.Stop();
            }

            isPaused = !isPaused;
        }
    }
}