using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RadioThermostat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Filtrete.D3M50 device = new Filtrete.D3M50("192.168.0.106");

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitTimer();
        }

        private void InitTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(5000);
            timer.Tick += new EventHandler(timerEventHandler);
            timer.Start();

        }

        private void timerEventHandler(Object sender, EventArgs args)
        {
            lblActualTemp.Content = Math.Round(device.Temperature, 1);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            device.Thermostat = Filtrete.D3M50.Mode.Cool;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            device.Thermostat = Filtrete.D3M50.Mode.Off;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            device.Thermostat = Filtrete.D3M50.Mode.Heat;
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            device.Thermostat = Filtrete.D3M50.Mode.Auto;
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            device.Fan = Filtrete.D3M50.FanMode.On;
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            device.Fan = Filtrete.D3M50.FanMode.Auto;
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            device.Fan = Filtrete.D3M50.FanMode.AutoCirculate;
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            device.TargetHeat = float.Parse(textBox1.Text);

        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            device.TargetCool = float.Parse(textBox2.Text);
        }

        private void radF_Checked(object sender, RoutedEventArgs e)
        {
            device.Unit = Filtrete.D3M50.UnitofMeasure.Fahrenheit;
        }

        private void radC_Checked(object sender, RoutedEventArgs e)
        {
            device.Unit = Filtrete.D3M50.UnitofMeasure.Celsius;
        }


      



    }
}
