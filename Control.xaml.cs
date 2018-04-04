//CREATED BY TOTON (ALEXIS GUIJARRO)@2016. FOR EDUCATIONAL PURPOSES

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
using System.Windows.Shapes;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;




namespace ARDUINO_CNC
{
    /// <summary>
    /// Lógica de interacción para Control.xaml
    /// </summary>
    public partial class Control : Window
    {
        public string[] ports = SerialPort.GetPortNames();
        public SerialPort arduino = new SerialPort();
        public Canvas dibujo;
        public int counter = 0;
        public int total = 0;
        public bool bandera = false;
        private int counter_deseado = 0;

      //OPENCV KINECT\\
        static Process opencvkinect = new Process();
        static ProcessStartInfo startinfo = new ProcessStartInfo();
        bool opkactivo = false;

      //======//InverseKinematics\\======\\
        private static double L1 = 82;
        private static double L2 = 62;

        //Correcciones
        static double correccion_S1 = 1;
        static double correccion_S2 = 1;
        static double S_1_CorrectionFactor = +8.3;
        static double S_2_CorrectionFactor = -20.2;
        static double X_CorrectionFactor = 42;
        static double Y_CorrectionFactor = -27;
        //10 YCORR
        //3 YCORR
        //+7 S1
        //-22 S2
        //42 XCORR
        //50 XCORR

        //Funciona Y correct: -18

        //Angulos
        double A;            
        double B;            
        double C;            
        double theta;

        //Distancias
        static double x;           
        static double y;            
        static double c;            
        static double pi = Math.PI;

        public struct Angulo
        {
            public double th2;
            public double th1;
        }
        
        public Control(Canvas dibujos)
        {
            InitializeComponent();
            btn_Parar.IsEnabled = false;
            btn_Enviar.IsEnabled = false;
            dibujo = dibujos;
            prepararOpencv();
            opencvkinect.Exited += opencvkinect_Exited;
        }

        void opencvkinect_Exited(object sender, EventArgs e)
        {
            opkactivo = false;
        }

        private void prepararOpencv()
        {
            opencvkinect.StartInfo.FileName = @"path\to\arduinocncopencv.exe";
        }
        public void SerialInit(string portselected)
        {
            arduino.PortName = portselected;
            arduino.BaudRate = 9600;
            arduino.DataReceived += new SerialDataReceivedEventHandler(arduino_DataReceived);
            try
            {
                arduino.Open();
                label2.Content = "Conectado a Arduino en el puerto " + portselected;
                if(!label2.IsVisible)
                {
                    Action act = () => { label2.Visibility = Visibility.Visible; };
                    label2.Dispatcher.Invoke(act);
                }
                ellipse1.Visibility = Visibility.Visible;

            }

            catch (InvalidOperationException) { }
            catch (ArgumentOutOfRangeException) { }
            catch (ArgumentException) { }
            catch (System.IO.IOException) { }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Puerto en Uso por Otra Aplicación","ERROR",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        void arduino_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (arduino.IsOpen)
            {
                try
                {
                    if (!arduino.BreakState)
                    {
                        arduino.WriteLine("+");
                        string data = arduino.ReadLine();
                        if (data == "OK\r")
                        {
                            string line;
                            string equis = "";
                            string ye = "";
                            string zeta = "";
                            string pack = "";
                            int x;
                            int y;
                            int z;

                            int cambio = 0;

                            System.IO.StreamReader file = new System.IO.StreamReader("GCODE.txt");
                            while ((line = file.ReadLine()) != null)
                            {
                                if (bandera == true)
                                {
                                    if (arduino.IsOpen)
                                    {
                                        arduino.BreakState = true;
                                    }
                                    break;
                                }
                                foreach (char c in line)
                                {
                                    if (bandera == true)
                                    {
                                        break;
                                    }
                                    if (c == 'X')
                                    {
                                        cambio = 1;
                                    }
                                    if (c == 'Y')
                                    {
                                        cambio = 2;
                                    }
                                    if (c == 'Z')
                                    {
                                        cambio = 3;
                                    }
                                    if (Char.IsDigit(c))
                                    {
                                        if (cambio == 1)
                                        {
                                            equis += c;
                                        }
                                        if (cambio == 2)
                                        {
                                            ye += c;
                                        }
                                        if (cambio == 3)
                                        {
                                            zeta += c;
                                        }
                                    }
                                }
                                x = Convert.ToInt32(equis);
                                y = Convert.ToInt32(ye);

                                Angulo grados;
                                //grados = CalcularAng2(x + 42, y);
                                grados = CalcularAng2(x, y);

                                pack = "A" + Convert.ToInt32(grados.th1) + "B" + Convert.ToInt32(grados.th2) +
                                    "Z" + zeta + "*";
                                if (arduino.IsOpen)
                                {
                                    arduino.WriteLine(pack);
                                }
                                counter++;
                                ///CHECAR SINCRONIZACION
                                Thread.Sleep(250);
                                pack = "";
                                equis = "";
                                ye = "";
                                zeta = "";
                            }
                            file.Close();
                        }

                    }
                }
                catch (System.IO.IOException)
                {
                    Action act = () => { label2.Visibility = Visibility.Hidden; };
                    label2.Dispatcher.Invoke(act);
                    Action act1 = () => { ellipse1.Visibility = Visibility.Hidden; };
                    ellipse1.Dispatcher.Invoke(act1);
                    MessageBox.Show("Se ha perdido la conexión", "Deteniendo", MessageBoxButton.OK, MessageBoxImage.Stop);
                    bandera = true;
                    Action act2 = () => { btn_Enviar.IsEnabled = false; };
                    btn_Enviar.Dispatcher.Invoke(act2);
                    Action act3 = () => { btn_Parar.IsEnabled = false; };
                    btn_Parar.Dispatcher.Invoke(act3);
                    ports = new string[0];
                    Action act4 = () => { comboBox1.ItemsSource = ports; };
                    comboBox1.Dispatcher.Invoke(act4);
                    Action act5 = () => { up.Children.Clear(); };
                    comboBox1.Dispatcher.Invoke(act5);
                    counter = 0;

                }
                //arduino.DiscardInBuffer();
                //arduino.DiscardOutBuffer();
            }
        }
        
