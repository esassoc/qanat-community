namespace Microsoft.Extensions.DependencyInjection
{
    public static class SitkaCaptureServiceCollectionExtensions
    {
        public static IServiceCollection AddSitkaCaptureService(this IServiceCollection services, string baseUri)
        {
            services.AddTransient(s => new SitkaCaptureService.SitkaCaptureService(baseUri));

            return services;
        }

    }
}
