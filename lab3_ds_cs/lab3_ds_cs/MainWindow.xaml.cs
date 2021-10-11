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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.IO;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace lab3_ds_cs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonEncrypt_Click(object sender, RoutedEventArgs e)
        {
            var data = UTF8Encoding.UTF8.GetBytes(InputText.Text);
            string key = Key.Text;
            var RC5 = new RC5(8);
            var keyEncoded = UTF8Encoding.UTF8.GetBytes(key);
            var md5 = MD5.Create();
            var hashedKey  = md5.ComputeHash(keyEncoded);
            byte[] keyArr = new byte[8];
            Array.Copy(hashedKey, 0, keyArr, 0, 8);

            byte [] encryptedData = RC5.encryptECB(data, keyArr);
        }
        byte[] encrypt(string filepath)
        {
            string key = Key.Text;
            var RC5 = new RC5(8);
            var keyEncoded = UTF8Encoding.UTF8.GetBytes(key);
            var md5 = MD5.Create();
            var hashedKey = md5.ComputeHash(keyEncoded);
            byte[] keyArr = new byte[8];
            Array.Copy(hashedKey, 0, keyArr, 0, 8);
            byte[] data = File.ReadAllBytes(filepath);

            byte[] encryptedData = RC5.encryptCBCPAD(data, keyArr);

            //Output.Text = String.Join(" ", encryptedData);
            return encryptedData;
        }

        byte[] decrypt(string filepath)
        {
            string key = Key.Text;
            var RC5 = new RC5(8);
            var keyEncoded = UTF8Encoding.UTF8.GetBytes(key);
            var md5 = MD5.Create();
            var hashedKey = md5.ComputeHash(keyEncoded);
            byte[] keyArr = new byte[8];
            Array.Copy(hashedKey, 0, keyArr, 0, 8);
            byte[] data = File.ReadAllBytes(filepath);

            byte[] decryptedData = RC5.decryptCBCPAD(data, keyArr);

            //Output.Text = String.Join(" ", encryptedData);
            return decryptedData;
        }

        private void Decrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog filePicker = new OpenFileDialog();
                if (filePicker.ShowDialog() == true)
                {
                    string filename = filePicker.FileName;
                    var data = decrypt(filename);
                    
                    string dir = Path.GetDirectoryName(filename);
                    string ext = Path.GetExtension(filename);
                    string filename2 = Path.GetFileNameWithoutExtension(filename);
                    string path = Path.Combine(dir, filename2 + "-dec" + ext);
                    File.WriteAllBytes(path, data);
                    MessageBox.Show($"Decrypted successfully", "Decryption", MessageBoxButton.OK);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show($"Can`t decrypt: {error.Message}", "Decryption", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
                         
        }

        private void Encrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog filePicker = new OpenFileDialog();
                if(filePicker.ShowDialog() == true)
                {
                    string filename = filePicker.FileName;
                    byte[] data = encrypt(filename);

                    string dir = Path.GetDirectoryName(filename);
                    string ext = Path.GetExtension(filename);
                    string filename2 = Path.GetFileNameWithoutExtension(filename);
                    string path = Path.Combine(dir, filename2 + "-enc" + ext);
                    File.WriteAllBytes(path, data);
                    MessageBox.Show($"Encrypted successfully", "Encryption", MessageBoxButton.OK);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show($"Can`t encrypt: {error.Message}", "Encryption", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
        }
    }
}
