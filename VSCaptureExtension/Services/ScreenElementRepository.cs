using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using EnvDTE;
using EnvDTE80;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.Shell;

namespace VSCaptureExtension.Model
{
    /// <summary>
    /// Repository for managing screen elements in a SQLite database.
    /// </summary>
    public class ScreenElementRepository //TODO : IRepositoryService
    {
        private readonly string _dbPath;

        public ScreenElementRepository()
        {
            string assemblyDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("The assembly path is null");
            _dbPath = Path.Combine(assemblyDir, "ScreenElements.db");
            InitializeDatabase();
        }

        public ScreenElementRepository(string databaseFilePath)
        {
            _dbPath = databaseFilePath;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(_dbPath))
            {
                throw new FileNotFoundException($"Database file not found at {_dbPath}");
                //File.Create(_dbPath).Dispose(); // Create the file if it doesn't exist
            }

            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            //using var connection =    new SQLiteConnection($"Data Source=ScreenElements.db");
            connection.Open();

            var command = connection.CreateCommand();



            command.CommandText =
            @"
                CREATE TABLE IF NOT EXISTS ScreenElements (
                    Id TEXT PRIMARY KEY,
                    Text TEXT,
                    Image BLOB);
            ";

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Inserts or updates a screen element in the database.
        /// </summary>
        /// <param name="item"></param>
        public void InsertOrUpdateScreenElement(string id, string text, BitmapImage image)
        {
            var item = new ScreenElement
            {
                Id = id,
                Text = text,
                Image = image
            };

            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
            INSERT INTO ScreenElements (Id, Text, Image)
            VALUES (@Id, @Text, @Image)
            ON CONFLICT(Id) DO UPDATE SET
                Text = excluded.Text,
                Image = excluded.Image;
            ";

            command.Parameters.AddWithValue("@Id", item.Id);
            command.Parameters.AddWithValue("@Text", (object)item.Text ?? DBNull.Value);
            if (item.Image != null)
            {
                command.Parameters.AddWithValue("@Image", BitmapImageToPngBytes(item.Image));
            }
            else
            {
                command.Parameters.AddWithValue("@Image", DBNull.Value);
            }

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Gets the screen element by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ScreenElement GetScreenElementById(string id)
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = "SELECT Id, Text, Image FROM ScreenElements WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new ScreenElement
                {
                    Id = reader.GetString(0),
                    Text = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Image = reader.IsDBNull(2) ? null : ByteArrayToBitmapImage((byte[])reader["Image"])
                };
            }

            return null;
        }

        /// <summary>
        /// Deletes a screen element.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteScreenElement(string id)
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM ScreenElements WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }

        private BitmapImage ByteArrayToBitmapImage(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze(); // Optional but recommended for cross-thread use
            return image;
        }


        private byte[] BitmapImageToPngBytes(BitmapImage image)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using var stream = new MemoryStream();
            encoder.Save(stream);
            return stream.ToArray();
        }

    }
}
