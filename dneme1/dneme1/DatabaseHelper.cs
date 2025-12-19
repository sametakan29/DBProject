using System;
using Npgsql;

namespace dneme1
{
    public static class DatabaseHelper
    {
        // Baðlantý bilgileri
        private const string Host = "localhost";
        private const string Port = "5432";
        private const string Database = "boyamakinedevami";
        private const string Username = "postgres"; 
        private const string Password = "admin";  

        private static string ConnectionString =>//baðlantý bilgilerini birleþtirir
            $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password}";

        /// Yeni bir PostgreSQL baðlantýsý oluþturur
        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }

        /// Veritabaný baðlantýsýný test eder
        public static bool TestConnection(out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

    }
}
