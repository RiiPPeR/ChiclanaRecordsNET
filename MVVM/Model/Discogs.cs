using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace ChiclanaRecordsNET.MVVM.Model
{
    public class DiscogsClient
    {
        private readonly HttpClient _client;

        public DiscogsClient()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://api.discogs.com/")
            };

            _client.DefaultRequestHeaders.Add("User-Agent", "MyDiscogsApp/1.0");
        }

        public async Task<Release> GetReleaseAsync(int releaseId)
        {
            try
            {
                var response = await _client.GetAsync($"releases/{releaseId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var release = JsonSerializer.Deserialize<Release>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return release;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error fetching release: {ex.Message}");
            }
        }
    }

    public class Release
    {
        public string Title { get; set; }
        public int Id { get; set; }
        public List<Artist> Artists { get; set; }
        public string DataQuality { get; set; }
        public string Country { get; set; }
        public int Year { get; set; }
        public List<Format> Formats { get; set; }
        public List<string> Genres { get; set; }
        public List<string> Styles { get; set; }
        public List<Label> Labels { get; set; }
        public List<Track> Tracklist { get; set; }
        public string Uri { get; set; }
        public List<Video> Videos { get; set; }
        public decimal LowestPrice { get; set; }
        public string Notes { get; set; }
        public int NumForSale { get; set; }
        public Community Community { get; set; }
        public List<Image> Images { get; set; }  // Added Images
        public string Thumb { get; set; }
    }

    public class Image
    {
        public string Type { get; set; }
        public string Uri { get; set; }
        public string ResourceUrl { get; set; }
        public string Uri150 { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ResourceUrl { get; set; }
        public string Role { get; set; }
        public string Join { get; set; }
        public string Anv { get; set; }
        public string Tracks { get; set; }
    }

    public class Format
    {
        public string Name { get; set; }
        public string Qty { get; set; }
        public List<string> Descriptions { get; set; }
    }

    public class Track
    {
        public string Position { get; set; }
        public string Title { get; set; }
        public string Duration { get; set; }
        public string Type { get; set; }
    }

    public class Label
    {
        public string Name { get; set; }
        public string Catno { get; set; }
        public int Id { get; set; }
        public string ResourceUrl { get; set; }
    }

    public class Video
    {
        public string Uri { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public bool Embed { get; set; }
    }

    public class Community
    {
        public Rating Rating { get; set; }
        public int Have { get; set; }
        public int Want { get; set; }
    }

    public class Rating
    {
        public decimal Average { get; set; }
        public int Count { get; set; }
    }
}
