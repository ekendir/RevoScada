using RevoScada.Entities;

namespace RevoScada.DataAccess.Concrete.Postgresql
{
    public class DataLogRepository : DapperGenericPostgreRepository<DataLog>
    {
        public DataLogRepository(string connectionString) : base(connectionString)
        {
        }
    }
}

// Dapper.SqlMapper.SetTypeMap(
// typeof(DataLog),
// new CustomPropertyTypeMap(
// typeof(DataLog),
// (type, columnName) =>
// type.GetProperties().FirstOrDefault(prop =>
// prop.GetCustomAttributes(false)
// .OfType<System.ComponentModel.DataAnnotations.Schema.ColumnAttribute>()
// .Any(attr => attr.Name == columnName)))
//);
// DataLogMapper dataLogMapper = new DataLogMapper();
    //public class DataLogMapper : ClassMapper<DataLog>
    //{
    //    public DataLogMapper()
    //    {

    //        Table("DataLogs");
    //        Map(m => m.Id)..Ignore();
    //        AutoMap();
    //    }
    //}