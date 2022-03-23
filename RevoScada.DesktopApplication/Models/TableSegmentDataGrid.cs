using RevoScada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class TableSegmentDataGrid
    {
        public string RecipeFieldValue { get; set; }
        public string RecipeFieldDisplayColor { get; set; }
        public bool IsMultipleCell { get; set; }
        public bool IsActive { get; set; }
        public short RecipeFieldId { get; set; }
        public short RecipeFieldOrderId { get; set; }
        public RecipeDetail RecipeDetail { get; set; }
        public CellChangeStates CellChangeState { get; set; } = CellChangeStates.Unchanged;
    }

    public enum CellChangeStates
    {
        Modified, Added, Deleted, Unchanged
    }
}
