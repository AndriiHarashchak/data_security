using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;
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

namespace lab5_ds_cs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DSAProvider dsaProvider;
        string EDSValue { get; set; }
        string EDSFromFile { get; set; }
        string FileToGenerateEdsData { get; set; }
        bool IsFileSelected { get; set; }
        private DSAParameters? Parameters { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            dsaProvider = new DSAProvider(null);
            IsFileSelected = false;
        }

        private void EDSForText_Click(object sender, RoutedEventArgs e)
        {
            string text = dsa_text.Text;
            byte[] textAsBytes = Encoding.UTF8.GetBytes(text);
            byte[] dsa = dsaProvider.GenerateEDS(textAsBytes);
            DSA.Text = Convert.ToHexString(dsa);
            EDSValue = Convert.ToHexString(dsa);
        }

        private void EDSvalueExport_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "signature"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show save file dialog box
            bool? result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                File.WriteAllText(filename, EDSValue);
            }
        }

        private void GenerateEDSForFile_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog();
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string path = dlg.FileName;
                byte[] dsa = dsaProvider.GenerateEDSForFile(path);
                DSA.Text = Convert.ToHexString(dsa);
                EDSValue = Convert.ToHexString(dsa);
            }
        }

        private void EDSFile_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog();
            dlg.Filters.Add(new CommonFileDialogFilter("Text files", "*.txt"));
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string path = dlg.FileName;
                EDSFromFile = File.ReadAllText(path);
                EDSFromFileValue.Text = EDSFromFile;
            }
            else
            {
                EDSFromFile = "";
            }
        }

        private void FileToGenerateEDS_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog();
            dlg.Title = "Select file to compare EDS value";
            //dlg.Filters.Add(new CommonFileDialogFilter("Text files", "*.txt"));
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string path = dlg.FileName;
                FileToGenerateEdsData = File.ReadAllText(path);
                IsFileSelected = true;
                FilePath.Text = path;
            }
            else
            {
                FileToGenerateEdsData = "";
                IsFileSelected = false;
                MessageBox.Show("Can`t read file. Try again", "Params import", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            dlg.Title = "Select file with DSA params";
            dlg.Filters.Add(new CommonFileDialogFilter("Text files", "*.json"));
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string path = dlg.FileName;
                //var data = File.ReadAllText(path);
                DSAParameters? parameters;
                bool ok = DeserializeFromFile(path, out parameters);
                if (!ok)
                {
                    MessageBox.Show("Params not imported. Try again", "Params import", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                /*if(ok && parameters != null)
                {
                    dsaProvider.PublicDsaParameters = (DSAParameters)parameters;
                }*/

                Parameters = parameters;
            }
            else
            {
                MessageBox.Show("Params not imported. Try again", "Params import", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CompareEDS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EDSFromFile != "" && IsFileSelected)
                {
                    var bytes = Encoding.UTF8.GetBytes(FileToGenerateEdsData);
                    byte[] eds = Convert.FromHexString(EDSFromFile);
                    bool ok = dsaProvider.VerifyEDS(eds, bytes, (DSAParameters)Parameters);
                    if (ok)
                    {
                        MessageBox.Show("EDS verified successfully", "EDC verifying", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("EDS not verified. Not correct EDS or data", "EDC verifying", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Some important data is not provided", "EDC Check", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can`t check. Try again\n " + ex.Message, "Params import", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public bool SerializeToFile(DSAParameters parameters, string filename)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(parameters);
            File.WriteAllText(filename, json);
            return true;
        }

        public bool DeserializeFromFile(string filename, out DSAParameters? parameters)
        {
            //var json = JsonSerializer.Serialize(parameters);
            if (File.Exists(filename))
            {
                string text = File.ReadAllText(filename);
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<DSAParameters>(text);
                parameters = result;
                return true;
            }
            else
            {
                parameters = null;
                return false;
            }
        }

        private void DSAParamsImport_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog();
            dlg.Title = "Select file to compare EDS value";
            //dlg.Filters.Add(new CommonFileDialogFilter("Text files", "*.txt"));
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string path = dlg.FileName;
                DSAParameters? parameters;
                bool ok = DeserializeFromFile(path, out parameters);
                if (!ok)
                {
                    MessageBox.Show("Params imported", "Params import", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }

                ok = dsaProvider.ImportParameters((DSAParameters) parameters);
                if (!ok)
                {
                    MessageBox.Show("Params not imported. try again", "Params import", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void DSAParamsExport_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog();
            dlg.IsFolderPicker = true;
            dlg.Title = "Folder to store DSA params";
            //dlg.Filters.Add(new CommonFileDialogFilter("Text files", "*.txt"));
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string path = dlg.FileName;
                var filename = path + "/public_params.json";
                var filename2 = path + "/all_params.json";
                var publicParams = dsaProvider.GetParameters(false);
                var privateParams = dsaProvider.GetParameters(true);
                //var json = Newtonsoft.Json.JsonConvert.SerializeObject(publicParams);
                try
                {
                    SerializeToFile(publicParams, filename);
                    SerializeToFile(privateParams, filename2);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can`t export params: " + ex.Message, "Params export", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            else
            {
                FileToGenerateEdsData = "";
            }
        }
    }
}