using Microsoft.Win32;
using System;
using System.Collections;
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
using System.Windows.Markup;

namespace RevomApp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AdornerLayer adornerLayer;
        bool drawAllowed = false;
        BorderAdorner activeBorder;
        Rectangle activeRect;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (sender, e) => {
                adornerLayer = AdornerLayer.GetAdornerLayer(myCanvas);
            };
        }
        //_____________________________________________________________________________________________________________
        //_____________________________________Rectangle Functions_____________________________________________________
        //_____________________________________________________________________________________________________________
        private void Draw_Rect(object sender, MouseButtonEventArgs e)
        {
            if (drawAllowed)
            {
                Random r = new Random();
                Brush customColor = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255)));
                Rectangle newRect = new Rectangle() {
                    Width = 100,
                    Height = 100,
                    Fill = customColor,
                    AllowDrop = true,
                    RadiusX = 50,
                    RadiusY = 50,
                    MinWidth = 100,
                    MinHeight = 100,
                    //MaxWidth = myCanvas.Width,
                    //MaxHeight = myCanvas.Height,
                    Cursor = Cursors.Hand
                };
                Canvas.SetLeft(newRect, Mouse.GetPosition(myCanvas).X - newRect.Width / 2);
                Canvas.SetTop(newRect, Mouse.GetPosition(myCanvas).Y - newRect.Height / 2);

                // Add rect to Canvas
                myCanvas.Children.Add(newRect);

                // Set Focus on the newRect
                Set_Rect_Focus(newRect);

                // It's not allowed to Draw anymore
                Change_Drawstate(sender, e);
            }
        }
        private void Set_Rect_Focus(Rectangle r)
        {
            // Set given Rect as Active
            activeRect = r;

            //clear previous border
            RemoveBorderAdorner(activeBorder);

            //create new BorderAdorner and add it to the AdornerLayer
            activeBorder = new BorderAdorner(activeRect);
            adornerLayer.Add((Adorner)activeBorder);
        }
        private void RemoveBorderAdorner(BorderAdorner borderAdorner)
        {
            if (activeBorder != null)
            {
                AdornerLayer aL = AdornerLayer.GetAdornerLayer(activeBorder);
                if (aL != null)
                {
                    aL.Remove(activeBorder);
                }
            }
        }
        // changes the drawAllowed variable and the look of the New_Rec_Button
        private void Change_Drawstate(object sender, RoutedEventArgs e)
        {
            drawAllowed = !drawAllowed;
            Button_New_Rec.IsChecked = drawAllowed;
        }
        private void Remove_Selected_Item(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Rectangle)
            {
                Rectangle activeRect = (Rectangle)e.OriginalSource;

                myCanvas.Children.Remove(activeRect);
            }
        }
        //_____________________________________________________________________________________________________________
        //_______________________________________Canvas Functions______________________________________________________
        //_____________________________________________________________________________________________________________
        private void Save_Canvas(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "RevomApp Files | *.revomap";
            saveFileDialog.DefaultExt = "revomap";

            if (saveFileDialog.ShowDialog() == true)
            {
                FileStream fs = File.Open(saveFileDialog.FileName, FileMode.Create);
                System.Windows.Markup.XamlWriter.Save(myCanvas, fs);
                fs.Close();
            }
        }
        private void Load_Canvas(object sender, RoutedEventArgs e)
        {
            //clear current canvas
            if (myCanvas.Children.Count != 0)
            {
                Clear_Canvas(sender, e);
            }
            //if canvas is clear
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
            MessageBoxResult result = System.Windows.MessageBox.Show("Would you like to save your changes?",
                                                                     "Save your Changes?",
                                                                     System.Windows.MessageBoxButton.YesNoCancel);
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
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                // Escape is pressed
                RemoveBorderAdorner(activeBorder);
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if(e.Key == Key.C)
                {
                    // Ctrl + C is pressed
                    // Save Rect to Clipboard
                    string xaml = XamlWriter.Save(activeRect);
                    Clipboard.SetData(DataFormats.Xaml, xaml);
                }
                else if(e.Key == Key.V)
                {
                    // Ctrl + C is pressed
                    // Get Rect from Clipboard
                    string xaml = (string)Clipboard.GetData(DataFormats.Xaml);
                    if (xaml != null)
                    {
                        Rectangle copiedRec = XamlReader.Parse(xaml) as Rectangle;
                        // Add Rect to Canvas and focus on it
                        myCanvas.Children.Add(copiedRec);
                        /*TODO Move the new Rectangle down and right a bit, so it's distingushed from the original */
                        //Canvas.SetLeft(copiedRec, copiedRec.Width);
                        //Canvas.SetTop(copiedRec, copiedRec.Height);
                        Set_Rect_Focus(copiedRec);
                    }
                }
            }
        }
        private void myCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (drawAllowed)
            {
                Draw_Rect(sender, e);
            }
            else if (e.OriginalSource is Rectangle)
            {
                //set clicked-rect as the activeRect
                Rectangle r = e.OriginalSource as Rectangle;
                Set_Rect_Focus(r);
            }
            else // if clicked somewhere else on the canvas
            {
                //clear previous Border if exists
                RemoveBorderAdorner(activeBorder);
                activeRect = null;
            }
        }
    }
}
