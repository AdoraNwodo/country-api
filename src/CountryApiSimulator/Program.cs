class Program
{
    private static readonly HttpClient httpClient = new HttpClient();
    private const string ApiUrl = "http://localhost:7028/api/countries"; // Replace with your API URL
    private const int MaxCalls = 10000;

    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting API simulation...");

        List<Task> tasks = new List<Task>();
        for (int i = 0; i < MaxCalls; i++)
        {
            tasks.Add(CallApiAsync(i));
        }

        await Task.WhenAll(tasks);

        Console.WriteLine("All API calls completed.");
        Console.ReadLine();
    }

    private static async Task CallApiAsync(int callNumber)
    {
        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(ApiUrl);
            response.EnsureSuccessStatusCode(); // Throws an exception if the response status code is not successful

            string content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Call {callNumber}: Success - Received {content.Length} characters.");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Call {callNumber}: Error - {e.Message} | [Inner Exception: {e.InnerException?.Message}]");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Call {callNumber}: Unexpected error - {e.Message} | [Inner Exception: {e.InnerException?.Message}]");
        }
    }
}
