using System.Data.SQLite;
using System.Windows;

namespace PharmaMem
{
    public partial class EditDrugWindow : Window
    {
        private int drugId;
        private Database db;

        public EditDrugWindow(int id)
        {
            InitializeComponent();
            drugId = id;
            db = new Database();
            LoadDrugDetails();
        }

        private void LoadDrugDetails()
        {
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Drugs WHERE Id = @Id", db.Connection);
            command.Parameters.AddWithValue("@Id", drugId);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                GenericNameTextBox.Text = reader["GenericName"].ToString();
                BrandNameTextBox.Text = reader["BrandName"].ToString();
                TypeTextBox.Text = reader["Type"].ToString();
                DosageTextBox.Text = reader["Dosage"].ToString();
                UsesTextBox.Text = reader["Uses"].ToString();
                SideEffectsTextBox.Text = reader["SideEffects"].ToString();
                GroupTextBox.Text = reader["Group"].ToString();
                CategoryTextBox.Text = reader["Category"].ToString();
                FormTextBox.Text = reader["Form"].ToString();
                FamilyTextBox.Text = reader["Family"].ToString();
                MechanismTextBox.Text = reader["Mechanism"].ToString();
                MainJobTextBox.Text = reader["MainJob"].ToString();
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            string genericName = GenericNameTextBox.Text;
            string brandName = BrandNameTextBox.Text;
            string type = TypeTextBox.Text;
            string dosage = DosageTextBox.Text;
            string uses = UsesTextBox.Text;
            string sideEffects = SideEffectsTextBox.Text;
            string group = GroupTextBox.Text;
            string category = CategoryTextBox.Text;
            string form = FormTextBox.Text;
            string family = FamilyTextBox.Text;
            string mechanism = MechanismTextBox.Text;
            string mainJob = MainJobTextBox.Text;

            SQLiteCommand command = new SQLiteCommand(@"
                UPDATE Drugs SET 
                    GenericName = @GenericName, 
                    BrandName = @BrandName, 
                    Type = @Type, 
                    Dosage = @Dosage, 
                    Uses = @Uses, 
                    SideEffects = @SideEffects, 
                    `Group` = @Group, 
                    Category = @Category, 
                    Form = @Form, 
                    Family = @Family, 
                    Mechanism = @Mechanism, 
                    MainJob = @MainJob
                WHERE Id = @Id", db.Connection);

            command.Parameters.AddWithValue("@GenericName", genericName);
            command.Parameters.AddWithValue("@BrandName", brandName);
            command.Parameters.AddWithValue("@Type", type);
            command.Parameters.AddWithValue("@Dosage", dosage);
            command.Parameters.AddWithValue("@Uses", uses);
            command.Parameters.AddWithValue("@SideEffects", sideEffects);
            command.Parameters.AddWithValue("@Group", group);
            command.Parameters.AddWithValue("@Category", category);
            command.Parameters.AddWithValue("@Form", form);
            command.Parameters.AddWithValue("@Family", family);
            command.Parameters.AddWithValue("@Mechanism", mechanism);
            command.Parameters.AddWithValue("@MainJob", mainJob);
            command.Parameters.AddWithValue("@Id", drugId);

            command.ExecuteNonQuery();

            MessageBox.Show("Changes saved successfully.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
    }
}
