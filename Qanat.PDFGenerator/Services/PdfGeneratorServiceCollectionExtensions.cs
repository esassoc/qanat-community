using Microsoft.Extensions.DependencyInjection;
using Qanat.API.Services.PdfGenerator;
using Qanat.PDFGenerator.Pdfs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Qanat.PDFGenerator.Services
{

    public static class PdfGeneratorServiceCollectionExtensions
    {
        public static IServiceCollection AddPdfGeneratorService(this IServiceCollection services)
        {
            services.AddRazorPages().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            });
            services.AddTransient<IRazorPartialToStringRenderer, RazorPartialToStringRenderer>();
            services.AddTransient<UsageStatementTemplatePdf>();

            return services;
        }
    }
}