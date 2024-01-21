namespace Constants
{
    public static class Hosts
    {
#if DEBUG
        public const string HTML_FILE_REPOSITORY_HOST =  Secrets.HTML_FILE_REPOSITORY_FOLDER;
        public const int HTML_FILE_REPOSITORY_PORT = Secrets.HTML_FILE_REPOSITORY_PORT;
        public const string HTML_FILE_REPOSITORY_USER = Secrets.HTML_FILE_REPOSITORY_USER;
        public const string HTML_FILE_REPOSITORY_PASSWORD = Secrets.HTML_FILE_REPOSITORY_PASSWORD;
        public const string HTML_FILE_REPOSITORY_FOLDER = Secrets.HTML_FILE_REPOSITORY_FOLDER;
        public const string PDF_FILE_REPOSITORY_HOST = Secrets.PDF_FILE_REPOSITORY_HOST;
        public const int PDF_FILE_REPOSITORY_PORT = Secrets.PDF_FILE_REPOSITORY_PORT;
        public const string PDF_FILE_REPOSITORY_USER = Secrets.PDF_FILE_REPOSITORY_USER;
        public const string PDF_FILE_REPOSITORY_PASSWORD = Secrets.PDF_FILE_REPOSITORY_PASSWORD;
        public const string PDF_FILE_REPOSITORY_FOLDER = Secrets.PDF_FILE_REPOSITORY_FOLDER;
#else
        public static readonly string HTML_FILE_REPOSITORY_HOST = Environment.GetEnvironmentVariable("FILE_REPOSITORY_HOST")!;
        public static readonly int HTML_FILE_REPOSITORY_PORT = int.Parse(Environment.GetEnvironmentVariable("FILE_REPOSITORY_PORT")!);
        public static readonly string HTML_FILE_REPOSITORY_USER = Environment.GetEnvironmentVariable("FILE_REPOSITORY_USER")!;
        public static readonly string HTML_SFTP_PASSWORD = Environment.GetEnvironmentVariable("SFTP_PASSWORD")!;
        public static readonly string HTML_FILE_REPOSITORY_FOLDER = Environment.GetEnvironmentVariable("FILE_REPOSITORY_FOLDER")!;
        public static readonly string PDF_FILE_REPOSITORY_HOST = Environment.GetEnvironmentVariable("FILE_REPOSITORY_HOST")!;
        public static readonly int PDF_FILE_REPOSITORY_PORT = int.Parse(Environment.GetEnvironmentVariable("FILE_REPOSITORY_PORT")!);
        public static readonly string PDF_FILE_REPOSITORY_USER = Environment.GetEnvironmentVariable("FILE_REPOSITORY_USER")!;
        public static readonly string PDF_SFTP_PASSWORD = Environment.GetEnvironmentVariable("SFTP_PASSWORD")!;
        public static readonly string PDF_FILE_REPOSITORY_FOLDER = Environment.GetEnvironmentVariable("FILE_REPOSITORY_FOLDER")!;
#endif

#if DEBUG
        public const string RABBITMQ_HOST = Secrets.RABBITMQ_HOST;
        public const int RABBITMQ_PORT = Secrets.RABBITMQ_PORT;
        public const string RABBITMQ_NAME = Secrets.RABBITMQ_NAME;
        public const string RABBITMQ_PASSWORD = Secrets.RABBITMQ_PASSWORD;
#else
        public static readonly string RABBITMQ_HOST = Environment.GetEnvironmentVariable("RABBITMQ_HOST")!;
        public static readonly int RABBITMQ_PORT = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT")!);
        public static readonly string RABBITMQ_NAME = Environment.GetEnvironmentVariable("RABBITMQ_NAME")!;
        public static readonly string RABBITMQ_PASSWORD = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")!;
#endif
    }
}