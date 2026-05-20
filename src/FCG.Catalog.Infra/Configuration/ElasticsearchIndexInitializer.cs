using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using FCG.Catalog.Infra.Mapping;
using FCG.Catalog.Infra.Mongo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FCG.Catalog.Infra.Configuration
{
    public class ElasticsearchIndexInitializer(
        ElasticsearchClient _client,
        IConfiguration _configuration,
        ILogger<ElasticsearchIndexInitializer> _logger) : IHostedService
    {
        private string Index => _configuration["Elasticsearch:IndexName"]!;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var exists = await _client.Indices.ExistsAsync(Index, cancellationToken);

                if (exists.Exists)
                {
                    _logger.LogInformation("Elasticsearch index '{Index}' already exists. Skipping creation.", Index);
                    return;
                }

                var response = await _client.Indices.CreateAsync(Index, c => c
                    .Mappings(m => m
                        .Properties<GameDocument>(p => p
                            .Text(t => t.Name, cfg => cfg
                                .Fields(f => f
                                    .Keyword(k => k.Name)))
                            .Text(t => t.PublisherName, cfg => cfg
                                .Fields(f => f
                                    .Keyword(k => k.PublisherName)))
                            .Keyword(t => t.Platform)
                            .Text(t => t.Description)
                            .DoubleNumber(t => t.Price)
                            .Boolean(t => t.IsAvailable)
                        )
                    ), cancellationToken
                );

                if (response.IsValidResponse)
                    _logger.LogInformation("Elasticsearch index '{Index}' created successfully.", Index);
                else
                    _logger.LogError("Failed to create Elasticsearch index '{Index}': {Debug}",
                        Index, response.DebugInformation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Elasticsearch index initialization.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}