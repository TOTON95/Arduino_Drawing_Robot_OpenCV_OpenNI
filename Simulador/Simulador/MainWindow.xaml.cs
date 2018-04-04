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
using System.IO;
using System.Threading;

namespace Simulador
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Point origen = new Point(50, 680);
        Point origen = new Point(240, 650);
        Point medio = new Point();

        double L1 = 330.00; //82mm
        double L2 = 249.51; //62mm

        double Th1;
        double Th2;

        Line A = new Line();
        Line B = new Line();
        Ellipse inicio = new Ellipse();
        Ellipse union = new Ellipse();
        Ellipse pluma = new Ellipse();

        SolidColorBrush scb = new SolidColorBrush();

        public MainWindow()
        {
            InitializeComponent();
            CrearBrazo();
            Angulo(-90 , 180);
            Slider.Value = 90;
            Slider_Copy.Value = 0;
            CrearGrafica();
        }
        public void CrearBrazo()
        {
            A.Stroke = SystemColors.WindowTextBrush;
            B.Stroke = SystemColors.WindowTextBrush;

            
            inicio.Stroke = SystemColors.ScrollBarBrush;
            inicio.StrokeThickness = 2;
            inicio.Fill = SystemColors.ScrollBarBrush;
            inicio.Width = 20;
            inicio.Height = 20;
            
            union.Stroke = SystemColors.ScrollBarBrush;
            union.StrokeThickness = 2;
            union.Fill = SystemColors.ScrollBarBrush;
            union.Width = 20;
            union.Height = 20;

            scb.Color = Color.FromRgb(0, 0, 255);
            pluma.Stroke = scb;
            pluma.StrokeThickness = 2;
            pluma.Fill = scb;
            pluma.Width = 10;
            pluma.Height = 10;


            A.X1 = origen.X;
            A.Y1 = origen.Y;
            A.X2 = 50.00;
            A.Y2 = 350.00; 

            B.X1 = A.X2;
            B.Y1 = A.Y2;
            B.X2 = 50.00;
            B.Y2 = 599.51;

           
            Canvas.SetLeft(inicio, A.X1-(inicio.Width/2));
            Canvas.SetTop(inicio, A.Y1-(inicio.Height/2));
            Canvas.SetLeft(union, A.X2 - (union.Width / 2));
            Canvas.SetTop(union, A.Y2 - (union.Height / 2));
            Canvas.SetLeft(pluma, B.X2 - (pluma.Width / 2));
            Canvas.SetTop(pluma, B.Y2 - (pluma.Height / 2));
            
            Dibujo.Children.Add(A);
            Dibujo.Children.Add(B);
            Dibujo.Children.Add(inicio);
            Dibujo.Children.Add(union);
            Dibujo.Children.Add(pluma);
        }
        private void Angulo(double th1, double th2)
    {
        //	x = L1 × cos( th1°)
        //  y = L2 × sin( th1°)
        double ang1 = ((Math.PI / 180.0) * th1);
        double ang2 = ((Math.PI / 180.0) * (th2-10));
        double x1 = origen.X + (L1 * Math.Cos(ang1));
        double y1 = origen.Y + (L1 * Math.Sin(ang1));
        A.X2 = x1;
        A.Y2 = y1;
        medio.X = x1;
        medio.Y = y1;
        B.X1 = medio.X;
        B.Y1 = medio.Y;
        Canvas.SetLeft(union, medio.X - (union.Width / 2));
        Canvas.SetTop(union, medio.Y - (union.Height / 2));
        double x2 = medio.X + (L2 * Math.Cos(ang2+ang1));
        double y2 = medio.Y + (L2 * Math.Sin(ang2+ang1));
        B.X2 = x2;
        B.Y2 = y2;
        Canvas.SetLeft(pluma, B.X2 - (pluma.Width / 2));
        Canvas.SetTop(pluma, B.Y2 - (pluma.Height / 2));

        Trazo(B.X2, B.Y2);
    }

        private void Trazo(double p1, double p2)
        {
            Line linea = new Line();
            linea.Stroke = scb;
            linea.X1 = p1;
            linea.Y1 = p2;
            linea.X2 = p1 - 2;
            linea.Y2 = p2 - 2;
            Dibujo.Children.Add(linea);
            /*Ellipse circulo = new Ellipse();
            circulo.Stroke = scb;
            circulo.StrokeThickness = 2;
            circulo.Fill = scb;
            circulo.Width = 5;
            circulo.Height = 5;
            Canvas.SetLeft(circulo, p1 - (circulo.Width/2));
            Canvas.SetTop(circulo, p2 - (circulo.Height/2));
            Dibujo.Children.Add(circulo);*/

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Th1 = Slider.Value;
            Th2 = Slider_Copy.Value;
            Angulo((-1)*Th1,180-Th2);
            th1.Content = "TH1: " + Th1.ToString() + "º";
            th2.Content = "TH2: " + Th2.ToString() + "º";
        }

        private void btn_limpiar_Click(object sender, RoutedEventArgs e)
        {
            Dibujo.Children.Clear();
            Dibujo.Children.Add(th1);
            Dibujo.Children.Add(th2);
            Dibujo.Children.Add(Slider);
            Dibujo.Children.Add(Slider_Copy);
            Dibujo.Children.Add(btn_limpiar);
            Dibujo.Children.Add(btn_exportar_excel);
            Dibujo.Children.Add(canGraph);
            Slider.Value = 90;
            Slider_Copy.Value = 0;
            CrearBrazo();
            Angulo(-90, 180);
        }
        private void CrearGrafica()
        {
            const double margin = 10;
            double xmin = margin;
            double xmax = canGraph.Width - margin;
            double ymin = margin;
            double ymax = canGraph.Height - margin;
            const double step = 10;

            // Make the X axis.
            GeometryGroup xaxis_geom = new GeometryGroup();
            xaxis_geom.Children.Add(new LineGeometry(
                new Point(0, ymax), new Point(canGraph.Width, ymax)));
            for (double x = xmin + step;
                x <= canGraph.Width - step; x += step)
            {
                xaxis_geom.Children.Add(new LineGeometry(
                    new Point(x, ymax - margin / 2),
                    new Point(x, ymax + margin / 2)));
            }

            System.Windows.Shapes.Path xaxis_path = new System.Windows.Shapes.Path();
            xaxis_path.StrokeThickness = 1;
            xaxis_path.Stroke = Brushes.Black;
            xaxis_path.Data = xaxis_geom;

            canGraph.Children.Add(xaxis_path);

            // Make the Y ayis.
            GeometryGroup yaxis_geom = new GeometryGroup();
            yaxis_geom.Children.Add(new LineGeometry(
                new Point(xmin, 0), new Point(xmin, canGraph.Height)));
            for (double y = step; y <= canGraph.Height - step; y += step)
            {
                yaxis_geom.Children.Add(new LineGeometry(
                    new Point(xmin - margin / 2, y),
                    new Point(xmin + margin / 2, y)));
            }

            System.Windows.Shapes.Path yaxis_path = new System.Windows.Shapes.Path();
            yaxis_path.StrokeThickness = 1;
            yaxis_path.Stroke = Brushes.Black;
            yaxis_path.Data = yaxis_geom;

            canGraph.Children.Add(yaxis_path);

            // Make some data sets.
            Brush[] brushes = { Brushes.Red, Brushes.Green, Brushes.Blue };
            Random rand = new Random();
            for (int data_set = 0; data_set < 3; data_set++)
            {
                int last_y = rand.Next((int)ymin, (int)ymax);

                PointCollection points = new PointCollection();
                for (double x = xmin; x <= xmax; x += step)
                {
                    last_y = rand.Next(last_y - 10, last_y + 10);
                    if (last_y < ymin) last_y = (int)ymin;
                    if (last_y > ymax) last_y = (int)ymax;
                    points.Add(new Point(x, last_y));
                }

                Polyline polyline = new Polyline();
                polyline.StrokeThickness = 1;
                polyline.Stroke = brushes[data_set];
                polyline.Points = points;

                canGraph.Children.Add(polyline);
            }
        }

        private void btn_exportar_excel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
