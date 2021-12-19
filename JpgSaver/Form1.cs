using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace JpgSaver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            MaximizeBox = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text)
                || string.IsNullOrEmpty(textBox2.Text)
                || string.IsNullOrEmpty(textBox3.Text)
                || string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show(@"Fill all fields", @"Invalid data", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    DownloadImages();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($@"Something went wrong. Data might be invalid. {ex.Message}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Count- of images (indexes at the end of link). Ex 100. Cookie- main cookie to connect to website if needed. Ex example. Link- without image number. Ex https://images.com/image=. Path- directory where to download. Ex C:/users/public/desktop/New Folder/. Address- PHPSESSID cookie url. Ex www.example.com", @"Image DownLoader", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DownloadImages()
        {
            var count = int.Parse(textBox1.Text);
            var cookieString = textBox2.Text;
            var link = textBox3.Text;
            var path = textBox4.Text;
            var address = textBox5.Text;
            var counter = 0;
            for (var index = 1; index < count + 1; index++)
            {
                var cookie = new Cookie("PHPSESSID", cookieString, "/", address);
                var webRequest = PrepareHttpWebRequest(link, index, cookie);

                // Grab the response from the server for the current WebRequest
                var webResponse = webRequest.GetResponse();

                var stream = webResponse.GetResponseStream();
                if (stream == null)
                {
                    continue;
                }

                var image = Image.FromStream(stream);

                webResponse.Close();
                // Wrap it in a StreamReader so you don't have to go byte by byte
                //TextReader tr = new StreamReader(webResponse.GetResponseStream());
                image.Save(path + index + ".jpg");
                counter++;
            }

            MessageBox.Show($@"Images have been downloaded {counter}/{count}", @"Success", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        // Create a WebRequest object and assign it a cookie container and make them think your Mozilla
        private HttpWebRequest PrepareHttpWebRequest(string link, int index, Cookie cookie)
        {
            var cookies = new CookieContainer();
            var webRequest = (HttpWebRequest)WebRequest.Create(link + index);
            webRequest.Method = "GET";
            webRequest.Accept = "*/*";
            webRequest.AllowAutoRedirect = false;
            webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0; .NET CLR 1.0.3705)";
            webRequest.CookieContainer = new CookieContainer();
            webRequest.ContentType = "text/xml";
            webRequest.Credentials = null;
            webRequest.CookieContainer.Add(cookies.GetCookies(new Uri(link + index)));
            webRequest.CookieContainer.Add(cookie);
            return webRequest;
        }
    }
}
