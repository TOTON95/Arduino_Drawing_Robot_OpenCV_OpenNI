//CREATED BY TOTON (ALEXIS GUIJARRO)@2016. FOR EDUCATIONAL PURPOSES

﻿using System;
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

namespace WpfApplication1
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point puntoactual = new Point();
        ARDUINO_CNC.Control control; 
        Cursor lapiz;
        
        //archivo de texto G-CODE;
        //string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public MainWindow()
        {
            InitializeComponent();
            control = new ARDUINO_CNC.Control(Dibujo);
            control.Show();

        }

        private void Dibujo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ButtonState == MouseButtonState.Pressed)
            {
                puntoactual = e.GetPosition(this);
            }
        }

        private void Dibujo_MouseMove(object sender, MouseEventArgs e)
        {
            string lineapasada = "";
            string lineaactual = "";
            int z = 0;
            double volteoY = (e.GetPosition(this).Y - 695.00) * (-1);

            if (e.LeftButton == MouseButtonState.Pressed)
            {

                Line linea = new Line();
                linea.Stroke = SystemColors.WindowFrameBrush;
                linea.X1 = puntoactual.X;
                linea.Y1 = puntoactual.Y;
                linea.X2 = e.GetPosition(this).X;
                linea.Y2 = e.GetPosition(this).Y;
                z = 1;

                //MAX coordenadas = x:673 y:695
                //Plano real = x:70 y:78.16
                //Plano PRUEBA = x: 101.82 y: 101.82 => 101
                //Para guardar en escritorio
                //using (StreamWriter outputFile = new StreamWriter(mydocpath + @"\GCODE.txt", true))

                if (Convert.ToInt32(puntoactual.X) != Convert.ToInt32(e.GetPosition(this).X) && Convert.ToInt32(puntoactual.Y) != Convert.ToInt32(e.GetPosition(this).Y))
                {
                    //CNC
                    //lineaactual = "G01 X" + Convert.ToInt32((e.GetPosition(this).X / 9.61)).ToString()+ " Y" + Convert.ToInt32(((volteoY) / 8.89)).ToString() +" Z-2" ;
                    //BRAZO ORIGINAL:
                    //lineaactual = "X" + Convert.ToInt32((e.GetPosition(this).X / 9.61)).ToString() + " Y" + Convert.ToInt32(((volteoY) / 8.89)).ToString() + " Z2";
                    //BRAZO PRUEBAS:
                    lineaactual = "X" + Convert.ToInt32((e.GetPosition(this).X / 7.8)).ToString() + " Y" + Convert.ToInt32(((volteoY) / 8.268)).ToString() + " Z" + Convert.ToString(z);
                    if (lineaactual != lineapasada)
                    {
                        using (StreamWriter outputFile = new StreamWriter("GCODE.txt", true))
                        {
                            //outputFile.WriteLine("G01 X{0} Y{1} Z-2", e.GetPosition(this).X, e.GetPosition(this).Y);
                            //CNC
                            // outputFile.WriteLine("G01 X{0} Y{1} Z-2", Convert.ToInt32((e.GetPosition(this).X) / 9.61), Convert.ToInt32((volteoY) / 8.89));
                            //Brazo
                            //outputFile.WriteLine("X{0} Y{1} Z0", Convert.ToInt32((e.GetPosition(this).X) / 9.61), Convert.ToInt32((volteoY) / 8.89));
                            //Brazo PRUEBAS
                            //outputFile.WriteLine("X{0} Y{1} Z0", Convert.ToInt32((e.GetPosition(this).X) / 6.7), Convert.ToInt32((volteoY) / 6.9));
                            outputFile.WriteLine(lineaactual);
                            outputFile.Close();
                        }
                        lineapasada = lineaactual;
                    }

                }
                puntoactual = e.GetPosition(this);
                Dibujo.Children.Add(linea); 

            }
            if(e.LeftButton == MouseButtonState.Released)
            {
                z = 0;
            }
           /*
                if (Convert.ToInt32(puntoactual.X) != Convert.ToInt32(e.GetPosition(this).X) && Convert.ToInt32(puntoactual.Y) != Convert.ToInt32(e.GetPosition(this).Y))
                {
                    //CNC
                    //lineaactual = "G01 X" + Convert.ToInt32((e.GetPosition(this).X / 7.8)).ToString()+ " Y" + Convert.ToInt32(((volteoY) / 8.268)).ToString() +" Z" + Convert.ToString(z) ;
                    //BRAZO ORIGINAL:
                    //lineaactual = "X" + Convert.ToInt32((e.GetPosition(this).X / 9.61)).ToString() + " Y" + Convert.ToInt32(((volteoY) / 8.89)).ToString() + " Z2";
                    //BRAZO PRUEBAS:
                    lineaactual = "X" + Convert.ToInt32((e.GetPosition(this).X / 7.8)).ToString() + " Y" + Convert.ToInt32(((volteoY) / 8.268)).ToString() + " Z" + Convert.ToString(z);
                    if (lineaactual != lineapasada)
                    {
                        using (StreamWriter outputFile = new StreamWriter("GCODE.txt", true))
                        {
                            outputFile.WriteLine(lineaactual);
                            outputFile.Close();
                        }
                        lineapasada = lineaactual;
                    }
                }
                puntoactual = e.GetPosition(this);
            */
            
        }

        private void HOJA_DE_DIBUJO_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            control.Close();
        }

        private void HOJA_DE_DIBUJO_MouseEnter(object sender, MouseEventArgs e)
        {
            lapiz = Cursors.Pen;
            Mouse.OverrideCursor = lapiz;
        }

        private void HOJA_DE_DIBUJO_MouseLeave(object sender, MouseEventArgs e)
        {
            lapiz = Cursors.Arrow;
            Mouse.OverrideCursor = lapiz;
        }
    }
}
