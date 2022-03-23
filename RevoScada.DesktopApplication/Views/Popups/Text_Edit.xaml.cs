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
    /// <summary>
    /// Interaction logic for Text_Edit.xaml
    /// </summary>
    public partial class Text_Edit : Window
    {
        #region Fields
        private Recipe_Editor Recipe_Editor;
        private bool _isItRecipe;
        private int _recipeId;
        #endregion

        #region Properties
        public string MainText { get; set; }
        #endregion

        public Text_Edit(Recipe_Editor recipeEditor, string recipeName, bool isItRecipe, int recipeId)
        {
            InitializeComponent();
            DataContext = this;

            LoadTitleTexts(isItRecipe);

            Recipe_Editor = recipeEditor;
            recipeNameBox.Text = recipeName;
            _isItRecipe = isItRecipe;
            _recipeId = recipeId;
        }

        private void LoadTitleTexts(bool isItRecipe)
        {
            if(isItRecipe)
            {
                MainText = "New recipe name:";
                this.Title = "Edit Recipe Name";
            } else
            {
                MainText = "New recipe group name:";
                this.Title = "Edit Recipe Group Name";
            }
        }

        private void applyBtn_Click(object sender, RoutedEventArgs e)
        {
            Recipe_Editor.ApplyNameChangesToRecipeItem(GetFixedRecipeName(recipeNameBox.Text), _isItRecipe, _recipeId);
        }

        private string GetFixedRecipeName(string recipe)
        {
            if (string.IsNullOrEmpty(recipe))
                return "New Recipe";

            if (recipe.Length > 200)
               return recipe.Substring(0, 200);

            return recipe;
        }

        private void recipeNameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                Recipe_Editor.ApplyNameChangesToRecipeItem(GetFixedRecipeName(recipeNameBox.Text), _isItRecipe, _recipeId);
        }

        private void recipeNameBox_Loaded(object sender, RoutedEventArgs e)
        {
            recipeNameBox.Focus();
            recipeNameBox.CaretIndex = recipeNameBox.Text.Length;
        }
    }
}