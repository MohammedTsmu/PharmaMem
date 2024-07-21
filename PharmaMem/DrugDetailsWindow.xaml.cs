using System.Data.SQLite;
using System.Windows;

namespace PharmaMem
{
    public partial class DrugDetailsWindow : Window
    {
        private int drugId;
        private Database db;

        public DrugDetailsWindow(int id)
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
                GenericName.Text = reader["GenericName"].ToString();
                BrandName.Text = reader["BrandName"].ToString();
                Type.Text = reader["Type"].ToString();
                Dosage.Text = reader["Dosage"].ToString();
                Uses.Text = reader["Uses"].ToString();
                SideEffects.Text = reader["SideEffects"].ToString();
                Group.Text = reader["Group"].ToString();
                Category.Text = reader["Category"].ToString();
                Form.Text = reader["Form"].ToString();
                Family.Text = reader["Family"].ToString();
                Mechanism.Text = reader["Mechanism"].ToString();
                MainJob.Text = reader["MainJob"].ToString();
            }
        }

        private void SaveDrug_Click(object sender, RoutedEventArgs e)
        {
            string genericName = GenericName.Text;
            string brandName = BrandName.Text;
            string type = Type.Text;
            string dosage = Dosage.Text;
            string uses = Uses.Text;
            string sideEffects = SideEffects.Text;
            string group = Group.Text;
            string category = Category.Text;
            string form = Form.Text;
            string family = Family.Text;
            string mechanism = Mechanism.Text;
            string mainJob = MainJob.Text;

            SQLiteCommand command = new SQLiteCommand(@"
                UPDATE Drugs 
                SET GenericName = @GenericName, BrandName = @BrandName, Type = @Type, Dosage = @Dosage, Uses = @Uses, 
                    SideEffects = @SideEffects, Group = @Group, Category = @Category, Form = @Form, Family = @Family, 
                    Mechanism = @Mechanism, MainJob = @MainJob
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
            this.Close();
        }

        private void DeleteDrug_Click(object sender, RoutedEventArgs e)
        {
            SQLiteCommand command = new SQLiteCommand("DELETE FROM Drugs WHERE Id = @Id", db.Connection);
            command.Parameters.AddWithValue("@Id", drugId);
            command.ExecuteNonQuery();
            this.Close();
        }

        private void ManageImages_Click(object sender, RoutedEventArgs e)
        {
            ManageImagesWindow manageImagesWindow = new ManageImagesWindow(drugId);
            manageImagesWindow.ShowDialog();
        }
    }
}
