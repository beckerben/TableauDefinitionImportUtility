using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace TableauDefinitionImportUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Common.logger("Tableau Definition Import Utility Started", txtLog);
        }

        private void btnTableauBrowse_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".tbw"; // Default file extension
            dlg.Filter = "Tableau Workbook (.twb)|*.twb"; // Filter files by extension
            dlg.Title = "Tableau Workbook";

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                this.txtTableauWB.Text = dlg.FileName;

            }

        }

        private void btnDefinitions_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.FileName = "Tableau Workbook"; // Default file name
            dlg.DefaultExt = ".csv"; // Default file extension
            dlg.Filter = "Definition CSV (*.csv, *.txt)|*.csv;*.txt"; // Filter files by extension
            dlg.Title = "Definition CSV File";                

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                this.txtDefinitionCSV.Text = dlg.FileName;

            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.txtLog.Clear();
            if (Validate())
            {
                try
                {
                    ParseTableau();
                }
                catch (Exception ex)
                {
                    Common.logger("Something terrible happened: " + ex.Message.ToString(),txtLog);
                }
            }
            else
            {
                MessageBox.Show("Backup cannot start, ensure all fields are populated", "Backup Not Started", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            
        }

        void ParseTableau()
        {

            Common.logger("All required fields populated, beginning import process", txtLog);
            bool blnOverwrite = this.cbOverwriteDefinitions.IsChecked == true;
            string strSeparator = this.txtSeparator.Text;
            string strEnclosureBegin = this.txtEnclosureBegin.Text;
            string strEnclosureEnd = this.txtEnclosureEnd.Text;
            string wb = this.txtTableauWB.Text;
            string def = this.txtDefinitionCSV.Text;
            int intCntDefinitionMatch = 0;
            int intCntDefinitionUpdated = 0;

            //Load the tableau workbook into an xmldocument
            XDocument xdoc = XDocument.Load(wb);

            //Run query to get the columns in the Tableau workbook
            var cols = (from lv1 in xdoc.Descendants("datasource").Descendants("column")
                        select lv1).ToList();

            Common.logger("Tableau file read, found " + cols.Count() + " columns to check for definitions", txtLog);

            //Loop through columns in the tableau workbook
            foreach (var lv1 in cols)
            {
                //get the column name and normalize it (i.e. for duplicated columns that are aliased			
                string strOriginalCol = lv1.Attribute("name").Value;
                string strNormalizedCol = NormalizeColumn(lv1.Attribute("name").Value);

                Console.WriteLine(strOriginalCol);

                //loop through the definitions for this column name and try to find a match
                string[] defs = File.ReadAllLines(def);
                var querydef = from line in defs
                               let data = line.Split(Convert.ToChar(strSeparator))
                               where data[0].ToUpper() == strNormalizedCol.ToUpper().Replace(strEnclosureBegin, "[").Replace(strEnclosureEnd, "]")
                               select new { Def = data[1] };
                string definition = null;
                foreach (var d in querydef)
                {
                    definition = d.Def;
                }

                //if we found a definition lets use it now
                if (definition != null)
                {
                    intCntDefinitionMatch++;
                    //if match found then see if definition already exists
                    //if a definition exists and the overwrite is true, then update
                    //else if a definition does not exist, then create
                    definition = definition.Replace(strEnclosureBegin, "").Replace(strEnclosureEnd, "");

                    bool definition_exists = false;
                    foreach (var lv2 in lv1.Descendants("run"))
                    {
                        definition_exists = true;
                        //if a definition exists and overwrite is true then update
                        if (blnOverwrite && lv2 != null)
                        {
                            lv2.Value = definition;
                            intCntDefinitionUpdated++;
                        }
                    }

                    if (!definition_exists)
                    {
                        //we have a definition but one does not exist, create the elements for it
                        lv1.Add(new XElement("desc", new XElement("formatted-text", new XElement("run", definition))));
                        intCntDefinitionUpdated++;
                    }
                }
            }

            xdoc.Save(wb + ".import");
            Common.logger("Found a matching definition for " + intCntDefinitionMatch.ToString() + " columns",txtLog);
            Common.logger("Of the matches found, updated the definition for " + intCntDefinitionUpdated.ToString() + " columns",txtLog);
            Common.logger("Import completed, updated tableau workbook can be found here: ", txtLog);
            Common.logger(wb + ".import", txtLog);
        }

        private string NormalizeColumn(string col)
        {
            string strNormal = col;
            if (col.Contains("(") && col.Contains(")"))
            {
                strNormal = col.Substring(0, col.IndexOf("(") - 1) + "]";
            }
            return strNormal;
        }
        
        /// <summary>
        /// Verifies that the process can proceed and all fields needed are populated
        /// </summary>
        /// <returns>boolean</returns>
        private bool Validate()
        {
            if (this.txtDefinitionCSV.Text != "" && this.txtTableauWB.Text != "" && this.txtSeparator.Text != "" && this.txtEnclosureBegin.Text != "" && this.txtEnclosureEnd.Text != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
