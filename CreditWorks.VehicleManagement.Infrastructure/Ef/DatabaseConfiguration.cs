using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CreditWorks.VehicleManagement.Infrastructure.Ef
{
    /// <summary>
    ///     Builds the SQL Server connection string from application configuration.
    /// </summary>
    public static class DatabaseConfiguration
    {
        /// <summary>
        ///     Gets the configured connection string or constructs one from database settings.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>A SQL Server connection string.</returns>
        /// <exception cref="InvalidOperationException">A required database setting is missing.</exception>
        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(configuration["Database:Server"]) && !string.IsNullOrWhiteSpace(connectionString)) return connectionString;

            string Required(string key) => !string.IsNullOrWhiteSpace(configuration[key])
                ? configuration[key]!
                : throw new InvalidOperationException($"Missing database configuration: {key}.");

            return new SqlConnectionStringBuilder
            {
                DataSource = Required("Database:Server"),
                InitialCatalog = Required("Database:Name"),
                UserID = Required("Database:Username"),
                Password = Required("Database:Password"),
                Encrypt = true,
                TrustServerCertificate = false
            }.ConnectionString;
        }
    }
}
