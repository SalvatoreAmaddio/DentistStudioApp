using DentistStudioApp.Controller;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowController _controller;
        public MainWindow()
        {
            InitializeComponent();
            _controller = new(this);
        }

    }
}
