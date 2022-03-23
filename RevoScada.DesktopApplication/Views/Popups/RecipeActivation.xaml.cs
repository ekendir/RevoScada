using DevExpress.Xpf.WindowsUI;
using RevoScada.DesktopApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    public partial class RecipeActivation : Window
    {
        public RecipeEditorVM _recipeEditorVM { get; set; }



        public RecipeActivation(RecipeEditorVM recipeEditorVM, bool isValidValue)
        {
            InitializeComponent();

            _recipeEditorVM = recipeEditorVM;

            if (isValidValue)
            {
                _recipeEditorVM.RecipeActiveValue = true;
                _recipeEditorVM.RecipeDeactiveValue = false;

                recipeActived.Tag = true;
                recipeDeactived.Tag = false;
            }
            else
            {
                _recipeEditorVM.RecipeDeactiveValue = true;
                _recipeEditorVM.RecipeActiveValue = false;

                recipeDeactived.Tag = true;
                recipeActived.Tag = false;
            }
        }



        private void recipeActived_Checked(object sender, RoutedEventArgs e)
        {

            if (_recipeEditorVM == null)
                return;

            bool result = _recipeEditorVM.UpdateRecipeValidValues(true);


            if (result == false)
            {
                WinUIMessageBox.Show("Reçete aktif edilemedi", "Başarısız",
                                                                            MessageBoxButton.OK, MessageBoxImage.Warning);
            }



        }

        private void recipeActived_Loaded(object sender, RoutedEventArgs e)
        {

            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag == null)
                return;

            if ((bool)radioButton.Tag)
                radioButton.IsChecked = true;
        }

        private void recipeDeactived_Checked(object sender, RoutedEventArgs e)
        {
            Recipe_Editor recipe_Editor = new Recipe_Editor();

            RadioButton radioButton = (RadioButton)sender;

            if (radioButton.Tag == null)
                return;

            if (_recipeEditorVM == null)
                return;

            bool result = _recipeEditorVM.UpdateRecipeValidValues(false);

            if (result == false)
            {
                WinUIMessageBox.Show("Reçete pasif edilemedi", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void recipeDeactived_Loaded(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag == null)
                return;

            if ((bool)radioButton.Tag)
                radioButton.IsChecked = true;
        }


        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            _recipeEditorVM.IsInitiallyLoaded = false;
            this.Close();
        }

    }
}
