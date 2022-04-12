using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System;

namespace SeaProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables
        public List<FAS_Element> allFAS_List = new List<FAS_Element>();
        string downloadDatabaseRequest =
                "SELECT mainentrystructureid, " +
                        "parententryid, " +
                        "prj_entry.entryid, " +
                        "entryclassid, " +
                        "productid, " +
                        "materialid, " +
                        "departmentid, " +
                        "name, " +
                        "cname, " +
                        "classcode, " +
                        "shipnumber, " +
                        "decimalcode, " +
                        "maindocument, " +
                        "extrainfo, " +
                        "normresource " +
                "FROM lnk_mainentrystructure " +
                "RIGHT JOIN prj_entry " +
                "ON lnk_mainentrystructure.entryid=prj_entry.entryid;";

        string connection = @"Server=localhost;Port=5432;User Id=postgres;Password=3942;Database=CDB_Seamatica_Desktop;";

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            UpdateDatabase();
            CreateTreeView();
        }
        public void UpdateDatabase() 
        { 
            allFAS_List.Clear();
            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(downloadDatabaseRequest, conn))
                {
                    NpgsqlDataReader? reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        allFAS_List.Add(new FAS_Element
                        {
                            Mainentrystructureid    = SafeGetString(reader, 0),
                            Parententryid           = SafeGetString(reader, 1),
                            Entryid                 = SafeGetString(reader, 2),
                            Entryclassid            = SafeGetString(reader, 3),
                            Productid               = SafeGetString(reader, 4),
                            Materialid              = SafeGetString(reader, 5),
                            Departmentid            = SafeGetString(reader, 6),
                            Name                    = SafeGetString(reader, 7),
                            Cname                   = SafeGetString(reader, 8),
                            Classcode               = SafeGetString(reader, 9),
                            Shipnumber              = SafeGetString(reader, 10),
                            Decimalcode             = SafeGetString(reader, 11),
                            Maindocument            = SafeGetString(reader, 12),
                            Extrainfo               = SafeGetString(reader, 13),
                            Normresource            = SafeGetString(reader, 14)
                        });
                    }
                }
                conn.Close();
            }
        }
        private static string SafeGetString(NpgsqlDataReader reader, int colIndex)
        {
            return reader.IsDBNull(colIndex) ? string.Empty : reader.GetString(colIndex);
        }

        List<int> list = new();
        private void UpdateRecord()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connection))
            {
                conn.Open();
                int i = 0;
                foreach (var record in allFAS_List)
                {
                    string updateNoteRequest = $"UPDATE prj_entry SET name='{record.Name}', cname = '{record.Cname}', extrainfo = '{record.Extrainfo}' WHERE entryid = '{record.Entryid}';";
                    using (var cmd = new NpgsqlCommand(updateNoteRequest, conn))
                    {
                        
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при записи данных элемента {record.Name}\n{ex.Message}", "Ошибка записи данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                            i++;
                        }
                    }
                }
                MessageBox.Show($"Записи успешно сохранены\nОшибок во время записи: {i}", "Сохранено", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void CreateTreeView()
        {
            foreach (var funk in allFAS_List)
            {
                var oc = new ObservableCollection<FAS_Element>();
                allFAS_List.Where(x => funk.Entryid == x.Parententryid).ToList().ForEach(oc.Add);
                funk.FAS_ElementList = oc;
            }
            var temp = new ObservableCollection<FAS_Element>();
            allFAS_List.Where(x => x.Parententryid == "").ToList().ForEach(temp.Add);
            Tree.ItemsSource = temp;
            DataContext = temp[0];
        }
        private void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Tree.Items.Count == 0) return;
            DataContext = Tree.SelectedItem;
        }
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Внимание: все несохраненные изменения будут утеряны. Продолжить? ", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                UpdateDatabase();
                CreateTreeView();
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateRecord();
        }
    }
}
