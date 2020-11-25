using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

//運行需求 .net5  使用最上層陳述式 toplevel 

Console.WriteLine("Start...");
//範例一 : 了解非同步運作方式

/*
 * caller 也要加上 await 才會等待 call method 的 await
 * 不然會在 getStringTask 時 Yield caller 然後就結束了
 */
//await GetUrlContentLengthAsync();
async Task<int> GetUrlContentLengthAsync()
{
    var client = new HttpClient();

    //按照慣例，非同步方法的名稱是以 "Async" 後置字元為結尾。 但別複寫原有內建事件名稱
    Task<string> getStringTask =
        client.GetStringAsync("https://docs.microsoft.com/dotnet");

    Console.WriteLine("Working...");

    //為了避免封鎖資源，GetStringAsync 會將控制權遞交 (Yield) 給它的呼叫端 GetUrlContentLengthAsync。
    string contents = await getStringTask;
    Console.WriteLine(contents);

    Console.WriteLine("hello");

    //如果方法沒有 return 陳述式或是 return 陳述式沒有運算元，則為 Task。
    return 2;
}


//範例二  有傳回的Task 跟沒有傳回的Task

async Task<int> GetTaskOfTResultAsync()
{
    int hours = 0;
    await Task.Delay(0);
    Console.WriteLine("Result Here");
    return hours;
}

await GetTaskOfTResultAsync();

async Task GetTaskAsync()
{
    await Task.Delay(0);
    Console.WriteLine("No Result Here");
    // No return statement needed
}

await GetTaskAsync();

//範例三  有傳回值的骰子

Random s_rnd = new Random();

Console.WriteLine($"You rolled {await GetDiceRollAsync()}");

async ValueTask<int> GetDiceRollAsync()
{
    Console.WriteLine("Shaking dice...");

    int roll1 = await RollAsync();
    int roll2 = await RollAsync();

    return roll1 + roll2;
}

async ValueTask<int> RollAsync()
{
    await Task.Delay(500);

    int diceRoll = s_rnd.Next(1, 7);
    return diceRoll;
}

//範例四 Cancel a list of tasks


// 用來記錄 Enter 事件的token
CancellationTokenSource s_cts = new CancellationTokenSource();

HttpClient s_client = new HttpClient
{
    MaxResponseContentBufferSize = 1_000_000
};

IEnumerable<string> s_urlList = new string[]
{
            "https://docs.microsoft.com",
            "https://docs.microsoft.com/aspnet/core",
            "https://docs.microsoft.com/azure",
            "https://docs.microsoft.com/azure/devops",
            "https://docs.microsoft.com/dotnet",
            "https://docs.microsoft.com/dynamics365",
            "https://docs.microsoft.com/education",
            "https://docs.microsoft.com/enterprise-mobility-security",
            "https://docs.microsoft.com/gaming",
            "https://docs.microsoft.com/graph",
            "https://docs.microsoft.com/microsoft-365",
            "https://docs.microsoft.com/office",
            "https://docs.microsoft.com/powershell",
            "https://docs.microsoft.com/sql",
            "https://docs.microsoft.com/surface",
            "https://docs.microsoft.com/system-center",
            "https://docs.microsoft.com/visualstudio",
            "https://docs.microsoft.com/windows",
            "https://docs.microsoft.com/xamarin"
};


Console.WriteLine("Application started.");
Console.WriteLine("Press the ENTER key to cancel...\n");

Task cancelTask = Task.Run(() =>
{
    while (Console.ReadKey().Key != ConsoleKey.Enter)
    {
        Console.WriteLine("Press the ENTER key to cancel...");
    }

    Console.WriteLine("\nENTER key pressed: cancelling downloads.\n");
    s_cts.Cancel();
});

//範例4.5 Cancel async tasks after a period of time
//s_cts.CancelAfter(3500);

//Task sumPageSizesTask = SumPageSizesAsync();

//await Task.WhenAny(new[] { cancelTask, sumPageSizesTask });

//Console.WriteLine("Application ending.");


//async Task SumPageSizesAsync()
//{
//    var stopwatch = Stopwatch.StartNew();

//    int total = 0;
//    foreach (string url in s_urlList)
//    {
//        // 當Enter被按下  Token 換變成 True 
//        int contentLength = await ProcessUrlAsync(url, s_client, s_cts.Token);
//        total += contentLength;
//    }

//    stopwatch.Stop();

//    Console.WriteLine($"\nTotal bytes returned:  {total:#,#}");
//    Console.WriteLine($"Elapsed time:          {stopwatch.Elapsed}\n");
//}

//static async Task<int> ProcessUrlAsync(string url, HttpClient client, CancellationToken token)
//{
//    HttpResponseMessage response = await client.GetAsync(url, token);
//    byte[] content = await response.Content.ReadAsByteArrayAsync();
//    Console.WriteLine($"{url,-60} {content.Length,10:#,#}");

//    return content.Length;
//}

// 範例5 完成處理
 HttpClient s_client2 = new HttpClient
{
    MaxResponseContentBufferSize = 1_000_000
};

 IEnumerable<string> s_urlList2 = new string[]
{
            "https://docs.microsoft.com",
            "https://docs.microsoft.com/aspnet/core",
            "https://docs.microsoft.com/azure",
            "https://docs.microsoft.com/azure/devops",
            "https://docs.microsoft.com/dotnet",
            "https://docs.microsoft.com/dynamics365",
            "https://docs.microsoft.com/education",
            "https://docs.microsoft.com/enterprise-mobility-security",
            "https://docs.microsoft.com/gaming",
            "https://docs.microsoft.com/graph",
            "https://docs.microsoft.com/microsoft-365",
            "https://docs.microsoft.com/office",
            "https://docs.microsoft.com/powershell",
            "https://docs.microsoft.com/sql",
            "https://docs.microsoft.com/surface",
            "https://docs.microsoft.com/system-center",
            "https://docs.microsoft.com/visualstudio",
            "https://docs.microsoft.com/windows",
            "https://docs.microsoft.com/xamarin"
};

await SumPageSizesAsync2();

 async Task SumPageSizesAsync2()
{
    var stopwatch = Stopwatch.StartNew();

    IEnumerable<Task<int>> downloadTasksQuery =
        from url in s_urlList2
        select ProcessUrlAsync2(url, s_client2);

    List<Task<int>> downloadTasks = downloadTasksQuery.ToList();

    int total = 0;
    while (downloadTasks.Any())
    {
        Task<int> finishedTask = await Task.WhenAny(downloadTasks);
        downloadTasks.Remove(finishedTask);
        int finishNumber = await finishedTask;
        Console.WriteLine("complete: " + finishNumber.ToString());
        total += await finishedTask;
    }

    stopwatch.Stop();

    Console.WriteLine($"\nTotal bytes returned:  {total:#,#}");
    Console.WriteLine($"Elapsed time:          {stopwatch.Elapsed}\n");
}

 async Task<int> ProcessUrlAsync2(string url, HttpClient client)
{
    byte[] content = await client.GetByteArrayAsync(url);
    Console.WriteLine($"{url,-60} {content.Length,10:#,#}");

    return content.Length;
}
