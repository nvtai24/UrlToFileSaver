using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    private static readonly HttpClient client = new HttpClient();

    static async Task Main()
    {
        try
        {
            Console.WriteLine("Enter URL:");
            string? url = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(url))
            {
                Console.WriteLine("Invalid URL.");
                return;
            }

            Console.WriteLine("Downloading...");
            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            string fileName = Path.GetFileName(new Uri(url).LocalPath);
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "downloaded_file";
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);

            byte[] buffer = new byte[8192]; // 8 KB buffer
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
            }

            Console.WriteLine($"File downloaded successfully: {fileName}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP error: {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"File writing error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}
