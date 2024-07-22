using System;
using System.Data.SQLite;
using System.IO;

namespace PharmaMem
{
    public class Database
    {
        public SQLiteConnection Connection { get; private set; }

        public Database()
        {
            string databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "pharma.db");
            if (!Directory.Exists(Path.GetDirectoryName(databasePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(databasePath));
            }

            Connection = new SQLiteConnection($"Data Source={databasePath};Version=3;");
            Connection.Open();
            CreateTables();
        }

        public void CreateTables()
        {
            string createDrugsTable = @"
                CREATE TABLE IF NOT EXISTS Drugs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    GenericName TEXT NOT NULL,
                    BrandName TEXT,
                    Type TEXT,
                    Dosage TEXT,
                    Uses TEXT,
                    SideEffects TEXT,
                    `Group` TEXT,
                    Category TEXT,
                    Form TEXT,
                    Family TEXT,
                    Mechanism TEXT,
                    MainJob TEXT,
                    MaxDose TEXT,
                    DrugInteractions TEXT,
                    SpecialInstructions TEXT,
                    StorageConditions TEXT,
                    ShelfLife TEXT,
                    Precautions TEXT,
                    Contraindications TEXT,
                    Manufacturer TEXT,
                    Price TEXT,
                    ProductCode TEXT
                )";

            string createDrugImagesTable = @"
                CREATE TABLE IF NOT EXISTS DrugImages (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    DrugId INTEGER,
                    ImagePath TEXT,
                    FOREIGN KEY (DrugId) REFERENCES Drugs(Id) ON DELETE CASCADE
                )";

            SQLiteCommand command = new SQLiteCommand(createDrugsTable, Connection);
            command.ExecuteNonQuery();

            command = new SQLiteCommand(createDrugImagesTable, Connection);
            command.ExecuteNonQuery();
        }
    }
}
