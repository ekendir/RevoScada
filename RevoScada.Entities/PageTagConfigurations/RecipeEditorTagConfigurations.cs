using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.PageTagConfigurations
{
    public class RecipeEditorTagConfigurations
    {
        public short DbNumber { get; set; }
        public short IncrementAmount { get; set; }
        public int BufferSizeofRecipe { get; set; }
        public short Length { get; set; }
        public int RecipeLoadOk { get; set; }
        public int RecipeName { get; set; }
        public int LeakageTestFailureCriteriaTestValue { get; set; }
        public int LeakageTestFailureSetTime { get; set; }
    }
}