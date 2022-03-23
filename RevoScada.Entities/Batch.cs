using System;
using Dapper.Contrib.Extensions;
using RevoScada.Entities.Enums;
namespace RevoScada.Entities
{
    [Table("public.\"Batches\"")]
    public class Batch
    {
        //[Key]
        [ExplicitKey]
        public int id { get; set; }
        public string LoadNumber { get; set; }
        [Computed]
        public string LoadNumberFormatted { get { return Revision > 0 ? $"{LoadNumber}-R{Revision}" : LoadNumber; } }
        public int RecipeId {get;set;}
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BatchCurrentState Status { get; set; }
        public short BatchGroupId { get; set; }
        public short Revision { get; set; }
        public bool IsEnterPartsSkip { get; set; }
        public int CreatedByUserId { get; set; }
        public int ModifiedByUserId { get; set; }
    }
}
