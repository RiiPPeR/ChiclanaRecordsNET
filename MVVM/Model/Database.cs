using Supabase;
using Supabase.Gotrue.Exceptions;
using Supabase.Postgrest;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Security;


namespace ChiclanaRecordsNET.MVVM.Model
{
    [Table("users")]
    public class User : BaseModel
    {
        [PrimaryKey("id")]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("friends")]
        public List<Guid> Friends { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    // TODO: FALTA EL ARTISTA
    [Table("records")]
    public class Record : BaseModel
    {
        [PrimaryKey("discogs_id")]
        [Column("discogs_id")]
        public int DiscogsId { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("image_url")]
        public string ImageUrl { get; set; }

        [Column("country")]
        public string Country { get; set; }

        [Column("year")]
        public int Year { get; set; }

        [Column("label")]
        public string Label { get; set; }

        [Column("catno")]
        public string CatalogNumber { get; set; }
    }

    [Table("user_records")]
    public class UserRecord : BaseModel
    {
        [PrimaryKey("id")]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("discogs_id")]
        public int DiscogsId { get; set; }

        [Column("added_at")]
        public DateTime AddedAt { get; set; }
    }

    public class Database
    {
        private readonly Supabase.Client _supabase;

        public Database()
        {
            var supabaseUrl = ConfigurationManager.AppSettings["SupabaseUrl"];
            var supabaseKey = ConfigurationManager.AppSettings["SupabaseKey"];

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true,
            };

            _supabase = new Supabase.Client(supabaseUrl, supabaseKey, options);
        }


