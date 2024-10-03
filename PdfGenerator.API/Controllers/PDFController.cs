using Microsoft.AspNetCore.Mvc;
using SelectPdf;

namespace PdfGenerator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PDFController : ControllerBase
    {
        private readonly IPDFService _pdfService;

        public PDFController(IPDFService pdfService)
        {
            _pdfService = pdfService;
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
    }

    public interface IPDFService
    {
        Task<byte[]> GeneratePDF(string html);
    }

    public class PDFService : IPDFService
    {
        public async Task<byte[]> GeneratePDF(string html)
        {
            return null;
        }
    }
}
