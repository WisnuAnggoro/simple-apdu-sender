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

namespace SimpleApduSender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SCardWrapper scWrapper;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DisplayErrorMessage(
            string message)
        {
            MessageBox.Show(
                message, 
                this.Title, 
                MessageBoxButton.OK, 
                MessageBoxImage.Error);
        }

        private void RefreshReaderList()
        {
            try
            {
                List<string> readers = scWrapper.GetReaderList();

                cboReader.Items.Clear();

                if (readers == null)
                    DisplayErrorMessage(scWrapper.LastErrorString);
                else
                {
                    foreach (string reader in readers)
                    {
                        cboReader.Items.Add(reader);
                    }
                    cboReader.SelectedIndex = 0;
                }

                cboReader.IsEnabled = cboReader.Items.Count > 0;
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(
                    ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            scWrapper = new SCardWrapper(
                SCardScopes.System);

            RefreshReaderList();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshReaderList();
        }

        private void btnScriptBrowse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
