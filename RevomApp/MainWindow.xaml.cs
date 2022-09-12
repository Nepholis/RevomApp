using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
            if (e.OriginalSource is Rectangle)
            {
                Rectangle activeRectangle = (Rectangle)e.OriginalSource;

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
                    Stroke = Brushes.White

                };
                Canvas.SetLeft(newRectangle, Mouse.GetPosition(myCanvas).X - newRectangle.Width / 2);
                Canvas.SetTop(newRectangle, Mouse.GetPosition(myCanvas).Y - newRectangle.Height / 2);

                myCanvas.Children.Add(newRectangle);
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "RevomApp Files | *.xaml";
            saveFileDialog.DefaultExt = "xaml";

            if (saveFileDialog.ShowDialog() == true)
            {
                //string xaml = System.Windows.Markup.XamlWriter.Save(myCanvas.Children);
                //File.WriteAllText(saveFileDialog.FileName, xaml);
                FileStream fs = File.Open(saveFileDialog.FileName, FileMode.Create);
                System.Windows.Markup.XamlWriter.Save(myCanvas, fs);
                fs.Close();
            }
        }

        private void Button_Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "RevomApp Files | *.xaml";
            openFileDialog.DefaultExt = "xaml";

            if (openFileDialog.ShowDialog() == true)
            {
                //myCanvas.Children = File.ReadAllText(openFileDialog.FileName);
                FileStream fs = File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
                Canvas savedCanvas = System.Windows.Markup.XamlReader.Load(fs) as Canvas;
                fs.Close();

                myDockPanel.Children.Add(savedCanvas);
            }
                
        }
    }
}