        public async Task<(User? user, string? error)> LogIn(string username, SecureString password)
        {
            try
            {
                var result = await _supabase.From<User>()
                    .Where(x => x.Username == username)
                    .Single();

                if (result == null)
                {
                    return (null, "Usuario no encontrado");
                }

                IntPtr unmanagedString = IntPtr.Zero;
                try
                {
                    unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(password);
                    string? passwordString = Marshal.PtrToStringUni(unmanagedString);

                    var session = await _supabase.Auth.SignIn(result.Email, passwordString);

                    if (session?.User != null)
                    {
                        return (result, null);
                    }

                    return (null, "Contraseña incorrecta");
                }
                finally
                {
                    if (unmanagedString != IntPtr.Zero)
                    {
                        Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
                    }
                }
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        public async Task<(bool success, string? error)> CreateUser(string username, string email, SecureString password)
        {
            try
            {
                IntPtr unmanagedString = IntPtr.Zero;
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(password);
                string? passwordString = Marshal.PtrToStringUni(unmanagedString);

                var result = await _supabase.From<User>()
                    .Where(x => x.Username == username)
                    .Single();

                if (result != null)
                {
                    return (false, "Ese usuario ya existe");
                }

                var authResponse = await _supabase.Auth.SignUp(email, passwordString);

                if (authResponse?.User != null)
                {
                    var user = new User
                    {
                        Id = Guid.Parse(authResponse.User.Id),
                        Username = username,
                        Email = email,
                        Friends = new List<Guid>(),
                        CreatedAt = DateTime.UtcNow
                    };

                    await _supabase.From<User>()
                        .Insert(user);

                    return (true, null);
                }
                return (false, "Error al crear el usuario");
            }
            catch (GotrueException ex)
            {
                return (false, ex.Message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }


        public async Task<List<Record>> GetUserRecords(Guid userId)
        {
            try
            {
                // first get the users record associations
                var userRecordsResponse = await _supabase
                    .From<UserRecord>()
                    .Filter("user_id", Constants.Operator.Equals, userId.ToString())
                    .Get();

                if (!userRecordsResponse.Models.Any())
                    return new List<Record>();

                // then get the discogs_ids from the user records
                var discogsIds = userRecordsResponse.Models.Select(ur => ur.DiscogsId).ToList();

                // then get the actual records using those discogs_ids
                var recordsResponse = await _supabase
                    .From<Record>()
                    .Filter("discogs_id", Constants.Operator.In, discogsIds)
                    .Get();

                return recordsResponse.Models.ToList();
            }
            catch
            {
                return new List<Record>();
            }
        }

        public async Task<Record?> GetRecord(int discogsId)
        {
            try
            {
                var response = await _supabase
                    .From<Record>()
                    .Filter("discogs_id", Constants.Operator.Equals, discogsId)
                    .Get();

                return response.Models.FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Record>> SearchRecords(string searchTerm)
        {
            try
            {
                var response = await _supabase
                    .From<Record>()
                    .Filter("title", Constants.Operator.ILike, $"%{searchTerm}%")
                    .Get();

                return response.Models.ToList();
            }
            catch
            {
                return new List<Record>();
            }
        }

        // TODO: NO LE HE PUESTO EL PUTO ARTISTA
        public async Task<(bool success, string? error)> AddRecordToCollection(
            Guid userId,
            int discogsId,
            string title,
            string imageUrl,
            string country,
            int year,
            string label,
            string catalogNumber)
        {
            try
            {
                // does the record exist?
                var existingRecordResponse = await _supabase
                    .From<Record>()
                    .Filter("discogs_id", Constants.Operator.Equals, discogsId)
                    .Get();

                var existingRecord = existingRecordResponse.Models.FirstOrDefault();

                // if not, add it
                if (existingRecord == null)
                {
                    var record = new Record
                    {
                        DiscogsId = discogsId,
                        Title = title,
                        ImageUrl = imageUrl,
                        Country = country,
                        Year = year,
                        Label = label,
                        CatalogNumber = catalogNumber
                    };

                    await _supabase
                        .From<Record>()
                        .Insert(record);
                }

                // check if user has the record
                var existingUserRecordResponse = await _supabase
                    .From<UserRecord>()
                    .Filter("user_id", Constants.Operator.Equals, userId.ToString())
                    .Filter("discogs_id", Constants.Operator.Equals, discogsId)
                    .Get();

                var existingUserRecord = existingUserRecordResponse.Models.FirstOrDefault();

                if (existingUserRecord != null)
                {
                    return (false, "Ya has añadido ese disco.");
                }

                // IMPLEMENT UUID FOR USER_RECORDS, IT ALWAYS TRIES TO PUT A 0 ON TO THE NEW RECORDS
                // then add the userrecord
                var userRecord = new UserRecord
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    DiscogsId = discogsId,
                    AddedAt = DateTime.UtcNow
                };

                await _supabase
                    .From<UserRecord>()
                    .Insert(userRecord);

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool success, string? error)> DeleteRecordFromCollection(Guid userId, int discogsId)
        {
            try
            {
                await _supabase
                    .From<UserRecord>()
                    .Filter("user_id", Constants.Operator.Equals, userId.ToString())
                    .Filter("discogs_id", Constants.Operator.Equals, discogsId)
                    .Delete();

                // TODO: implement if no one else has that record associated it gets deleted

                var response = await _supabase
                    .From<UserRecord>()
                    .Filter("discogs_id", Constants.Operator.Equals, discogsId)
                    .Get();

                if (response.Model == null)
                {
                    await _supabase
                        .From<Record>()
                        .Filter("discogs_id", Constants.Operator.Equals, discogsId)
                        .Delete();
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<bool> HasRecord(Guid userId, int discogsId)
        {
            try
            {
                var response = await _supabase
                    .From<UserRecord>()
                    .Filter("user_id", Constants.Operator.Equals, userId.ToString())
                    .Filter("discogs_id", Constants.Operator.Equals, discogsId)
                    .Get();

                return response.Models.Any();
            }
            catch
            {
                return false;
            }
        }
    }

    //public async Task<(bool success, string? error)> AddRecordToCollection(Guid id, int record_id)
    //{
    //    try
    //    {
    //        var user = await _supabase.From<User>()
    //            .Where(x => x.Id == id)
    //            .Single();

    //        if (user == null)
    //        {
    //            return (false, "Usuario no encontrado");
    //        }

    //        if (user.Records.Contains(record_id))
    //        {
    //            return (false, "Ya has añadido ese disco.");
    //        }

    //        user.Records.Add(record_id);

    //        await _supabase
    //            .From<User>()
    //            .Update(user);

    //        return (true, null);
    //    }
    //    catch (Exception ex)
    //    {
    //        return (false, ex.Message);
    //    }
    //}

    //public async Task<(bool success, string? error)> DeleteRecordFromCollection(Guid id, int record_id)
    //{
    //    try
    //    {
    //        var user = await _supabase.From<User>()
    //            .Where(x => x.Id == id)
    //            .Single();

    //        if (user == null)
    //        {
    //            return (false, "Usuario no encontrado");
    //        }

    //        user.Records.Remove(record_id);

    //        await _supabase
    //            .From<User>()
    //            .Update(user);

    //        return (true, null);
    //    }
    //    catch (Exception ex)
    //    {
    //        return (false, ex.Message);
    //    }
    //}

}


