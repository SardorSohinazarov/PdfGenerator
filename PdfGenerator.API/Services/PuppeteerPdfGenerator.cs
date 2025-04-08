using PuppeteerSharp;
using PuppeteerSharp.Media;
using NavigationOptions = PuppeteerSharp.NavigationOptions;

namespace PdfGenerator.API.Services
{
    public class PuppeteerPdfGenerator : IDisposable
    {
        private readonly ILogger<PuppeteerPdfGenerator> _logger;
        private IBrowser? _browser;
        private readonly string _executablePath;

        public PuppeteerPdfGenerator(
            ILogger<PuppeteerPdfGenerator> logger
            )
        {
            _logger = logger;
            _executablePath = OperatingSystem.IsWindows()
                ? "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
                : "/usr/bin/chromium";
        }

        public void Dispose()
        {
            if (_browser != null)
            {
                _browser.Dispose();
            }
        }

        public async ValueTask<IBrowser> Init()
        {
            if (_browser == null)
            {
                _browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    Args = new[] { "--no-sandbox", "--disable-dev-shm-usage" },
                    ExecutablePath = _executablePath
                });
            }

            return _browser;
        }

        public async Task<Stream> GeneratePdfAsync(string html, bool landscape = false)
        {
            var pdfOptions = new PdfOptions
            {
                Format = PaperFormat.A4,
                MarginOptions = new MarginOptions()
                {
                    Bottom = "1cm",
                    Top = "1cm",
                    Left = "1cm",
                    Right = "1cm"
                },
                Landscape = landscape
            };

            Stream stream;

            var browser = await Init().ConfigureAwait(false);
            await using (var page = await browser.NewPageAsync())
            {
                await page.SetContentAsync(html, new NavigationOptions { Timeout = 60000 });
                stream = await page.PdfStreamAsync(pdfOptions);
            }

            _logger.LogInformation("Pdf generated successfully");

            return stream;
        }

        public async Task<(string, string)> GeneratePdfBase64Async(string html, bool landscape = false)
        {
            var pdfOptions = new PdfOptions
            {
                Format = PaperFormat.A4,
                MarginOptions = new MarginOptions()
                {
                    Bottom = "1cm",
                    Top = "1cm",
                    Left = "1cm",
                    Right = "1cm"
                },
                Landscape = landscape
            };

            // PDF ni Stream sifatida generatsiya qilamiz
            var browser = await Init().ConfigureAwait(false);
            await using (var page = await browser.NewPageAsync())
            {
                await page.SetContentAsync(html, new NavigationOptions { Timeout = 60000 });
                var pdfStream = await page.PdfStreamAsync(pdfOptions);

                // Streamni Base64 formatiga aylantiramiz
                using var memoryStream = new MemoryStream();
                await pdfStream.CopyToAsync(memoryStream);
                var pdfBase64 = Convert.ToBase64String(memoryStream.ToArray());

                _logger.LogInformation("Pdf generated successfully and converted to Base64");

                return (pdfBase64, null);
            }
        }
    }
}
