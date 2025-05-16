

using Microsoft.Data.Sqlite;

namespace POC_Tesseract.Database
{
    internal class DataRepository
    {
        private readonly string _dbPath;
        
        public DataRepository(string databaseFilePath)
        {
            _dbPath = databaseFilePath;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(_dbPath))
            {
                //throw new FileNotFoundException($"Database file not found at {_dbPath}");
                File.Create(_dbPath).Dispose(); // Create the file if it doesn't exist
            }

            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText =
            @"
                CREATE TABLE IF NOT EXISTS Texts (
                    Id TEXT PRIMARY KEY,
                    Content TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Images (
                    Id TEXT PRIMARY KEY,
                    Data BLOB NOT NULL
                );

                CREATE TABLE IF NOT EXISTS ScreenElements (
                    Id TEXT PRIMARY KEY,
                    TextId TEXT NOT NULL,
                    ImageId TEXT NOT NULL,
                    FOREIGN KEY (TextId) REFERENCES Texts(Id),
                    FOREIGN KEY (ImageId) REFERENCES Images(Id)
                );
            ";

            command.ExecuteNonQuery();
        }
        public void AddElement(DataBaseElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (string.IsNullOrWhiteSpace(element.Id))
                throw new ArgumentException("Element Id cannot be null or empty.", nameof(element));

            bool hasText = !string.IsNullOrWhiteSpace(element.Text);
            bool hasImage = element.Image != null && element.Image.Length > 0;

            if (!hasText && !hasImage)
                throw new ArgumentException("At least one of Text or Image must be provided.");

            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            using var transaction = connection.BeginTransaction();

            // Generate unique IDs for text and image if present
            string? textId = hasText ? Guid.NewGuid().ToString() : null;
            string? imageId = hasImage ? Guid.NewGuid().ToString() : null;

            // Insert into Texts if text is present
            if (hasText)
            {
                var insertTextCmd = connection.CreateCommand();
                insertTextCmd.CommandText = @"
                    INSERT OR IGNORE INTO Texts (Id, Content) VALUES ($id, $content);";
                insertTextCmd.Parameters.AddWithValue("$id", textId);
                insertTextCmd.Parameters.AddWithValue("$content", element.Text);
                insertTextCmd.ExecuteNonQuery();
            }

            // Insert into Images if image is present
            if (hasImage)
            {
                var insertImageCmd = connection.CreateCommand();
                insertImageCmd.CommandText = @"
                    INSERT OR IGNORE INTO Images (Id, Data) VALUES ($id, $data);";
                insertImageCmd.Parameters.AddWithValue("$id", imageId);
                insertImageCmd.Parameters.AddWithValue("$data", element.Image);
                insertImageCmd.ExecuteNonQuery();
            }

            // Insert into ScreenElements
            var insertScreenElementCmd = connection.CreateCommand();
            insertScreenElementCmd.CommandText = @"
                INSERT OR IGNORE INTO ScreenElements (Id, TextId, ImageId) VALUES ($id, $textId, $imageId);";
            insertScreenElementCmd.Parameters.AddWithValue("$id", element.Id);
            insertScreenElementCmd.Parameters.AddWithValue("$textId", (object?)textId ?? DBNull.Value);
            insertScreenElementCmd.Parameters.AddWithValue("$imageId", (object?)imageId ?? DBNull.Value);
            insertScreenElementCmd.ExecuteNonQuery();

            transaction.Commit();
        }

        public void DeleteElement(string screenElementId)
        {
            if (string.IsNullOrWhiteSpace(screenElementId))
                throw new ArgumentException("ScreenElement Id cannot be null or empty.", nameof(screenElementId));

            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            using var transaction = connection.BeginTransaction();

            // Get associated TextId and ImageId
            string? textId = null;
            string? imageId = null;
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = @"
                SELECT TextId, ImageId FROM ScreenElements WHERE Id = $id;";
            selectCmd.Parameters.AddWithValue("$id", screenElementId);

            using (var reader = selectCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    textId = reader["TextId"] as string;
                    imageId = reader["ImageId"] as string;
                }
                else
                {
                    // No such ScreenElement
                    return;
                }
            }

            // Delete the ScreenElement
            var deleteScreenElementCmd = connection.CreateCommand();
            deleteScreenElementCmd.CommandText = @"
                DELETE FROM ScreenElements WHERE Id = $id;";
            deleteScreenElementCmd.Parameters.AddWithValue("$id", screenElementId);
            deleteScreenElementCmd.ExecuteNonQuery();

            // Delete the Text if not referenced by any other ScreenElement
            if (!string.IsNullOrEmpty(textId))
            {
                var textRefCmd = connection.CreateCommand();
                textRefCmd.CommandText = @"
                    SELECT COUNT(*) FROM ScreenElements WHERE TextId = $textId;";
                textRefCmd.Parameters.AddWithValue("$textId", textId);
                long textRefCount = (long)textRefCmd.ExecuteScalar()!;
                if (textRefCount == 0)
                {
                    var deleteTextCmd = connection.CreateCommand();
                    deleteTextCmd.CommandText = @"DELETE FROM Texts WHERE Id = $id;";
                    deleteTextCmd.Parameters.AddWithValue("$id", textId);
                    deleteTextCmd.ExecuteNonQuery();
                }
            }

            // Delete the Image if not referenced by any other ScreenElement
            if (!string.IsNullOrEmpty(imageId))
            {
                var imageRefCmd = connection.CreateCommand();
                imageRefCmd.CommandText = @"
                    SELECT COUNT(*) FROM ScreenElements WHERE ImageId = $imageId;";
                imageRefCmd.Parameters.AddWithValue("$imageId", imageId);
                long imageRefCount = (long)imageRefCmd.ExecuteScalar()!;
                if (imageRefCount == 0)
                {
                    var deleteImageCmd = connection.CreateCommand();
                    deleteImageCmd.CommandText = @"DELETE FROM Images WHERE Id = $id;";
                    deleteImageCmd.Parameters.AddWithValue("$id", imageId);
                    deleteImageCmd.ExecuteNonQuery();
                }
            }

            transaction.Commit();
        }





    }
}