        private void Window_Closed(object sender, EventArgs e)
        {

            Application.Current.Shutdown();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                string selected = comboBox1.SelectedItem.ToString();
                if (arduino.IsOpen == false)
                {
                    SerialInit(selected);
                }
                else
                {
                    try
                    {
                        arduino.Close();
                    }
                    catch (InvalidOperationException) {}
                    ellipse1.Visibility = Visibility.Hidden;
                    SerialInit(selected);
                }
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            ports = SerialPort.GetPortNames();
            comboBox1.ItemsSource = ports;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (arduino.IsOpen)
            {
                try
                {
                   //arduino.WriteLine("A97B15*");
                    arduino.DiscardInBuffer();
                    arduino.DiscardOutBuffer();
                    arduino.Close();
                    arduino.Dispose();
                }
                catch (InvalidOperationException) { }
            }
            Process[] pname = Process.GetProcessesByName("arduinocncopencv");
            if (pname.Length != 0)
            {
                opencvkinect.Kill();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ports = SerialPort.GetPortNames();
            comboBox1.ItemsSource = ports;
        }

        private TextBlock texto(string texto)
        {
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE8BF05"));
            TextBlock sd = new TextBlock();
            sd.Foreground = mySolidColorBrush;
            sd.Background = Brushes.Black;
            sd.FontSize = 48;
            sd.FontFamily = new FontFamily("Consolas");
            sd.Text = texto;
            return sd;
        }

        private void Cargar(object sender, RoutedEventArgs e)
        {
            up.Children.Clear();
            string line;
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader("GCODE.txt");
                while ((line = file.ReadLine()) != null)
                {
                    up.Children.Add(texto(counter.ToString() + "|" + "  " + line));
                    counter++;
                }
                file.Close();
                total = counter;
                counter = 0;
                if (arduino.IsOpen)
                {
                    btn_Enviar.IsEnabled = true;
                }
            }
            catch(System.IO.FileNotFoundException)
            {
                MessageBox.Show("No se ha dibujado nada", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            

        }

        private void Limpiar(object sender, RoutedEventArgs e)
        {
            try
            {
                System.IO.File.Delete("GCODE.txt");
            }
            catch (System.IO.FileNotFoundException) { }
            catch (System.IO.IOException)
            {
                MessageBox.Show("", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            Process[] pname = Process.GetProcessesByName("arduinocncopencv");
            if (pname.Length != 0)
            {
                pname[0].Kill();
                opencvkinect.Start();
            }
            up.Children.Clear();
            dibujo.Children.Clear();
            btn_Enviar.IsEnabled = false;
            counter = 0;
        }

        private void btn_Enviar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult decision = MessageBox.Show("¿Deseas empezar el dibujo?", "ATENCIÓN", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(decision == MessageBoxResult.Yes)
            {
                if (arduino.IsOpen)
                {
                    if (arduino.BreakState)
                    {
                        arduino.BreakState = false;
                    }
                    btn_Parar.IsEnabled = true;
                    btn_Enviar.IsEnabled = false;

                    arduino.WriteLine("+");
                }
                
            }
            if (decision == MessageBoxResult.No)
            {
               
            }

        }

        private void btn_Parar_Click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult fd = MessageBox.Show("¿Quieres detener el proceso?", "¿Parar el Proceso?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (fd == MessageBoxResult.Yes)
            {
                btn_Parar.IsEnabled = false;
                bandera = true;
                if (arduino.IsOpen)
                {
                    //arduino.WriteLine("A90B10*");
                    arduino.WriteLine("A97B15Z0*");
                }
            }
        }

        private Angulo CalcularAng2(double x, double y)
        {
            Angulo valor = new Angulo();
            x = x + X_CorrectionFactor;
            y = y + Y_CorrectionFactor;
            c = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
            B = (Math.Acos((Math.Pow(L2, 2) - Math.Pow(L1, 2) - Math.Pow(c, 2)) / (-2 * L1 * c))) * (180 / pi);
            C = (Math.Acos((Math.Pow(c, 2) - Math.Pow(L2, 2) - Math.Pow(L1, 2)) / (-2 * L1 * L2))) * (180 / pi);
            theta = (Math.Asin(y / c)) * (180 / pi);
            valor.th1 = B + theta + S_1_CorrectionFactor;
            valor.th2 = C + S_2_CorrectionFactor;
            return valor;

        }

        private void btn_Enviar_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bandera = false;
        }

        private void btn_camara_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"path\to\arduinocncopencv.exe");
            if (opkactivo == false)
            {
                opencvkinect.Start();
                opkactivo = true;
            }
        }

    
    }
}
