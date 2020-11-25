using System;
using System.Net.Http;
using System.Threading.Tasks;

await GetUrlContentLengthAsync();
Console.WriteLine("end");

async Task<int> GetUrlContentLengthAsync()
{
    var client = new HttpClient();

    Task<string> getStringTask =
        client.GetStringAsync("https://docs.microsoft.com/dotnet");

    Console.WriteLine("Working...");

    string contents = await getStringTask;
    Console.WriteLine(contents);

    Console.WriteLine("hello");

    return 2;
}

