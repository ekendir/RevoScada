using RevoScada.DataAccess.Abstract;
using RevoScada.Entities.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Business.Configurations
{
    public class SiemensPlcConfigService
    {

        private string _connectionString { get; set; }

        private readonly IGenericRepository<SiemensPlcConfig> _repository;

        public SiemensPlcConfigService(string connectionString)
        {
            _connectionString = connectionString;
            _repository = new DataAccess.Concrete.SqLite.SiemensPlcConfigRepository(_connectionString);
        }

        public IEnumerable<SiemensPlcConfig> GetAll(Expression<Func<SiemensPlcConfig, bool>> filter = null)
        {
            return _repository.GetAll();
        }

        public IEnumerable<SiemensPlcConfig> GetActiveFurnaceConfigurations()
        {
           FurnaceService furnaceService = new FurnaceService(_connectionString);
           PlcDeviceService plcDeviceService = new PlcDeviceService(_connectionString);
            var activeFurnaces = from f in furnaceService.GetAll()
                                 join pd in plcDeviceService.GetAll() on f.Id equals pd.FurnaceId
                                 join pc in _repository.GetAll() on f.Id equals pc.PlcDeviceId
                                 where f.IsActive == true
                     select new SiemensPlcConfig
                     {
                         Ip = pc.Ip,
                         PlcDeviceId = pc.PlcDeviceId,
                         Rack = pc.Rack,
                         Slot = pc.Slot,
                         ModifiedDate = pc.ModifiedDate
                     };
        
            return activeFurnaces.Distinct();
        }

        public IEnumerable<SiemensPlcConfig> GetAllBySqlQuery(string queryText)
        {
            return _repository.GetAllBySqlQuery(queryText);
        }

        public SiemensPlcConfig GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool Insert(SiemensPlcConfig entity)
        {
            return _repository.Insert(entity);
        }

        public bool Update(SiemensPlcConfig entity)
        {
            return _repository.Update(entity);
        }


    }
}
