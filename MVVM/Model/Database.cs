﻿using Supabase;
using Supabase.Gotrue.Exceptions;
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
        public Guid Id { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("records")]
        public List<int> Records { get; set; }

        [Column("friends")]
        public List<Guid> Friends { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

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

        public async Task<(bool success, string? error)> CreateUser(string username, string email, string password)
        {
            try
            {
                var authResponse = await _supabase.Auth.SignUp(email, password);

                if (authResponse?.User != null)
                {
                    var user = new User
                    {
                        Id = Guid.Parse(authResponse.User.Id),
                        Username = username,
                        Email = email,
                        Password = password,
                        Records = new List<int>(),
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
    }
}
