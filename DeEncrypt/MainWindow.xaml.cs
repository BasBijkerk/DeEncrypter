using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace DeEncrypt
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
        public byte[] encrypted;
        public string decrypted;
        public byte[] Key;
        public byte[] IV;

        public static string IOString;
        //private bool Enc;
        //private bool Dec;
        public static byte[] EncryptString(string Input, byte[] Key, byte[] IV)
        {
            
            if (Input == null || Input.Length <= 0) throw new ArgumentNullException("");
            if (Key == null || Key.Length <= 0) throw new ArgumentNullException("");
            if (IV == null || IV.Length <= 0) throw new ArgumentNullException("");
            byte[] encrypted;

            
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream mstream = new MemoryStream())
                {
                    using (CryptoStream cstream = new CryptoStream(mstream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swriter = new StreamWriter(cstream))
                        {
                            swriter.Write(Input);
                            
                        }
                        
                        encrypted = mstream.ToArray();
                    }

                }
                
            }
            return encrypted;

        }
        public static string DecryptString(byte[] Input, byte[] Key, byte[] IV)
        {
            if (Input == null || Input.Length <= 0) throw new ArgumentNullException("");
            if (Key == null || Key.Length <= 0) throw new ArgumentNullException("");
            if (IV == null || IV.Length <= 0) throw new ArgumentNullException("");
            string decrypted = null;

            using(Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using(MemoryStream msstream = new MemoryStream(Input))
                {
                    using(CryptoStream cstream = new CryptoStream(msstream, decryptor, CryptoStreamMode.Read))
                    {
                        using(StreamReader sread = new StreamReader(cstream))
                        {
                            decrypted = sread.ReadToEnd();
                        }
                       
                    }
                }
            }
            
            return decrypted;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            
            using (MD5 md5 = MD5.Create())
            {
               
                IV = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(KeyBox.Text));
            }
            EnDeCrypt(true, false);
            textBox2.Text = Convert.ToBase64String(encrypted) + "*" + Convert.ToBase64String(Key);
            
        }

        private void button_Click2(object sender, RoutedEventArgs e)
        {
            //textBox.Text = "";
            using (MD5 md5 = MD5.Create())
            {
                IV = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(KeyBox.Text));
                //MD5Box.Text = Convert.ToBase64String(IV);
            }
            if(IV != null && textBox2.Text != "")
            {
                Key = Convert.FromBase64String(textBox2.Text.Split('*')[1].ToString());
                //IV = Convert.FromBase64String(textBox2.Text.Split('*')[2].ToString());
                EnDeCrypt(false, true);
                textBox.Text = decrypted;
            }
            
            
        }

        private void EnDeCrypt(bool Enc, bool Dec)
        {
            using (Aes aes = Aes.Create())
            {
                if (Enc)
                {
                    
                    Key = aes.Key;
                    //IV = aes.IV;
                    encrypted = EncryptString(textBox.Text, Key, IV);
                    Enc = !Enc;
                }
                
                if (Dec)
                {
                    var SplitTextBox = textBox2.Text.Split('*');
                    decrypted = DecryptString(Convert.FromBase64String(SplitTextBox[0].ToString()), Key, IV);
                    Dec = !Dec;
                }

                
            }
        }
       


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            if (File.Exists(FileBox.Text + ".txt"))
            {
                if(File.Exists(FileBox.Text + i + ".txt"))
                {
                    i++;
                }
                File.WriteAllText(FileBox.Text + i +".txt", textBox2.Text);
            }
            else
            {
                File.WriteAllText(FileBox.Text + ".txt", textBox2.Text);
            }
            
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            textBox2.Text = File.ReadAllText(FileBox.Text + ".txt");
        }
    }

    
}
