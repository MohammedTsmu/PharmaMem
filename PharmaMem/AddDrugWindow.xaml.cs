using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;

namespace PharmaMem
{
    public partial class AddDrugWindow : Window
    {
        private List<string> imagePaths = new List<string>();

        public AddDrugWindow()
        {
            InitializeComponent();
            ShowPlaceholder(GenericNameTextBox, "Generic Name");
            ShowPlaceholder(BrandNameTextBox, "Brand Name");
            ShowPlaceholder(TypeTextBox, "Type");
            ShowPlaceholder(DosageTextBox, "Dosage");
            ShowPlaceholder(UsesTextBox, "Uses");
            ShowPlaceholder(SideEffectsTextBox, "Side Effects");
            ShowPlaceholder(GroupTextBox, "Group");
            ShowPlaceholder(CategoryTextBox, "Category");
            ShowPlaceholder(FormTextBox, "Form");
            ShowPlaceholder(FamilyTextBox, "Family");
            ShowPlaceholder(MechanismTextBox, "Mechanism");
            ShowPlaceholder(MainJobTextBox, "Main Job");

            ShowPlaceholder(MaxDoseTextBox, "Max Dose");
            ShowPlaceholder(DrugInteractionsTextBox, "Drug Interactions");
            ShowPlaceholder(SpecialInstructionsTextBox, "Special Instructions");
            ShowPlaceholder(StorageConditionsTextBox, "Storage Conditions");
            ShowPlaceholder(ShelfLifeTextBox, "Shelf Life");
            ShowPlaceholder(PrecautionsTextBox, "Precautions");
            ShowPlaceholder(ContraindicationsTextBox, "Contraindications");
            ShowPlaceholder(ManufacturerTextBox, "Manufacturer");
            ShowPlaceholder(PriceTextBox, "Price");
            ShowPlaceholder(ProductCodeTextBox, "Product Code");
        }

        private void ShowPlaceholder(TextBox textBox, string placeholder)
        {
            textBox.Text = placeholder;
            textBox.Foreground = Brushes.Gray;

            textBox.GotFocus += (sender, args) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.Foreground = Brushes.Black;
                }
            };

            textBox.LostFocus += (sender, args) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.Foreground = Brushes.Gray;
                }
            };
        }

        private void AddImages_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png",
                Multiselect = true,
                Title = "Select Drug Images"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                imagePaths.AddRange(openFileDialog.FileNames);
                MessageBox.Show($"{openFileDialog.FileNames.Length} images added.", "Images Added", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveDrug_Click(object sender, RoutedEventArgs e)
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

            string maxDose = MaxDoseTextBox.Text;
            string drugInteractions = DrugInteractionsTextBox.Text;
            string specialInstructions = SpecialInstructionsTextBox.Text;
            string storageConditions = StorageConditionsTextBox.Text;
            string shelfLife = ShelfLifeTextBox.Text;
            string precautions = PrecautionsTextBox.Text;
            string contraindications = ContraindicationsTextBox.Text;
            string manufacturer = ManufacturerTextBox.Text;
            string price = PriceTextBox.Text;
            string productCode = ProductCodeTextBox.Text;

            Database db = new Database();
            SQLiteCommand command = new SQLiteCommand(@"
                INSERT INTO Drugs (GenericName, BrandName, Type, Dosage, Uses, SideEffects, `Group`, Category, Form, Family, Mechanism, MainJob, MaxDose, DrugInteractions, SpecialInstructions, StorageConditions, ShelfLife, Precautions, Contraindications, Manufacturer, Price, ProductCode) 
                VALUES (@GenericName, @BrandName, @Type, @Dosage, @Uses, @SideEffects, @Group, @Category, @Form, @Family, @Mechanism, @MainJob,  @MaxDose, @DrugInteractions, @SpecialInstructions, @StorageConditions, @ShelfLife, @Precautions, @Contraindications, @Manufacturer, @Price, @ProductCode)", db.Connection);

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

            command.Parameters.AddWithValue("@MaxDose", maxDose);
            command.Parameters.AddWithValue("@DrugInteractions", drugInteractions);
            command.Parameters.AddWithValue("@SpecialInstructions", specialInstructions);
            command.Parameters.AddWithValue("@StorageConditions", storageConditions);
            command.Parameters.AddWithValue("@ShelfLife", shelfLife);
            command.Parameters.AddWithValue("@Precautions", precautions);
            command.Parameters.AddWithValue("@Contraindications", contraindications);
            command.Parameters.AddWithValue("@Manufacturer", manufacturer);
            command.Parameters.AddWithValue("@Price", price);
            command.Parameters.AddWithValue("@ProductCode", productCode);

            command.ExecuteNonQuery();

            long drugId = db.Connection.LastInsertRowId;

            foreach (string imagePath in imagePaths)
            {
                SQLiteCommand imageCommand = new SQLiteCommand(@"
                    INSERT INTO DrugImages (DrugId, ImagePath) 
                    VALUES (@DrugId, @ImagePath)", db.Connection);

                imageCommand.Parameters.AddWithValue("@DrugId", drugId);
                imageCommand.Parameters.AddWithValue("@ImagePath", imagePath);

                imageCommand.ExecuteNonQuery();
            }

            MessageBox.Show("Drug saved successfully.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
    }
}
