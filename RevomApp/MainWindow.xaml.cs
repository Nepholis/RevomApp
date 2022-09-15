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
using System.Windows.Controls.Primitives;
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
        AdornerLayer adorner;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += (sender, e) =>
            {
                adorner = AdornerLayer.GetAdornerLayer(myCanvas);
            };
        }

        private void Add_or_Remove_Items(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Border)
            {
                Border activeBorder = (Border)e.OriginalSource;

                myCanvas.Children.Remove(activeBorder);
                adorner.Remove(new BorderAdorner(activeBorder));
            }
            else
            {
                customColor = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255)));
                Border newBorder = new Border()
                {
                    Width = 100,
                    Height = 100,
                    Background = customColor,
                    BorderThickness = new Thickness(3,3,3,3),
                    BorderBrush = Brushes.White,
                    AllowDrop = true,
                    
            };
                Canvas.SetLeft(newBorder, Mouse.GetPosition(myCanvas).X - newBorder.Width / 2);
                Canvas.SetTop(newBorder, Mouse.GetPosition(myCanvas).Y - newBorder.Height / 2);


                myCanvas.Children.Add(newBorder);
                adorner.Add(new BorderAdorner(newBorder));
                
            }
        }
        private void Remove_Selected_Item(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Border)
            {
                Border activeBorder = (Border)e.OriginalSource;

                myCanvas.Children.Remove(activeBorder);
                adorner.Remove(new BorderAdorner(activeBorder));
            }
        }

        private void Save_Canvas(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "RevomApp Files | *.revomap";
            saveFileDialog.DefaultExt = "revomap";

            if (saveFileDialog.ShowDialog() == true)
            {
                //string xaml = System.Windows.Markup.XamlWriter.Save(myCanvas.Children);
                //File.WriteAllText(saveFileDialog.FileName, xaml);
                FileStream fs = File.Open(saveFileDialog.FileName, FileMode.Create);
                System.Windows.Markup.XamlWriter.Save(myCanvas, fs);
                fs.Close();
            }
        }

        private void Load_Canvas(object sender, RoutedEventArgs e)
        {

            //clear current canvas
            Clear_Canvas(sender, e);

            //if canvas was cleared
            if(myCanvas.Children.Count == 0) 
            {
                //get filepath by userdialog
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "RevomApp Files | *.revomap";
                openFileDialog.DefaultExt = "revomap";

                if (openFileDialog.ShowDialog() == true)
                {
                    //get Canvas from file
                    FileStream fs = File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
                    Canvas savedCanvas = System.Windows.Markup.XamlReader.Load(fs) as Canvas;
                    fs.Close();

                

                    //insert elements from savedCanvas into current canvas
                    List<UIElement> temp = new List<UIElement>();
                    foreach (UIElement element in savedCanvas.Children)
                    {
                        temp.Add(element);
                    }
                    savedCanvas.Children.Clear();
                    foreach (UIElement el in temp)
                    {

                        this.myCanvas.Children.Add(el);
                    }
                }
            }
            
        }
        private void Clear_Canvas(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Would you like to save your changes?", "Save your Changes?", System.Windows.MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    Save_Canvas(sender, e);
                    break;
                case MessageBoxResult.No:
                    myCanvas.Children.Clear();
                    break;
                default:
                    break;
            }
        }
    }
}
