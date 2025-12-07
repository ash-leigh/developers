using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace ExchangeRateProvider.Api.Configuration
{
    public class OpenApiDocumentTransformer : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            document.Info = new OpenApiInfo
            {
                Title = "Exchange Rate Provider API",
                Version = "v1",
                Description = "API for retrieving exchange rates for the given currency." +
                              "Provides current exchange rates and supports filtering by quote currencies.",
                Contact = new OpenApiContact
                {
                    Name = "API Support",
                    Email = "ashleighadams.contact@gmail.com"
                }
            };

            return Task.CompletedTask;
        }
    }
}
