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

namespace FashionTrack
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnAddNewUser_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegisterWindow registerNewuser = new RegisterWindow();
            registerNewuser.Show();
        }

        private void btnAddNewUser_MouseEnter(object sender, MouseEventArgs e)
        {

        }
    }
}
