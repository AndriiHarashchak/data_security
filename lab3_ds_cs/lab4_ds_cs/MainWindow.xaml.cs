using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace lab4_ds_cs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string? Filename { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Filename = null;
        }
        private void FilePick_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog filePicker = new OpenFileDialog();
            if (filePicker.ShowDialog() == true)
            {
                Filename = filePicker.FileName;
                LabelFilename.Content = Filename;
            }
            else
            {
                Filename = null;
            }
        }
        
        private void EncryptRSA_Click(object sender, RoutedEventArgs e)
        {

            if(Filename != null)
            {
                Encryptor encryptor= new Encryptor();

                MessageBoxResult dialogResult = MessageBox.Show("Do you want to import rsa params?", "Params import", MessageBoxButton.YesNo);
                string? RsaParamsPath = null;
                string? publicKeyPath = null;
                string? fullKeyPath = null;
                if (dialogResult == MessageBoxResult.Yes)
                {
                    CommonOpenFileDialog dialog1 = new CommonOpenFileDialog();
                    //dialog1.InitialDirectory = "./";
                    dialog1.Title = "Select file with all rsa params";
                    if (dialog1.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        RsaParamsPath = dialog1.FileName;
                    }
                }
                else if (dialogResult == MessageBoxResult.No)
                {
                    CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.InitialDirectory = "./";
                dialog.IsFolderPicker = true;
                
                dialog.Title = "Select public rsa params export folder";
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                  publicKeyPath = dialog.FileName;
                }
                dialog.Title = "Select full rsa params export folder";
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    fullKeyPath = dialog.FileName;
                }
                }
                
                try {
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    encryptor.EncryptFileRSA(Filename, RsaParamsPath, publicKeyPath, fullKeyPath);
                    stopWatch.Stop();
                    
                    // Get the elapsed time as a TimeSpan value.
                    TimeSpan ts = stopWatch.Elapsed;
                    // Format and display the TimeSpan value.
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);
                    Output.Text += " RSA: encrypted succesfully \n";
                    Output.Text += $"RSA: encryption time: {elapsedTime} \n";
                }
                catch (Exception ex){
                    //string text = Output.Text;
                    Output.Text += "Error: "+ ex.Message + "\n";
                }
            }
        }

        private void DencryptRSA_Click(object sender, RoutedEventArgs e)
        {
            if (Filename != null)
            {
                Encryptor encryptor = new Encryptor();

                string key = Key.Text;
                byte[] keyBytes = UTF8Encoding.UTF8.GetBytes(key);
                try
                {
                    CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                    //dialog.IsFolderPicker = false;
                    dialog.Title = "Select private rsa params file";
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        Stopwatch stopWatch = new Stopwatch();
                        stopWatch.Start();

                        encryptor.DecryptFileRSA(Filename, dialog.FileName);

                        stopWatch.Stop();
                        // Get the elapsed time as a TimeSpan value.
                        TimeSpan ts = stopWatch.Elapsed;
                        // Format and display the TimeSpan value.
                        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                        Output.Text += "RSA: decrypted successfully \n ";
                        Output.Text += $"RSA: decryption time: {elapsedTime} \n";
                    }
                    else
                    {
                        Output.Text += "No params provided, can`t decrypt \n ";
                    }
                    
                }
                catch (Exception ex)
                {
                    Output.Text += "Error: " + ex.Message+ "\n";
                }
            }
        }

        private void EncryptRC5_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Filename))
            try
            {
                string key = Key.Text;

                RC5_l4 rc5 = new RC5_l4();
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                byte[] encryptedData = rc5.encryptFile(Filename, key);
                writeToFIle(encryptedData, "_enc_rc5");

                stopWatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
                Output.Text += "RC5: Encrypted succesfully\n";
                Output.Text += $"RC5: Encryption time: {elapsedTime} \n";
            }
            catch (Exception ex)
            {
                Output.Text += "RC5 error: " + ex.Message + "\n";
                //throw;
            }
            else
            {
                Output.Text += "RC5 error: File not selected or file not exist anymore \n";
            }
        }

        private void DencryptRC5_Click(object sender, RoutedEventArgs e)
        {
            if(File.Exists(Filename))
            try
            {
                string key = Key.Text;

                RC5_l4 rc5 = new RC5_l4();
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                byte[] encryptedData = rc5.decryptFile(Filename, key);
                writeToFIle(encryptedData, "_dec");

                stopWatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

                    Output.Text += "RC5: Decrypted succesfully\n";
                    Output.Text += $"RC5: Decryption time: {elapsedTime} \n";
                }
            catch (Exception ex)
            {
                Output.Text += "RC5 error: " + ex.Message + "\n";
                //throw;
            }
            else
            {
                Output.Text += "RC5 error: File not selected or file not exist anymore \n";
            }
        }
        bool writeToFIle(byte[] data, string file_exp)
        {
            string dir = System.IO.Path.GetDirectoryName(Filename);
            string ext = System.IO.Path.GetExtension(Filename);
            string filename2 = System.IO.Path.GetFileNameWithoutExtension(Filename);
            string path = System.IO.Path.Combine(dir??"./", filename2 + file_exp + ext);//"-enc"
            File.WriteAllBytes(path, data);
            return true;
        }
    }
}
