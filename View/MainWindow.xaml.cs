using Backend.Utils;
using DentistStudioApp.Controller;
using FrontEnd.ExtensionMethods;
using FrontEnd.Utils;
using System.Windows;

namespace DentistStudioApp.View
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowController _controller;
        public MainWindow()
        {
            InitializeComponent();
            Helper.ManageTabClosing(MainTab);
            Curtain.SoftwareInfo = new SoftwareInfo("Salvatore Amaddio", "www.salvatoreamaddio.co.uk", "Mister J", "2024");
            _controller = new(this);
        }

    }
}
