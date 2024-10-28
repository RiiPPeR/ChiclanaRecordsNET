using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Configuration;

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

            var userToken = ConfigurationManager.AppSettings["DiscogsKey"];

            _client.DefaultRequestHeaders.Add("User-Agent", "MyDiscogsApp/1.0");
            _client.DefaultRequestHeaders.Add("Authorization", $"Discogs token={userToken}");
        }

        public async Task<Album> GetReleaseAsync(int releaseId)
        {
            try
            {
                var response = await _client.GetAsync($"releases/{releaseId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var release = JsonSerializer.Deserialize<Album>(content, new JsonSerializerOptions
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

    public class Artist
    {
        public string anv { get; set; }
        public int? id { get; set; }
        public string join { get; set; }
        public string name { get; set; }
        public string resource_url { get; set; }
        public string role { get; set; }
        public string tracks { get; set; }
    }

    public class Community
    {
        public List<Contributor> contributors { get; set; }
        public string data_quality { get; set; }
        public int? have { get; set; }
        public Rating rating { get; set; }
        public string status { get; set; }
        public Submitter submitter { get; set; }
        public int? want { get; set; }
    }

    public class Company
    {
        public string catno { get; set; }
        public string entity_type { get; set; }
        public string entity_type_name { get; set; }
        public int? id { get; set; }
        public string name { get; set; }
        public string resource_url { get; set; }
    }

    public class Contributor
    {
        public string resource_url { get; set; }
        public string username { get; set; }
    }

    public class Extraartist
    {
        public string anv { get; set; }
        public int? id { get; set; }
        public string join { get; set; }
        public string name { get; set; }
        public string resource_url { get; set; }
        public string role { get; set; }
        public string tracks { get; set; }
    }

    public class Format
    {
        public List<string> descriptions { get; set; }
        public string name { get; set; }
        public string qty { get; set; }
    }

    public class Identifier
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Image
    {
        public int? height { get; set; }
        public string resource_url { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
        public string uri150 { get; set; }
        public int? width { get; set; }
    }

    public class Label
    {
        public string catno { get; set; }
        public string entity_type { get; set; }
        public int? id { get; set; }
        public string name { get; set; }
        public string resource_url { get; set; }
    }

    public class Rating
    {
        public double? average { get; set; }
        public int? count { get; set; }
    }

    public class Album
    {
        public string title { get; set; }
        public int? id { get; set; }
        public List<Artist> artists { get; set; }
        public string data_quality { get; set; }
        public string thumb { get; set; }
        public Community community { get; set; }
        public List<Company> companies { get; set; }
        public string country { get; set; }
        public DateTime? date_added { get; set; }
        public DateTime? date_changed { get; set; }
        public int? estimated_weight { get; set; }
        public List<Extraartist> extraartists { get; set; }
        public int? format_quantity { get; set; }
        public List<Format> formats { get; set; }
        public List<string> genres { get; set; }
        public List<Identifier> identifiers { get; set; }
        public List<Image> images { get; set; }
        public List<Label> labels { get; set; }
        public double? lowest_price { get; set; }
        public int? master_id { get; set; }
        public string master_url { get; set; }
        public string notes { get; set; }
        public int? num_for_sale { get; set; }
        public string released { get; set; }
        public string released_formatted { get; set; }
        public string resource_url { get; set; }
        public List<object> series { get; set; }
        public string status { get; set; }
        public List<string> styles { get; set; }
        public List<Tracklist> tracklist { get; set; }
        public string uri { get; set; }
        public List<Video> videos { get; set; }
        public int? year { get; set; }
    }

    public class Submitter
    {
        public string resource_url { get; set; }
        public string username { get; set; }
    }

    public class Tracklist
    {
        public string duration { get; set; }
        public string position { get; set; }
        public string title { get; set; }
        public string type_ { get; set; }
    }

    public class Video
    {
        public string description { get; set; }
        public int? duration { get; set; }
        public bool? embed { get; set; }
        public string title { get; set; }
        public string uri { get; set; }
    }
}