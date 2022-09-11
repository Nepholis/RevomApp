using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RevomApp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Brush customColor;
        Random r = new Random();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Add_or_Remove_Items(object sender, MouseButtonEventArgs e)
        {
            if(e.OriginalSource is Rectangle)
            {
                Rectangle activeRectangle =(Rectangle)e.OriginalSource;

                myCanvas.Children.Remove(activeRectangle);
            }
            else
            {
                customColor = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255)));
                Rectangle newRectangle = new Rectangle
                {
                    Width = 50,
                    Height = 50,
                    Fill = customColor,
                    StrokeThickness = 3,
                    Stroke = Brushes.Black
                };
                Canvas.SetLeft(newRectangle, Mouse.GetPosition(myCanvas).X - newRectangle.Width/2);
                Canvas.SetTop(newRectangle, Mouse.GetPosition(myCanvas).Y - newRectangle.Height /2);

                myCanvas.Children.Add(newRectangle);
            }
        }
    }
}
