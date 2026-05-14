namespace App.Core.Options
{
    public class ConnectionOptions
    {
        public const string Connections = "Connections";

        public string MssqlServer { get; set; } = string.Empty;
        public string PostgreSql { get; set; } = string.Empty;
        public string Hangfire { get; set; } = string.Empty;
        public string Redis { get; set; } = string.Empty;
        public string DatabaseProvider { get; set; } = "MsSqlServer";
    }
}
