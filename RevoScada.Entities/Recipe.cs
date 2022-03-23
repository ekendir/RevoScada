using System;
using Dapper.Contrib.Extensions;

namespace RevoScada.Entities
{
    [Table("public.\"Recipes\"")]
    public class Recipe
    {
        [ExplicitKey]
        public int id { get; set; }
        public string RecipeName { get; set; }
        public string Description { get; set; }
        public short RecipeGroupId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public DateTime LastRunDate { get; set; }
        public bool IsActive { get; set; }
        public int CreatedByUserId { get; set; }
        public int ModifiedByUserId { get; set; }
        public int DeletedByUserId { get; set; }
        public short RequirementValue { get; set; }
        public short RequirementTime { get; set; }
        public string Specification { get; set; }
        public bool IsValid { get; set; }

    }
}
