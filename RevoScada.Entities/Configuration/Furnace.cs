using System;
using Dapper.Contrib.Extensions;

namespace RevoScada.Entities.Configuration
{
    public class Furnace
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string FurnaceName { get; set; }
        public short FurnaceTypeId { get; set; }
        public string Description { get; set; }
        public int FurnaceSoftwareId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public string ImagePath { get; set; }
    }
}
