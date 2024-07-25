using System;
namespace CountryAPI
{
	public class CacheSettings
	{
		public CacheSettings()
		{
		}

		public string CacheKey { get; set; }
		public int CacheDurationInHours { get; set; }

    }
}

