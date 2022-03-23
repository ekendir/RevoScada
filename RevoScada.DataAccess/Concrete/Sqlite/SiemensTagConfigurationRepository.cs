using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevoScada.DataAccess.Concrete.SqLite
{
    public class SiemensTagConfigurationRepository : DapperGenericRepository<SiemensTagConfiguration>
    {
        public SiemensTagConfigurationRepository(string connectionString) : base(connectionString)
        {

        }
        
        /*
        public IEnumerable<SiemensReadRequestItem> GetReadRequestList(int plcId)
        {
            IEnumerable<SiemensReadRequestItem> readRequestItems;

            DapperGenericRepository<SiemensReadRequestItem> dapperGenericRepository = new DapperGenericRepository<SiemensReadRequestItem>(ConnectionString);

            try
            {
                readRequestItems = dapperGenericRepository.GetAllBySqlQuery($"SELECT DBNumber, max(offset) MaxOffset  FROM  SiemensTagConfigurations WHERE plcId={plcId} GROUP BY DBNumber");

                foreach (SiemensReadRequestItem readRequestItem  in readRequestItems)
                {
                    readRequestItem.MaxOffset = (readRequestItem.MaxOffset + 255) % 65534;
                }

                return readRequestItems;

            }
            catch (Exception exception)
            {
                
                exception.Data.Add("DetailMessage", "Repository:GetReadRequestList");

                throw exception;

            }
        } 
        */

        public IEnumerable<SiemensReadRequestItem> ReadRequestList(int plcId)
        {
            IEnumerable<SiemensReadRequestItem> readRequestItems;

            DapperGenericRepository<SiemensReadRequestItem> dapperGenericRepository = new DapperGenericRepository<SiemensReadRequestItem>(ConnectionString);

            try
            {
                readRequestItems = dapperGenericRepository.GetAllBySqlQuery($"SELECT Id, Offset as ComputedSize, DBNumber, DataType FROM SiemensTagConfigurations WHERE plcid={plcId} AND IsActive=1");
                return readRequestItems;

            }
            catch (Exception exception)
            {
                exception.Data.Add("DetailMessage", "Repository:GetReadRequestList");
                throw exception;
            }

        }

        public IEnumerable<OnDemandDataBlock> OnDemands(int plcId)
        {
            IEnumerable<OnDemandDataBlock> onDemandItems;

            DapperGenericRepository<OnDemandDataBlock> dapperGenericRepository = new DapperGenericRepository<OnDemandDataBlock>(ConnectionString);

            try
            {
                onDemandItems = dapperGenericRepository.GetAll().ToList().Where(x=>x.PlcId==plcId);

                return onDemandItems;

            }
            catch (Exception exception)
            {

                exception.Data.Add("DetailMessage", "Repository:OnDemandList");

                throw exception;

            }
        }
    }
}

