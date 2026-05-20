using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace FCG.Catalog.Infra.Caching
{
	public class CachingService : ICachingService
	{
		private readonly IDistributedCache _cache;
		private readonly DistributedCacheEntryOptions _options;

		public CachingService(IDistributedCache cache)
		{
			_cache = cache;
			_options = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
				SlidingExpiration = TimeSpan.FromMinutes(2)
			};
		}

		public async Task<T?> GetAsync<T>(string key)
		{
			var value = await GetAsync(key);
			if (value == null)
				return default;
			return JsonConvert.DeserializeObject<T>(value);
		}

		public async Task SetAsync<T>(string key, T value)
		{
			var jsonValue = JsonConvert.SerializeObject(value);
			await SetAsync(key, jsonValue);
		}

		public async Task RemoveAsync(string key)
		{
			await _cache.RemoveAsync(key);
		}

		#region private ::

		private async Task<string?> GetAsync(string key)
		{
			return await _cache.GetStringAsync(key);
		}

		private async Task SetAsync(string key, string value)
		{
			await _cache.SetStringAsync(key, value, _options);
		}

		#endregion
	}
}
