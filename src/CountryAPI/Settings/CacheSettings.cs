using System;
namespace CountryAPI
{
	public class CacheSettings
	{
		public string CacheKey { get; set; } = "Countries";
		public int CacheDurationInHours { get; set; }

    }
}

