using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WPF_LB_5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        static int count = 0;

        class MySinFunction : EasingFunctionBase
        {
            protected override double EaseInCore(double normalizedTime)
            {
                // applies the formula of time to the seventh power.
                //return Math.Pow(normalizedTime, 7);
                return Math.Sin(normalizedTime * new Random().Next(4,50));
            }
            // Typical implementation of CreateInstanceCore
            protected override Freezable CreateInstanceCore()
            {
                return new MySinFunction();
            }
        }

        class MyCosFunction : EasingFunctionBase
        {
            public MyCosFunction() : base()
            {
                this.EasingMode = EasingMode.EaseIn;
            }

            protected override double EaseInCore(double normalizedTime)
            {
                //applies the formula of time to the seventh power.
                //return Math.Pow(normalizedTime, 7);
                return Math.Cos(normalizedTime * 8);
            }
            // Typical implementation of CreateInstanceCore
            protected override Freezable CreateInstanceCore()
            {
                return new MyCosFunction();
               
            }
        }

        async void MoveElAsync(Ellipse El)
        {
            ++count;
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            Task allTasks = null;

            try
            {
                Task t1 = Task.Run(delegate ()
                {
                    int[] m = new int[] { -75, -150, 75, 150 };
                    var rnd = new Random();
                    while (!token.IsCancellationRequested)
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, (Action)(() =>
                        {

                            var dx = new DoubleAnimation(Canvas.GetLeft(El), Canvas.GetLeft(El) + m[rnd.Next(4)], new Duration(new TimeSpan(0, 0, 4)));
                            var dy = new DoubleAnimation(Canvas.GetTop(El), Canvas.GetTop(El) + m[rnd.Next(4)], new Duration(new TimeSpan(0, 0, 4)));

                            El.BeginAnimation(Canvas.LeftProperty, dx);
                            El.BeginAnimation(Canvas.TopProperty, dy);
                            //Points.Text = $"{Canvas.GetLeft(El)}<0 || { Canvas.GetTop(El)}<0 || {Canvas.GetLeft(El)}> {Canva.ActualWidth} || { Canvas.GetTop(El)}>{Canva.ActualHeight}";
                        }));
                        Thread.Sleep(4000);
                    }
                });

                Task t2 = Task.Run(() =>
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, (Action)(() =>
                    {
                        var dt = new DoubleAnimation(20, 40, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
                        dt.RepeatBehavior = RepeatBehavior.Forever;
                        dt.AutoReverse = true;
                        dt.EasingFunction = new CircleEase();
                        El.BeginAnimation(Ellipse.WidthProperty, dt);
                        El.BeginAnimation(Ellipse.HeightProperty, dt);
                    }));
                });

                Task t3 = Task.Run(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, (Action)(() =>
                        {
                            if (Canvas.GetLeft(El) < 0 || Canvas.GetTop(El) < 0 || Canvas.GetLeft(El) > Canva.ActualWidth || Canvas.GetTop(El) > Canva.ActualHeight)
                            {
                                Canva.Children.Remove(El);
                                cts.Cancel();
                            }
                        }));
                        Thread.Sleep(1000);
                    }
                });
                allTasks = Task.WhenAll(t1, t2, t3);
                await allTasks;
                --count;
                //MessageBox.Show("ALL END!!!");
            }
            catch (Exception ex)
            {
                string str=string.Empty;
                str += $"Исключение: {ex.Message};\n";
                str += $"IsFaulted: {allTasks.IsFaulted};\n";
                foreach (var inx in allTasks.Exception.InnerExceptions)
                {
                    str += $"Внутреннее исключение: {inx.Message};\n";
                }
                MessageBox.Show(str);
            }
            
        }

        private void btStart3_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show($"The number of processors on this computer is { Environment.ProcessorCount}.");
            //if (count < Environment.ProcessorCount)
            //{
                Ellipse El = new Ellipse();
                El.Width = 30;
                El.Height = 30;
                El.Fill = Brushes.LightBlue;
                var rnd = new Random();
                Canvas.SetLeft(El, rnd.Next(30, 450));
                Canvas.SetTop(El, rnd.Next(30, 450));
                Canva.Children.Add(El);
                MoveElAsync(El);
            //}
        }
    }
}
