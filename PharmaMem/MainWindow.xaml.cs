using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PharmaMem
{
    public partial class MainWindow : Window
    {
        private Database db;

        public MainWindow()
        {
            InitializeComponent();
            db = new Database();
            LoadDrugs();
        }

        private void LoadDrugs()
        {
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Drugs", db.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            List<Drug> drugs = new List<Drug>();

            while (reader.Read())
            {
                drugs.Add(new Drug
                {
                    Id = (long)reader["Id"],
                    GenericName = reader["GenericName"].ToString(),
                    BrandName = reader["BrandName"].ToString(),
                    Type = reader["Type"].ToString(),
                    Dosage = reader["Dosage"].ToString(),
                    Uses = reader["Uses"].ToString(),
                    SideEffects = reader["SideEffects"].ToString(),
                    Group = reader["Group"].ToString(),
                    Category = reader["Category"].ToString(),
                    Form = reader["Form"].ToString(),
                    Family = reader["Family"].ToString(),
                    Mechanism = reader["Mechanism"].ToString(),
                    MainJob = reader["MainJob"].ToString()
                });
            }

            DrugsList.ItemsSource = drugs;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();
            List<Drug> filteredDrugs = ((List<Drug>)DrugsList.ItemsSource).Where(d =>
                d.GenericName.ToLower().Contains(searchText) ||
                d.BrandName.ToLower().Contains(searchText) ||
                d.Type.ToLower().Contains(searchText) ||
                d.Category.ToLower().Contains(searchText)
            ).ToList();

            DrugsList.ItemsSource = filteredDrugs;
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            string selectedFilter = (FilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(selectedFilter) || selectedFilter == "All")
            {
                LoadDrugs();
                return;
            }

            string searchText = SearchTextBox.Text.ToLower();
            List<Drug> filteredDrugs = ((List<Drug>)DrugsList.ItemsSource).Where(d =>
            {
                switch (selectedFilter)
                {
                    case "Generic Name":
                        return d.GenericName.ToLower().Contains(searchText);
                    case "Brand Name":
                        return d.BrandName.ToLower().Contains(searchText);
                    case "Type":
                        return d.Type.ToLower().Contains(searchText);
                    case "Category":
                        return d.Category.ToLower().Contains(searchText);
                    default:
                        return false;
                }
            }).ToList();

            DrugsList.ItemsSource = filteredDrugs;
        }

        private void DrugsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DrugsList.SelectedItem is Drug selectedDrug)
            {
                ViewDrugDetailsWindow detailsWindow = new ViewDrugDetailsWindow((int)selectedDrug.Id);
                detailsWindow.ShowDialog();
            }
        }

        private void AddDrug_Click(object sender, RoutedEventArgs e)
        {
            AddDrugWindow addDrugWindow = new AddDrugWindow();
            addDrugWindow.ShowDialog();
            LoadDrugs(); // Refresh the list after adding a new drug
        }

        private void ImportFromCSV_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Import Drugs from CSV"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                using (var reader = new System.IO.StreamReader(openFileDialog.FileName))
                {
                    string line;
                    bool isFirstLine = true;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (isFirstLine)
                        {
                            isFirstLine = false; // Skip the header row
                            continue;
                        }

                        string[] fields = line.Split(',');

                        SQLiteCommand command = new SQLiteCommand(@"
                            INSERT INTO Drugs (GenericName, BrandName, Type, Dosage, Uses, SideEffects, `Group`, Category, Form, Family, Mechanism, MainJob) 
                            VALUES (@GenericName, @BrandName, @Type, @Dosage, @Uses, @SideEffects, @Group, @Category, @Form, @Family, @Mechanism, @MainJob)", db.Connection);

                        command.Parameters.AddWithValue("@GenericName", fields[1]);
                        command.Parameters.AddWithValue("@BrandName", fields[2]);
                        command.Parameters.AddWithValue("@Type", fields[3]);
                        command.Parameters.AddWithValue("@Dosage", fields[4]);
                        command.Parameters.AddWithValue("@Uses", fields[5]);
                        command.Parameters.AddWithValue("@SideEffects", fields[6]);
                        command.Parameters.AddWithValue("@Group", fields[7]);
                        command.Parameters.AddWithValue("@Category", fields[8]);
                        command.Parameters.AddWithValue("@Form", fields[9]);
                        command.Parameters.AddWithValue("@Family", fields[10]);
                        command.Parameters.AddWithValue("@Mechanism", fields[11]);
                        command.Parameters.AddWithValue("@MainJob", fields[12]);

                        command.ExecuteNonQuery();
                    }
                }

                LoadDrugs(); // Refresh the list after importing
            }
        }

        private void BackupDatabase_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "SQLite Database files (*.db)|*.db",
                Title = "Backup Database"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string databasePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "pharma.db");
                string backupPath = saveFileDialog.FileName;
                System.IO.File.Copy(databasePath, backupPath, true);
                MessageBox.Show("Database backup completed successfully.", "Backup", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RestoreDatabase_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "SQLite Database files (*.db)|*.db",
                Title = "Restore Database"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string databasePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "pharma.db");
                string backupPath = openFileDialog.FileName;
                System.IO.File.Copy(backupPath, databasePath, true);
                MessageBox.Show("Database restored successfully.", "Restore", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadDrugs(); // Refresh the list after restoring
            }
        }
    }

    public class Drug
    {
        public long Id { get; set; }
        public string GenericName { get; set; }
        public string BrandName { get; set; }
        public string Type { get; set; }
        public string Dosage { get; set; }
        public string Uses { get; set; }
        public string SideEffects { get; set; }
        public string Group { get; set; }
        public string Category { get; set; }
        public string Form { get; set; }
        public string Family { get; set; }
        public string Mechanism { get; set; }
        public string MainJob { get; set; }
    }
}
