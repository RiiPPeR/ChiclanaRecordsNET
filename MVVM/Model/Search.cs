using System.IO;
using System.Net.Http;
using System.Text.Json;

namespace ChiclanaRecordsNET.MVVM.Model
{
    public class DiscogsSearch
    {
        private readonly HttpClient _client;

        public DiscogsSearch()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://api.discogs.com/")
            };

            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string apiFilePath = Path.Combine(currentDirectory, "MVVM", "Model", "api.txt");

            System.Diagnostics.Debug.WriteLine($"Path: {apiFilePath}");

            var userToken = File.ReadAllText(apiFilePath).Trim();
           
            System.Diagnostics.Debug.WriteLine($"Api key: {userToken}");

            _client.DefaultRequestHeaders.Add("User-Agent", "MyDiscogsApp/1.0");
            _client.DefaultRequestHeaders.Add("Authorization", $"Discogs token={userToken}");
        }

        public async Task<SearchResponse> GetSearchAsync (string artist, string title)
        {
            try
            {
                var response = await _client.GetAsync($"database/search?format=vinyl&artist={artist}&title={title}&type=release&country=spain");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var search = JsonSerializer.Deserialize<SearchResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return search ?? new SearchResponse();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }
    }
}


public class SearchResponse
{
    public Pagination Pagination { get; set; }
    public List<SearchResult> Results { get; set; }
}

public class Pagination
{
    public int PerPage { get; set; }
    public int Pages { get; set; }
    public int Page { get; set; }
    public PaginationUrls Urls { get; set; }
    public int Items { get; set; }
}

public class PaginationUrls
{
    public string Last { get; set; }
    public string Next { get; set; }
}

public class SearchResult
{
    public List<string> Style { get; set; }
    public string Thumb { get; set; }
    public string Title { get; set; }
    public string Country { get; set; }
    public List<string> Format { get; set; }
    public string Uri { get; set; }
    public CommunityD Community { get; set; }
    public List<string> Label { get; set; }
    public string Catno { get; set; }
    public string Year { get; set; }
    public List<string> Genre { get; set; }
    public string ResourceUrl { get; set; }
    public string Type { get; set; }
    public int Id { get; set; }
    public string LabelFirst
    {
        get => Label.FirstOrDefault();
    }
}

public class CommunityD
{
    public int Want { get; set; }
    public int Have { get; set; }
}
