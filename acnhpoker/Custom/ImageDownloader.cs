using Ionic.Zip;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace ACNHPoker
{
    public partial class ImageDownloader : Form
    {
        public ImageDownloader()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            config.AppSettings.Settings["RestartRequired"].Value = "false";
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            yesBtn.Enabled = false;
            noBtn.Enabled = false;


            string path = Path.Combine(Application.StartupPath, "img.zip");

            if (File.Exists(@"img.zip"))
            {
                File.Delete(@"img.zip");
            }

            WebClient webClient = new WebClient();
            webClient.Proxy = null;

            webClient.DownloadProgressChanged += (s, ez) =>
            {
                progressBar.Value = ez.ProgressPercentage;
            };

            webClient.DownloadFileCompleted += (s, ez) =>
            {
                waitmsg.Visible = true;
                progressBar.Visible = false;

                Thread unzipThread = new Thread(delegate () { extractHere(); });
                unzipThread.Start();
            };

            webClient.DownloadFileAsync(new Uri("https://github.com/MyShiLingStar/ACNHPoker/releases/download/ImgPack8/img.zip"), "img.zip");

        }

        private void extractHere()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            config.AppSettings.Settings["RestartRequired"].Value = "true";

            try
            {
                using (ZipFile archive = new ZipFile(@"" + System.Environment.CurrentDirectory + "\\img.zip"))
                {
                    archive.ExtractAll(@"" + System.Environment.CurrentDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }
            catch
            {

            }

            this.Invoke((MethodInvoker)delegate
            {
                this.Close();
            });
        }

    }
}
