using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace SimpleApduSender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker worker;
        private SCardWrapper scWrapper;

        public MainWindow()
        {
            InitializeComponent();

            this.Title = "Simple APDU Sender v"
                + typeof(MainWindow).Assembly.GetName().Version;
        }

        private void DisplayErrorMessage(
            string message)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                MessageBox.Show(
                message,
                this.Title,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            }));
        }

        private void DisplayLastErrorMessage()
        {
            DisplayErrorMessage(
                scWrapper.LastErrorString);
        }

        private bool DisplayErrorMessageYesNo(
            string message)
        {
            MessageBoxResult msgResult = MessageBoxResult.No;

            Dispatcher.Invoke(new Action(() =>
            {
                msgResult = MessageBox.Show(
                    message,
                    this.Title,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Error);
            }));

            return msgResult == MessageBoxResult.Yes;
        }

        private void RefreshReaderList()
        {
            try
            {
                List<string> readers = scWrapper.GetReaderList();

                cboReader.Items.Clear();

                if (readers == null)
                    DisplayLastErrorMessage();
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

        private List<ApduScript> ReadScript()
        {
            List<ApduScript> retval = new List<ApduScript>();

            string[] lines = null;

            Dispatcher.Invoke(new Action(() =>
            {
                lines = txtScript.Text.Split(
                    new string[] { Environment.NewLine },
                    StringSplitOptions.None);
            }));

            string input = String.Empty;
            string output = String.Empty;

            // Last Phrase:
            // 0x0 (0000) -> Unknown
            // 0x1 (0001) -> "RST()"
            // 0x2 (0010) -> "I:"
            // 0x4 (0100) -> "O:"
            // 0x8 (1000) -> Comment
            byte lastPhrase = 0;

            foreach (string s in lines)
            {
                string line = s.Trim();

                // Ignore empty line
                if (line == String.Empty)
                {
                    // If user create apdu command without expected result
                    if (lastPhrase == 2)
                    {
                        retval.Add(
                        new ApduScript(
                            input,
                            null));
                    }

                    lastPhrase = 0;
                    continue;
                }
                // Ignore comment line
                else if (line.Substring(0,1) == "'")
                {
                    // If user create apdu command without expected result
                    if (lastPhrase == 2)
                    {
                        retval.Add(
                        new ApduScript(
                            input,
                            null));
                    }

                    lastPhrase = 8;
                    continue;
                }
                else if (line.Substring(0, 5).ToUpper() == "RST()")
                {
                    // If user create apdu command without expected result
                    if (lastPhrase == 2)
                    {
                        retval.Add(
                        new ApduScript(
                            input,
                            null));
                    }

                    retval.Add(
                        new ApduScript(
                            true));

                    lastPhrase = 1;
                }
                else if (line.Substring(0, 2).ToUpper() == "I:")
                {
                    // If user create apdu command without expected result
                    if (lastPhrase == 2)
                    {
                        retval.Add(
                        new ApduScript(
                            input,
                            null));
                    }

                    // Record new apdu command
                    input = line.Substring(2).Trim();

                    lastPhrase = 2;
                }
                else if (line.Substring(0, 2).ToUpper() == "O:")
                {
                    if (lastPhrase == 2)
                    {
                        // Record new expected result
                        output = line.Substring(2).Trim();

                        retval.Add(
                            new ApduScript(
                                input,
                                output)); 
                    }

                    lastPhrase = 4;
                }
            }

            return retval;
        }

        private void ExecuteScript(object sender)
        {
            // Read script in the rich text box
            List<ApduScript> scriptList = ReadScript();

            int counter = 0;
            int scriptLength = scriptList.Count;

            Dispatcher.Invoke(new Action(() =>
            {
                pbExecute.Minimum = 0;
                pbExecute.Maximum = scriptLength;

                rtbOutput.Document.Blocks.Clear();
                rtbOutput.AppendText("\n");
                rtbOutput.AppendText("\n");
            }));

            // Flag for completed execution
            bool isContinue = true;

            // Start counting the time
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            foreach (ApduScript script in scriptList)
            {

                if (script.IsReset)
                {
                    System.Threading.Thread.Sleep(1000);

                    string atr = "";
                    bool success = false;

                    Dispatcher.Invoke(new Action(() =>
                    {
                        success = scWrapper.ResetReader(
                        (string)cboReader.SelectedItem,
                        out atr);

                        rtbOutput.AppendText(
                            String.Format(
                                ">> RST(){0}",
                                Environment.NewLine));
                    }));

                    if (!success)
                    {
                        isContinue = DisplayErrorMessageYesNo(
                            "Unable to reset the reader. Do you wish to continue?");

                        if (!isContinue)
                            break;
                    }

                    Dispatcher.Invoke(new Action(() =>
                    {
                        rtbOutput.AppendText(
                        String.Format(
                            "<< {0}{1}",
                            atr,
                            Environment.NewLine));

                        rtbOutput.ScrollToEnd();
                        rtbOutput.Refresh();
                    }));
                }
                else
                {
                    string expOut = scWrapper.SendAPDU(script.Input);

                    Dispatcher.Invoke(new Action(() =>
                    {
                        rtbOutput.AppendText(
                        String.Format(
                            ">> {0}{1}",
                            script.Input,
                            Environment.NewLine));

                        rtbOutput.AppendText(
                            String.Format(
                                "<< {0}{1}",
                                expOut,
                                Environment.NewLine));

                        rtbOutput.ScrollToEnd();
                        rtbOutput.Refresh();
                    }));

                    if (expOut == String.Empty)
                    {
                        isContinue = DisplayErrorMessageYesNo(
                            String.Format(
                                "{0}{1}. Do you wish to continue?",
                                scWrapper.LastErrorString,
                                Environment.NewLine));

                        if (!isContinue)
                            break;
                    }
                    else
                    {
                        if (script.ExpectedOutput != null &&
                            expOut != script.ExpectedOutput)
                        {
                            isContinue = DisplayErrorMessageYesNo(
                                "Unexpected result. Do you wish to continue?");

                            if (!isContinue)
                                break; 
                        }
                    } 
                }

                (sender as BackgroundWorker).ReportProgress(++counter);
            }

            stopWatch.Stop();

            // If isContinue is not touched (which means keep valued TRUE),
            // the script is run completely and we need to know the elapsed time
            if (isContinue)
            {
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;

                Dispatcher.Invoke(new Action(() =>
                {
                    rtbOutput.AppendText(
                        String.Format(
                            "{0}Elapsed time: {1:00}:{2:00}:{3:00}.{4:00}",
                            Environment.NewLine,
                            ts.Hours, 
                            ts.Minutes, 
                            ts.Seconds,
                            ts.Milliseconds / 10));

                    rtbOutput.ScrollToEnd();
                    rtbOutput.Refresh();
                }));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;

            scWrapper = new SCardWrapper(
                SCardScopes.System,
                SCardShareModes.Shared,
                SCardProtocol.Any);

            RefreshReaderList();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            RunScript(sender);
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbExecute.Value = e.ProgressPercentage;
        }

        private void RunScript(object sender)
        {
            try
            {
                // Establish Context
                if (!scWrapper.EstablishContext())
                {
                    DisplayLastErrorMessage();
                    return;
                }

                // Connect to selected reader
                Dispatcher.Invoke(new Action(() =>
                {

                    if (!scWrapper.ConnectReader((string)cboReader.SelectedItem))
                    {
                        DisplayLastErrorMessage();
                        return;
                    }
                }));

                // Execute script
                ExecuteScript(sender);

                // Disconnect to selected reader
                if (!scWrapper.DisconnectReader())
                {
                    DisplayLastErrorMessage();
                    return;
                }

                // Release Context
                if (!scWrapper.ReleaseContext())
                {
                    DisplayLastErrorMessage();
                    return;
                }
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(
                    ex.Message);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshReaderList();
        }

        private void btnScriptBrowse_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.FileName = "Document"; // Default file name
            //dlg.DefaultExt = ".txt"; // Default file extension
            //dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;

                txtScriptPath.Text = filename;
                
                // Load faster than using RichTextBox
                txtScript.Text = File.ReadAllText(filename);
            }
        }

        private void btnRunScript_Click(object sender, RoutedEventArgs e)
        {
            worker.RunWorkerAsync();
        }
    }

    public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate () { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
        }
    }
}
