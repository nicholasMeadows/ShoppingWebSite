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


            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            
            MySqlCommand query = new MySqlCommand("INSERT INTO `users`.`users` (`username`, `hash`, `salt`) VALUES (@username, @hash, @salt);", connection);
            query.Parameters.Add("@username", MySqlDbType.VarChar).Value = registerUser.username;
            query.Parameters.Add("@hash", MySqlDbType.VarChar).Value = hash;
            query.Parameters.Add("@salt", MySqlDbType.Blob).Value = salt;

            query.ExecuteNonQuery();
            connection.Close();

        }

        public static bool CheckUsername(string username) {

            connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand query = new MySqlCommand("SELECT * FROM `users`.`users` WHERE username = @username", connection);
            query.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;
            MySqlDataReader reader = query.ExecuteReader();
            reader.Read();

            if (!reader.HasRows)
            {
                reader.Close();
                connection.Close();
                return true;
            }
            return false;
        }

        public static AccessTokenModel GenerateAccessToken(string username)
        {

            char[] accessToken = new char[151];
            Random rand = new Random();

            for (int i = 0; i < 151; ++i)
            {
                int charAsNum = rand.Next(45, 122);

                while (charAsNum >= 58 && charAsNum <= 64 || charAsNum >= 91 && charAsNum <= 94 || charAsNum == 96 || charAsNum >= 46 && charAsNum <= 47)
                {
                    charAsNum = rand.Next(48, 122);
                }
                accessToken[i] = (char)charAsNum;
            }


            char[] refreshToken = new char[131];


            for (int i = 0; i < 131; ++i)
            {
                int charAsNum = rand.Next(45, 122);

                while (charAsNum >= 58 && charAsNum <= 64 || charAsNum >= 91 && charAsNum <= 94 || charAsNum == 96 || charAsNum >= 46 && charAsNum <= 47)
                {
                    charAsNum = rand.Next(48, 122);
                }
                refreshToken[i] = (char)charAsNum;
            }


            AccessTokenModel token = new AccessTokenModel();
            token.accessToken = new string(accessToken);
            token.expiresIn = 3600; //Seconds
            token.refreshToken = new string(refreshToken);
            

            connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand query = new MySqlCommand("INSERT INTO `users`.`tokens` (username, token, refreshToken, timestamp) VALUES (@username, @accessToken, @refreshToken, CURRENT_TIMESTAMP);", connection);
            query.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;
            query.Parameters.Add("@accessToken", MySqlDbType.VarChar).Value = token.accessToken;
            query.Parameters.Add("@refreshToken", MySqlDbType.VarChar).Value = token.refreshToken;

            query.ExecuteNonQuery();
            connection.Close();
            return token;
        }
    }
}
