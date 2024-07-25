class Program
{
    private static readonly HttpClient httpClient = new HttpClient();

    // Use your local url.
    // Calling the Azure URL overwhelms the system (and exceeds the Quota) since we are using the free tier
    private const string BASEURL = "__YOUR_LOCAL_URL__";

    private const string AllCountriesApiUrl = $"{BASEURL}/api/countries";
    private const string CountryByCodeApiUrl = $"{BASEURL}/api/countries/col";
    private const string AllRegionsApiUrl = $"{BASEURL}/api/regions";
    private const string AllLanguagesApiUrl = $"{BASEURL}/api/languages";
    private const int MaxRounds = 1000;
    private const int MaxCallsPerRound = 10;

    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting API simulation...");

        for (int round = 0; round < MaxRounds; round++)
        {
            List<Task> allCountriesTask = new List<Task>();
            List<Task> countryByCodeTask = new List<Task>();
            List<Task> allRegionsTask = new List<Task>();
            List<Task> allLanguagesTask = new List<Task>();

            for (int call = 0; call < MaxCallsPerRound; call++)
            {
                var number = (round * 100) + call;
                allCountriesTask.Add(CallApiAsync(number, AllCountriesApiUrl));
                countryByCodeTask.Add(CallApiAsync(number, CountryByCodeApiUrl));
                allRegionsTask.Add(CallApiAsync(number, AllRegionsApiUrl));
                allLanguagesTask.Add(CallApiAsync(number, AllLanguagesApiUrl));
            }
            await Task.WhenAll(allCountriesTask);
            await Task.WhenAll(countryByCodeTask);
            await Task.WhenAll(allRegionsTask);
            await Task.WhenAll(allLanguagesTask);
            Thread.Sleep(100);
        }

        Console.WriteLine("All API calls completed.");
        Console.ReadLine();
    }

    private static async Task CallApiAsync(int callNumber, string apiUrl)
    {
        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Call {callNumber}, Endpoint {apiUrl}: Success.");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Call {callNumber}, Endpoint {apiUrl}: Error - {e.Message} | [Inner Exception: {e.InnerException?.Message}]");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Call {callNumber}, Endpoint {apiUrl}: Unexpected error - {e.Message} | [Inner Exception: {e.InnerException?.Message}]");
        }
    }
}
