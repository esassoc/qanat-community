using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Qanat.API.Services.PdfGenerator;

namespace Qanat.PDFGenerator.Pdfs
{
    public class PdfGeneratorBase
    {
        protected readonly IRazorPartialToStringRenderer _renderer;
        protected readonly SitkaCaptureService.SitkaCaptureService _sitkaCaptureService;
        protected static readonly string _projectPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);



        protected PdfGeneratorBase(IRazorPartialToStringRenderer renderer, SitkaCaptureService.SitkaCaptureService sitkaCaptureService)
        {
            _renderer = renderer;
            _sitkaCaptureService = sitkaCaptureService;
        }

        /// <summary>
        /// The css file you are trying to read must be set to "Copy always" so it ends up
        /// being build into the project directory.
        /// </summary>
        /// <param name="path">Typically something like @"Content/css/Print.css"</param>
        /// <returns></returns>
        protected string GetCssStringAtProjectPath(string path)
        {
            var combinedPath = Path.Combine(_projectPath, path);
            return File.ReadAllText(combinedPath);
        }

        protected Task<byte[]> HtmlToPdf(string htmlString, List<string> cssStrings)
        {
            var capturePostData = new CapturePostData()
            {
                html = htmlString,
                cssStrings = cssStrings
            };
            var pdfBytes = _sitkaCaptureService.PrintPDF(capturePostData);
            return pdfBytes;
        }

        public static string ImageAtPathToByteSrcString(string relativePath)
        {
            var bytes = File.ReadAllBytes(Path.Combine(_projectPath, relativePath));
            
            var byteString = Convert.ToBase64String(bytes);
            return $"data:image/png;base64,{byteString}";
        }
    }
}