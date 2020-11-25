using System;
using System.Net.Http;
using System.Threading.Tasks;

//運行需求 .net5 toplevel 

/*
 * caller 也要加上 await 才會等待 call method 的 await
 * 不然會在 getStringTask 時 Yield caller 然後就結束了
 */

await GetUrlContentLengthAsync();

Console.WriteLine("end");

async Task<int> GetUrlContentLengthAsync()
{
    var client = new HttpClient();

    //按照慣例，非同步方法的名稱是以 "Async" 後置字元為結尾。
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

