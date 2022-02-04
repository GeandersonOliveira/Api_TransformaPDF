namespace Infrastructure
{
    public class JsonData
    {
        public readonly IHttpClientFactory HttpClientFactory;

        public JsonData(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory;
        }

        public async Task<byte[]> GetAndReadByteArrayAsync(string url)
        {
            HttpClient httpClient = HttpClientFactory.CreateClient("default");
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception(await response.Content.ReadAsStringAsync());

            var result = await response.Content.ReadAsByteArrayAsync();

            return result;
        }

    }
}