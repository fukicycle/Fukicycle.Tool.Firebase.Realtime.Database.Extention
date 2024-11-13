using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
using Fukicycle.Tool.Firebase.Realtime.Database.Extention.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Newtonsoft.Json;


namespace Fukicycle.Tool.Firebase.Realtime.Database.Extention
{
    public static class WASMSubscriber
    {
        public static async Task SubscribeForWASMAsync<T>(this ChildQuery childQuery, Action<T> onNext, Action<string> onError)
        {
            string url = await childQuery.BuildUrlAsync();
            using HttpClient httpClient = new HttpClient();
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/stream-event"));
            requestMessage.SetBrowserResponseStreamingEnabled(true);
            HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);
            responseMessage.EnsureSuccessStatusCode();
            using Stream stream = await responseMessage.Content.ReadAsStreamAsync();
            using StreamReader reader = new StreamReader(stream);
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    string? line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }
                    if (line.StartsWith("data: "))
                    {
                        try
                        {
                            string content = line.Substring(6, line.Length - 6);
                            SubscribeResponse<T>? response = JsonConvert.DeserializeObject<SubscribeResponse<T>>(content);
                            if (response != null)
                            {
                                onNext(response.Object);
                            }
                        }
                        catch (Exception ex)
                        {
                            onError(ex.Message);
                        }
                    }
                }
            });
        }
    }
}
