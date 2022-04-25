using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace PdfDownloader
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Console.WriteLine(dir);

            var pdfUrl = "https://web.archive.org/web/20151010204956if_/http://www.link.cs.cmu.edu:80/15859-f07/papers/point-location.pdf";
            var pdfDir = dir + "\\PdfFiles";
            await downloadPDFAsync(pdfUrl, pdfDir);
        }

        static async Task downloadPDFAsync(string url, string pdfDir)
        {
            try
            {
                if (!Directory.Exists(pdfDir))
                {
                    Directory.CreateDirectory(pdfDir);
                }

                DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
                var timestamp = now.ToString("yyyyMMddHHmmssfff");

                var pdfFilePath = pdfDir + "\\" + timestamp + ".pdf";
                await downloadFile(url, pdfFilePath);

            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
        }

        static async Task<bool> downloadFile(string url, string pdfPath)
        {
            var curDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            var chromeSessionDir = curDir + "\\ChromeSession";

            var tmpDownloadDir = curDir + "\\Tmp";
            if (Directory.Exists(tmpDownloadDir))
            {
                Directory.Delete(tmpDownloadDir, true);
            }
            Directory.CreateDirectory(tmpDownloadDir);

            ChromeOptions option = new ChromeOptions();
            option.AddUserProfilePreference("download.default_directory", tmpDownloadDir);
            option.AddArgument("--user-data-dir=" + chromeSessionDir);

            ChromeDriver driver = new ChromeDriver(curDir, option);
            driver.Navigate().GoToUrl(url);

            //await Task.Delay(3000);

            string downloadedFile;
            do
            {
                string[] downloadedFiles = Directory.GetFiles(tmpDownloadDir);
                if (downloadedFiles.Length > 0 && downloadedFiles[0].ToLower().EndsWith(".pdf"))
                {
                    downloadedFile = downloadedFiles[0];
                    break;
                }
                await Task.Delay(500);
            } while (true);

            // copy file
            try
            {
                File.Copy(downloadedFile, pdfPath, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            driver.Close();
            return true;
        }
    }
}
