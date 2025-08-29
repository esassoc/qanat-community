using Qanat.API.Services.PdfGenerator;
using Qanat.Models.DataTransferObjects;

namespace Qanat.PDFGenerator.Pdfs
{
    public class UsageStatementTemplatePdf : PdfGeneratorBase
    {
        public UsageStatementTemplatePdf(IRazorPartialToStringRenderer renderer, SitkaCaptureService.SitkaCaptureService sitkaCaptureService) : base(renderer, sitkaCaptureService)
        {
        }

        public async Task<byte[]> BuildPdf(UsageStatementWaterAccountDto usageStatementWaterAccountDto)
        {
            var htmlString = await _renderer.RenderPartialToStringAsync("UsageStatementTemplatePdf", usageStatementWaterAccountDto);
            var cssStrings = new List<string>()
            {
                GetCssStringAtProjectPath(@"Content/css/Print.css")
            };
            var pdf = await HtmlToPdf(htmlString, cssStrings);
            return pdf;
        }
    }
}