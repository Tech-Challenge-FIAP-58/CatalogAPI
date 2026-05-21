using AutoMapper;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Application.Services;
using FCG.Catalog.Domain.Inputs;
using FCG.Catalog.Domain.Models.Catalog;
using FCG.Catalog.Domain.Web;
using FCG.Catalog.Infra.Mapping;
using FCG.Catalog.Infra.Mongo;
using Microsoft.Extensions.Configuration;

namespace FCG.Catalog.Infra.Search
{
    public class GameSearchService(
        ElasticsearchClient _client,
        IMapper _mapper,
        IConfiguration _configuration) : BaseService, IGameSearchService
    {
        private string Index => _configuration["Elasticsearch:IndexName"]!;

        public async Task<IApiResponse<IEnumerable<GameResponseDto>>> Search(
            string query, int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            var response = await _client.SearchAsync<GameDocument>(s => s
                .Index(Index)
                .From((page - 1) * pageSize)
                .Size(pageSize)
                .Query(q => q
                    .MultiMatch(mm => mm
                        .Query(query)
                        .Fields(new[] { "name^3", "publisherName^2", "platform", "description" })
                        .Fuzziness(new Fuzziness("AUTO"))
                        .Type(TextQueryType.BestFields)
                    )
                )
                .Sort(sort => sort.Score(sc => sc.Order(SortOrder.Desc)))
            , ct);

            if (!response.IsValidResponse)
                return BadRequest<IEnumerable<GameResponseDto>>(
                    $"Search failed: {response.DebugInformation}");

            var result = _mapper.Map<IEnumerable<GameResponseDto>>(response.Documents);

            return Ok(result);
        }

        public async Task IndexGameAsync(GameDocument doc, CancellationToken ct = default)
        {
            var response = await _client.IndexAsync(doc, i => i
                .Index(Index)
                .Id(doc.Id.ToString()), ct);

            if (!response.IsValidResponse)
                throw new InvalidOperationException(
                    $"Failed to index game {doc.Id}: {response.DebugInformation}");
        }

        public async Task RemoveGameAsync(Guid id, CancellationToken ct = default)
        {
            var response = await _client.DeleteAsync<GameDocument>(
                id.ToString(), d => d.Index(Index), ct);

            if (!response.IsValidResponse)
                throw new InvalidOperationException(
                    $"Failed to remove game {id} from index: {response.DebugInformation}");
        }
    }
}