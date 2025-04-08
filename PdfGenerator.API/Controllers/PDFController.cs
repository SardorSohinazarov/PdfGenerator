using Microsoft.AspNetCore.Mvc;
using PdfGenerator.API.Services;
using SelectPdf;

namespace PdfGenerator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PDFController : ControllerBase
    {
        private readonly PuppeteerPdfGenerator _puppeteerPdfGenerator;

        public PDFController(
            PuppeteerPdfGenerator puppeteerPdfGenerator)
        {
            _puppeteerPdfGenerator = puppeteerPdfGenerator;
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePDF(string html)
        {
            HtmlToPdf htmlToPdf = new HtmlToPdf();
            PdfDocument doc = htmlToPdf.ConvertHtmlString(html);

            doc.Save("C:\\Users\\Sardor\\Desktop\\a.pdf");

            // PDF ni qaytarish
            return Ok("PDF muvaffaqiyatli yaratildi");
        }

        [HttpPost("puppeteer")]
        public async Task<IActionResult> GeneratePDFPuppeteer(string html)
        {
            (string result, string error) = await _puppeteerPdfGenerator.GeneratePdfBase64Async(html);
            if (string.IsNullOrEmpty(result)) return new ObjectResult(error);
            HttpContext.Response.Headers.Add("Content-Disposition", $"attachment; filename=Data.pdf; filename*=UTF-8''Data.pdf");
            return File(Convert.FromBase64String(result), "application/pdf");
        }
    }
}
