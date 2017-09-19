using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

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

            this.Title = "Simple APDU Sender v"
                + typeof(MainWindow).Assembly.GetName().Version;
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

        private void DisplayLastErrorMessage()
        {
            DisplayErrorMessage(
                scWrapper.LastErrorString);
        }

        private bool DisplayErrorMessageYesNo(
            string message)
        {
            return (MessageBox.Show(
                message,
                this.Title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Error) == MessageBoxResult.Yes);
        }

        private string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(
              // TextPointer to the start of content in the RichTextBox.
              rtb.Document.ContentStart,
              // TextPointer to the end of content in the RichTextBox.
              rtb.Document.ContentEnd
            );

            // The Text property on a TextRange object returns a string 
            // representing the plain text content of the TextRange. 
            return textRange.Text;
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

            string[] lines =
                StringFromRichTextBox(rtbScript)
                    .Split(
                        new[] { Environment.NewLine },
                        StringSplitOptions.RemoveEmptyEntries);

            string input = String.Empty;
            string output = String.Empty;

            foreach (string s in lines)
            {
                string line = s.Trim();

                if (line == String.Empty)
                {
                    continue;
                }
                else if (line.Take(1).ToString() == "'")
                {
                    continue;
                }
                else if (line.Substring(0, 5) == "RST()")
                {
                    retval.Add(
                        new ApduScript(
                            true));
                }
                else if (line.Substring(0, 2) == "I:")
                {
                    input = line.Substring(2).Trim();
                }
                else if (line.Substring(0, 2) == "O:")
                {
                    output = line.Substring(2).Trim();

                    retval.Add(
                        new ApduScript(
                            input,
                            output));
                }
            }

            return retval;
        }

        private void ExecuteScript()
        {
            // Read script in the rich text box
            List<ApduScript> scriptList = ReadScript();

            rtbOutput.Document.Blocks.Clear();
            rtbOutput.AppendText("\n");
            rtbOutput.AppendText("\n");

            foreach (ApduScript script in scriptList)
            {
                bool isContinue = true;

                if (script.IsReset)
                {
                    string atr;
                    bool success = scWrapper.ResetReader(
                        (string)cboReader.SelectedItem,
                        out atr);

                    rtbOutput.AppendText(
                        String.Format(
                            ">> RST(){0}",
                            Environment.NewLine));

                    if (!success)
                    {
                        isContinue = DisplayErrorMessageYesNo(
                            "Unable to reset the reader. Do you wish to continue?");

                        if (!isContinue)
                            break;
                    }

                    rtbOutput.AppendText(
                        String.Format(
                            "<< {0}{1}",
                            atr,
                            Environment.NewLine));

                    rtbOutput.ScrollToEnd();
                    rtbOutput.Refresh();
                }
                else
                {
                    string expOut = scWrapper.SendAPDU(script.Input);

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
                    else if (expOut != script.ExpectedOutput)
                    {
                        isContinue = DisplayErrorMessageYesNo(
                            "Unexpected result. Do you wish to continue?");

                        if (!isContinue)
                            break;
                    } 
                }
            }
        }

        private void LoadFileToRichTextBox(
            string FilePath,
            RichTextBox rtb)
        {
            TextRange range;
            FileStream fStream;

            if (File.Exists(FilePath))
            {
                range = new TextRange(
                    rtb.Document.ContentStart, 
                    rtb.Document.ContentEnd);

                fStream = new FileStream(
                    FilePath, 
                    FileMode.OpenOrCreate);

                range.Load(fStream, DataFormats.Text);

                fStream.Close();
            }
            else
            {
                DisplayErrorMessage("Unable to find " + FilePath);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            scWrapper = new SCardWrapper(
                SCardScopes.System,
                SCardShareModes.Shared,
                SCardProtocol.Any);

            RefreshReaderList();
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

                LoadFileToRichTextBox(
                    filename,
                    rtbScript);
            }
        }

        private void btnRunScript_Click(object sender, RoutedEventArgs e)
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
                if (!scWrapper.ConnectReader((string)cboReader.SelectedItem))
                {
                    DisplayLastErrorMessage();
                    return;
                }

                // Execute script
                ExecuteScript();

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
