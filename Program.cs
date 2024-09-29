using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        List<string> urls = new List<string>
        {
            "https://www.coursera.org/",
            "https://www.microsoft.com",
            "https://www.github.com",
        };

        CancellationTokenSource cts = new CancellationTokenSource();

        cts.CancelAfter(TimeSpan.FromSeconds(10));

        Console.WriteLine("Нажмите любую клавишу для отмены операции вручную.");

        Task task = Task.Run(() =>
        {
            Console.ReadKey();
            cts.Cancel();
        });

        try
        {
            await DownloadAllPagesAsync(urls, cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("\nОперация была отменена.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nПроизошла ошибка: {ex.Message}");
        }
    }

    static async Task DownloadAllPagesAsync(List<string> urls, CancellationToken token)
    {
        HttpClient httpClient = new HttpClient();

        List<Task> downloadTasks = new List<Task>();

        foreach (string url in urls)
        {
            downloadTasks.Add(DownloadPageAsync(httpClient, url, token));
        }

        await Task.WhenAll(downloadTasks);
    }

    static async Task DownloadPageAsync(HttpClient client, string url, CancellationToken token)
    {
        try
        {
            Console.WriteLine($"Загрузка: {url}");

            HttpResponseMessage response = await client.GetAsync(url, token);

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Загрузка завершена: {url} ({content.Length} символов)");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"Загрузка отменена: {url}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке {url}: {ex.Message}");
        }
    }
}
