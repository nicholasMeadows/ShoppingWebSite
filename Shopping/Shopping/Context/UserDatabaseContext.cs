using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Shopping.Models;
using MySql.Data.MySqlClient;

namespace Shopping.Context
{
    public static class UserDatabaseContext
    {

        private static MySqlConnection connection;
        private static string server = "98.179.199.29";
        private static string database = "users";
        private static string UID = "fullAccess";
        private static string dbPassword = "fullAccess";
        private static string connectionString = "SERVER=" + server + ";" +
                                "DATABASE=" + database + ";" +
                                "UID=" + UID + ";" +
                                "PASSWORD=" + dbPassword + ";";

        public static bool Login(UserPassModel userPass)
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand query = new MySqlCommand("SELECT * FROM `users`.`users` WHERE username = @username",connection);
            query.Parameters.Add("@username", MySqlDbType.VarChar).Value = userPass.username;

            MySqlDataReader reader = query.ExecuteReader();
            reader.Read();

            if (reader.HasRows)
            {
                string hashFromDatabase = (string)reader["hash"];
                byte[] salt = (byte[])reader["salt"];

                reader.Close();

                string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: userPass.password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

                if (hashFromDatabase.Equals(hash))
                    return true;
            }

            return false;           
        }

        public static void Register(RegisterUserModel registerUser)
        {
            // generate a 128 - bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: registerUser.password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand query = new MySqlCommand("INSERT INTO `users`.`users` (`username`, `hash`, `salt`) VALUES (@username, @hash, @salt);", connection);
            query.Parameters.Add("@username", MySqlDbType.VarChar).Value = registerUser.username;
            query.Parameters.Add("@hash", MySqlDbType.VarChar).Value = hash;
            query.Parameters.Add("@salt", MySqlDbType.Blob).Value = salt;

            query.ExecuteNonQuery();
            connection.Close();

        }

    }
}
