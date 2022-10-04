
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;

namespace RevomApp
{
    public class BorderAdorner : Adorner
    {
        private double angle = 0.0;
        private Point transformOrigin = new Point(0, 0);
        private Rectangle childElement;
        private VisualCollection visualChilderns;
        public Thumb center, leftTop, rightTop, leftBottom, rightBottom, left, right, top, bottom;
        
        private bool dragStarted = false;
        private bool isHorizontalDrag = false;

        private Point _posInRect; //saves the MousePosition within a Rectangle before the movement

        public BorderAdorner(UIElement element) : base(element)
        {
            visualChilderns = new VisualCollection(this);
            childElement = element as Rectangle;

            // define ThumbParts and add DragDelta+ShiftKey behaviour 
            CreateThumbPart(ref center);
            center.DragDelta += (sender, e) =>
            {
                double hor = e.HorizontalChange;
                double vert = e.VerticalChange;

                _posInRect = Mouse.GetPosition(childElement);

                Canvas.SetLeft(childElement, Mouse.GetPosition(childElement.Parent as Canvas).X - _posInRect.X + hor);
                Canvas.SetTop(childElement, Mouse.GetPosition(childElement.Parent as Canvas).Y - _posInRect.Y + vert );
                e.Handled = true;
            };
            CreateThumbPart(ref leftTop);
            leftTop.DragDelta += (sender, e) =>
            {
                double hor = e.HorizontalChange;
                double vert = e.VerticalChange;
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    if (dragStarted) isHorizontalDrag = Math.Abs(hor) > Math.Abs(vert);
                    if (isHorizontalDrag) vert = hor; else hor = vert;
                }
                ResizeX(hor);
                ResizeY(vert);
                dragStarted = false;
                e.Handled = true;
            };
            CreateThumbPart(ref rightTop);
            rightTop.DragDelta += (sender, e) =>
            {
                double hor = e.HorizontalChange;
                double vert = e.VerticalChange;

                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    if (dragStarted) isHorizontalDrag = Math.Abs(hor) > Math.Abs(vert);
                    if (isHorizontalDrag) vert = -hor; else hor = -vert;
                }
                ResizeWidth(hor);
                ResizeY(vert);
                dragStarted = false;
                e.Handled = true;
            };
            CreateThumbPart(ref leftBottom);
            leftBottom.DragDelta += (sender, e) =>
            {
                double hor = e.HorizontalChange;
                double vert = e.VerticalChange;

                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    if (dragStarted) isHorizontalDrag = Math.Abs(hor) > Math.Abs(vert);
                    if (isHorizontalDrag) vert = -hor; else hor = -vert;
                }
                ResizeX(hor);
                ResizeHeight(vert);
                dragStarted = false;
                e.Handled = true;
            };
            CreateThumbPart(ref rightBottom);
            rightBottom.DragDelta += (sender, e) =>
            {
                double hor = e.HorizontalChange;
                double vert = e.VerticalChange;
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    if (dragStarted) isHorizontalDrag = Math.Abs(hor) > Math.Abs(vert);
                    if (isHorizontalDrag) vert = hor; else hor = vert;
                }
                ResizeWidth(hor);
                ResizeHeight(vert);
                dragStarted = false;
                e.Handled = true;
            };
            CreateThumbPart(ref left);
            left.DragDelta += (sender, e) =>
            {
                double hor = e.HorizontalChange;
                double vert = e.VerticalChange;
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    ResizeWidth(-hor);
                }
                ResizeX(hor);
                e.Handled = true;
            };
            CreateThumbPart(ref right);
            right.DragDelta += (sender, e) =>
            {
                double hor = e.HorizontalChange;
                double vert = e.VerticalChange;

                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    ResizeX(-hor);
                }
                ResizeWidth(hor);
                e.Handled = true;
            };
            CreateThumbPart(ref top);
            top.DragDelta += (sender, e) =>
            {
                double hor = e.HorizontalChange;
                double vert = e.VerticalChange;
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    ResizeHeight(-vert);
                }
                ResizeY(vert);
                e.Handled = true;
            };
            CreateThumbPart(ref bottom);
            bottom.DragDelta += (sender, e) =>
            {
                double hor = e.HorizontalChange;
                double vert = e.VerticalChange;

                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    ResizeY(-vert);
                }
                ResizeHeight(vert);
                e.Handled = true;
            };
            // define cursors
            center.Cursor = Cursors.SizeAll;
            left.Cursor = Cursors.SizeWE;
            right.Cursor = Cursors.SizeWE;
            top.Cursor = Cursors.SizeNS;
            bottom.Cursor = Cursors.SizeNS;
            leftTop.Cursor = Cursors.SizeNWSE;
            rightTop.Cursor = Cursors.SizeNESW;
            leftBottom.Cursor = Cursors.SizeNESW;
            rightBottom.Cursor = Cursors.SizeNWSE;

        }
        // create collision box of the Adorner
        public void CreateThumbPart(ref Thumb newThumb)
        {
            newThumb = new Thumb { Width = 30, Height = 30, Opacity = 0 };
            newThumb.DragStarted += (object sender, DragStartedEventArgs e) => dragStarted = true;
            visualChilderns.Add(newThumb);
        }

        private void ResizeWidth(double e)
        {
            double deltaHorizontal = Math.Min(-e, childElement.ActualWidth - childElement.MinWidth);
            Canvas.SetTop(childElement, Canvas.GetTop(childElement) - transformOrigin.X * deltaHorizontal * Math.Sin(angle));
            Canvas.SetLeft(childElement, Canvas.GetLeft(childElement) + (deltaHorizontal * transformOrigin.X * (1 - Math.Cos(angle))));
            childElement.Width -= deltaHorizontal;
        }
        private void ResizeX(double e)
        {
            double deltaHorizontal = Math.Min(e, childElement.ActualWidth - childElement.MinWidth);
            Canvas.SetTop(childElement, Canvas.GetTop(childElement) + deltaHorizontal * Math.Sin(angle) - transformOrigin.X * deltaHorizontal * Math.Sin(angle));
            Canvas.SetLeft(childElement, Canvas.GetLeft(childElement) + deltaHorizontal * Math.Cos(angle) + (transformOrigin.X * deltaHorizontal * (1 - Math.Cos(angle))));
            childElement.Width -= deltaHorizontal;
        }
        private void ResizeHeight(double e)
        {
            double deltaVertical = Math.Min(-e, childElement.ActualHeight - childElement.MinHeight);
            Canvas.SetTop(childElement, Canvas.GetTop(childElement) + (transformOrigin.Y * deltaVertical * (1 - Math.Cos(-angle))));
            Canvas.SetLeft(childElement, Canvas.GetLeft(childElement) - deltaVertical * transformOrigin.Y * Math.Sin(-angle));
            childElement.Height -= deltaVertical;
        }
        private void ResizeY(double e)
        {
            double deltaVertical = Math.Min(e, childElement.ActualHeight - childElement.MinHeight);
            Canvas.SetTop(childElement, Canvas.GetTop(childElement) + deltaVertical * Math.Cos(-angle) + (transformOrigin.Y * deltaVertical * (1 - Math.Cos(-angle))));
            Canvas.SetLeft(childElement, Canvas.GetLeft(childElement) + deltaVertical * Math.Sin(-angle) - (transformOrigin.Y * deltaVertical * Math.Sin(-angle)));
            childElement.Height -= deltaVertical;
        }


        //Overrides the adorners visual points when the user resized the UIElement
        //MS Doc SimpleCircleAdorner => https://docs.microsoft.com/de-de/dotnet/desktop/wpf/controls/how-to-implement-an-adorner?view=netframeworkdesktop-4.8
        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect rec = new Rect(this.AdornedElement.DesiredSize);

            // Some arbitrary drawing implements.
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.White);
            renderBrush.Opacity = 1;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Gray), 2.5);
            double circleRadius = 5.0;
            double rectLength = 30;
            double collisionOffset = rectLength/2;

            // Render White Rectangle Outline
            //Border border = new Border();
            //border.BorderBrush = renderBrush;
            //border.BorderThickness = new Thickness(10);
            //border.Child = childElement;

            // Points pointing to the middle of each side.
            Point m_top = new Point((rec.TopLeft.X + rec.TopRight.X) / 2, (rec.TopLeft.Y + rec.TopRight.Y) / 2);
            Point m_bottom = new Point((rec.BottomLeft.X + rec.BottomRight.X) / 2, (rec.BottomLeft.Y + rec.BottomRight.Y) / 2);
            Point m_left = new Point((rec.TopLeft.X + rec.BottomLeft.X) / 2, (rec.TopLeft.Y + rec.BottomLeft.Y) / 2);
            Point m_right = new Point((rec.TopRight.X + rec.BottomRight.X) / 2, (rec.TopRight.Y + rec.BottomRight.Y) / 2);

            // Arrange Collision Boxes at each side.
            left.Arrange(new Rect(rec.TopLeft.X - collisionOffset, rec.TopLeft.Y, rectLength, rec.Height));
            left.Height = rec.Height-rectLength;
            right.Arrange(new Rect(rec.TopRight.X - collisionOffset, rec.TopRight.Y, rectLength, rec.Height));
            right.Height = rec.Height-rectLength;
            top.Arrange(new Rect(rec.TopLeft.X, rec.TopLeft.Y - collisionOffset, rec.Width, rectLength));
            top.Width = rec.Width-rectLength;
            bottom.Arrange(new Rect(rec.BottomLeft.X, rec.BottomLeft.Y -collisionOffset, rec.Width, rectLength));
            bottom.Width = rec.Width-rectLength;

            // Arrange Collision Boxes at each corner.
            leftTop.Arrange(new Rect(rec.TopLeft.X - collisionOffset, rec.TopLeft.Y - collisionOffset, rectLength, rectLength));
            rightTop.Arrange(new Rect(rec.TopRight.X - collisionOffset, rec.TopRight.Y - collisionOffset, rectLength, rectLength));
            leftBottom.Arrange(new Rect(rec.BottomLeft.X - collisionOffset, rec.BottomLeft.Y - collisionOffset, rectLength, rectLength));
            rightBottom.Arrange(new Rect(rec.BottomRight.X - collisionOffset, rec.BottomRight.Y - collisionOffset, rectLength, rectLength));

            // Arrange center CollisionBox for movability
            center.Arrange(new Rect(rec.TopLeft.X+collisionOffset,rec.TopLeft.Y+collisionOffset, top.Width, left.Height));
            center.Width = top.Width;
            center.Height = left.Height;

            // Draw a circle at each corner.
            drawingContext.DrawEllipse(renderBrush, renderPen, rec.TopLeft, circleRadius, circleRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, rec.TopRight, circleRadius, circleRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, rec.BottomLeft, circleRadius, circleRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, rec.BottomRight, circleRadius, circleRadius);
            // Draw a circle at the middle of each side.
            drawingContext.DrawEllipse(renderBrush, renderPen, m_top, circleRadius, circleRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, m_bottom, circleRadius, circleRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, m_left, circleRadius, circleRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, m_right, circleRadius, circleRadius);
        }
        protected override int VisualChildrenCount => visualChilderns.Count;
        protected override Visual GetVisualChild(int index) => visualChilderns[index];
    }
}