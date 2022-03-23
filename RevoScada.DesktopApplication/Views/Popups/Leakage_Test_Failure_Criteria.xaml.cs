using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.ViewModels;
using RevoScada.ProcessController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RevoScada.DesktopApplication.Views.Popups
{
    /// <summary>
    /// Interaction logic for Leakage_Test_Failure_Criteria.xaml
    /// </summary>
    public partial class Leakage_Test_Failure_Criteria : Window
    {
        #region Properties
        public RecipeEditorVM _recipeEditorVM { get; set; }
        public string VacuumUnitTitle { get; set; }
        public string IntegrityCheckTimeFormatTitle { get; set; }
        #endregion

        public Leakage_Test_Failure_Criteria(RecipeEditorVM recipeEditorVM, short currRequirementVal, short currRequirementTime)
        {
            InitializeComponent();
            DataContext = this;

            _recipeEditorVM = recipeEditorVM;
            VacuumUnitTitle = ProcessManager.Instance.ApplicationProperties["VacuumUnitTitle"].Value;
            IntegrityCheckTimeFormatTitle = ProcessManager.Instance.ApplicationProperties["IntegrityCheckTimeFormatTitle"].Value;
            testValTextBox.Text = currRequirementVal.ToString();
            manualTimeTextBox.Text = currRequirementTime.ToString();
            testValTextBox.Focus();
            testValTextBox.CaretIndex = testValTextBox.Text.Length;
        }

        private void testValTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var change = e.Changes.FirstOrDefault();

            TextBox txtBox = (TextBox)sender;

            Regex regex = new Regex(@"^[0-9]{0,4}$");

            // Step 2: call Match on Regex instance.
            Match match = regex.Match(txtBox.Text);

            // Step 3: test for Success.
            if (!match.Success)
            {
                txtBox.Text = txtBox.Text.Remove(change.Offset, change.AddedLength);
            }
        }

        private void SetPresetTimeValueToTextbox(int value)
        {
            manualTimeTextBox.Text = value.ToString();
        }

        private void preset1MRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            SetPresetTimeValueToTextbox(1);
        }

        private void preset5MRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            SetPresetTimeValueToTextbox(5);
        }

        private void preset10MRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            SetPresetTimeValueToTextbox(10);
        }

        private void preset15MRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            SetPresetTimeValueToTextbox(15);
        }

        private void manualTimeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var change = e.Changes.FirstOrDefault();

            TextBox txtBox = (TextBox)sender;

            Regex regex = new Regex(@"^[,.0-9-]{0,15}$");

            //Regex regex = new Regex(@"^[0-9]{1,2}$");

            // Step 2: call Match on Regex instance.
            Match match = regex.Match(txtBox.Text);

            // Step 3: test for Success.
            if (!match.Success)
            {
                txtBox.Text = txtBox.Text.Remove(change.Offset, change.AddedLength);
            }
        }

        private void applyBtn_Click(object sender, RoutedEventArgs e)
        {
            short requirementVal = Convert.ToInt16(testValTextBox.Text);
            short requirementTime = Convert.ToInt16(manualTimeTextBox.Text);
            _recipeEditorVM.UpdateRecipeRequirementValues(requirementVal, requirementTime);
            this.Close();
        }
    }
}