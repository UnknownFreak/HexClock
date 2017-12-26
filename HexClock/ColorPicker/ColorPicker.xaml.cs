using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DesktopApplication.ColorPicker
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    ///
    public static class Extension
    {

        public static Color Blend(this Color color, Color backColor, double amount)
        {
            byte r = (byte)((color.R * amount) + backColor.R * (1 - amount));
            byte g = (byte)((color.G * amount) + backColor.G * (1 - amount));
            byte b = (byte)((color.B * amount) + backColor.B * (1 - amount));
            return Color.FromRgb(r, g, b);
        }

        public static HsvColor ToHsv(this Color color)
        {
            HsvColor hsv = new HsvColor();

            float r = color.R / 255.0f;
            float g = color.G / 255.0f;
            float b = color.B / 255.0f;


            float min, max, delta;

            min = r < g ? r : g;
            min = min < b ? min : b;

            max = r > g ? r : g;
            max = max > b ? max : b;

            hsv.v = max;                                // v
            delta = max - min;
            if (delta < 0.00001f)
            {
                hsv.s = 0;
                hsv.h = 0; // undefined, maybe nan?
                return hsv;
            }
            if (max > 0.0f)
            { // NOTE: if Max is == 0, this divide would cause a crash
                hsv.s = (delta / max);                  // s
            }
            else
            {
                // if max is 0, then r = g = b = 0
                // s = 0, v is undefined
                hsv.s = 0.0f;
                hsv.h = float.NaN;                            // its now undefined
                return hsv;
            }
            if (r >= max)                           // > is bogus, just keeps compilor happy
                hsv.h = (g - b) / delta;        // between yellow & magenta
            else
        if (g >= max)
                hsv.h = 2.0f + (b - r) / delta;  // between cyan & yellow
            else
                hsv.h = 4.0f + (r - g) / delta;  // between magenta & cyan

            hsv.h *= 60.0f;                              // degrees

            if (hsv.h < 0.0f)
                hsv.h += 360.0f;

            return hsv;

        }
    }

    public class HsvColor
    {
        public float h;
        public float s;
        public float v;
    }

    public partial class ColorPicker : Window
    {
        public ColorPicker()
        {
            InitializeComponent();
        }

        private double yLine = 0.0;
        private double xLine = 0.0;
        private Color barColor = Colors.Red;

        private bool allowBarMove = false;
        private bool allowGridMove = false;

        private bool restoreColor = false;

        public void updateColorFromDataContext()
        {
            //Color col = (Color)DataContext;
            Color col = (DataContext as SolidColorBrush).Color;
            if (col != null)
            {
                setColor(col);
            }
        }

        private void tryUpdateDataContext(Color c)
        {
            (DataContext as SolidColorBrush).Color = c;
        }

        private void setColor(Color col)
        {
            HsvColor hsv = col.ToHsv();

            if(!float.IsNaN(hsv.h))
            {
                double pz = (hsv.h / 360.0) * ColorBar.ActualHeight;

                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(new TranslateTransform(0, pz));

                ColorMarker.RenderTransform = transformGroup;

                Color[] colors = { Colors.Red, Colors.Yellow, Colors.Lime, Colors.Cyan, Colors.Blue, Colors.Magenta, Colors.Red };
                int nrElements = colors.Length - 1;

                double per = pz / 255.0;
                int index = (int)((double)nrElements * per);
                int index2 = Math.Min((int)((double)nrElements * per) + 1, nrElements);
                double per2 = per * (double)nrElements; // 0.0 - 6.0

                barColor = colors[index].Blend(colors[index2], 1.0 - (per2 - index));

                SelectorColor.Color = barColor;

            }

            xLine = hsv.s;
            yLine = hsv.v;

            {
                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(new TranslateTransform(xLine * ColorGrid.ActualWidth, -yLine * ColorGrid.ActualHeight));

                Selector.RenderTransform = transformGroup;
            }
            calculateColor();
            oldColor.Fill = new SolidColorBrush(col);
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            updateGridLines();
            calculateColor();
            allowGridMove = true;
        }

        private void ColorGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && allowGridMove)
            {
                updateGridLines();
                calculateColor();
            }
            else
            {
                allowGridMove = false;
            }
        }

        private void ColorBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            updateDefiningColor();
            calculateColor();
            allowBarMove = true;
        }

        private void ColorBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && allowBarMove)
            {
                updateDefiningColor();
                calculateColor();
            }
            else
            {
                allowBarMove = false;
            }
        }

        private void updateDefiningColor()
        {
            Point p = Mouse.GetPosition(ColorBar);

            double pz = Math.Min(Math.Max(p.Y, 0), ColorBar.ActualHeight);

            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new TranslateTransform(0, pz));

            ColorMarker.RenderTransform = transformGroup;

            Color[] colors = { Colors.Red, Colors.Yellow, Colors.Lime, Colors.Cyan, Colors.Blue, Colors.Magenta, Colors.Red };
            int nrElements = colors.Length - 1;

            double per = pz / 255.0;
            int index = (int)((double)nrElements * per);
            int index2 = Math.Min((int)((double)nrElements * per) + 1, nrElements);
            double per2 = per * (double)nrElements; // 0.0 - 6.0

            barColor = colors[index].Blend(colors[index2], 1.0 - (per2 - index));

            SelectorColor.Color = barColor;

        }

        private void updateGridLines()
        {
            Point p = Mouse.GetPosition(ColorGrid);

            double h = ColorGrid.ActualHeight;
            double w = ColorGrid.ActualWidth;

            double px = p.X;
            double py = -h + p.Y;

            px = Math.Min(Math.Max(px, 0), w);
            py = Math.Max(Math.Min(py, 0), -h);
            
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new TranslateTransform(px, py));

            Selector.RenderTransform = transformGroup;

            double per = (py * -1.0) / 255.0;
            yLine = per;

            per = px / 255.0;
            xLine = per;

        }

        private void calculateColor()
        {
            Color whiteColor = Colors.White;
            Color blackColor = Colors.Black;
            Color definingColor = barColor;

            Color c = whiteColor;
            c = c.Blend(blackColor, yLine);
            definingColor = definingColor.Blend(blackColor, yLine);

            c = definingColor.Blend(c, xLine);

            selectedColor.Fill = new SolidColorBrush(c);
            tryUpdateDataContext(c);
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            restoreColor = true;
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (restoreColor)
            {
                Color c = (oldColor.Fill as SolidColorBrush).Color;
                tryUpdateDataContext(c);
            }
        }

        private void confirm_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (allowBarMove)
                {
                    updateDefiningColor();
                    calculateColor();
                }
                if (allowGridMove)
                {
                    updateGridLines();
                    calculateColor();
                }
            }
            else
            {
                allowGridMove = false;
                allowBarMove = false;
            }
        }
    }
}