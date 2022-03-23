using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RevoScada.DataAccess.Abstract;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;

namespace RevoScada.Business.Configurations
{
    public class SiemensTagConfigurationService 
    {
        private string _connectionString { get; set; }

        private readonly IGenericRepository<SiemensTagConfiguration> _repository;

        public SiemensTagConfigurationService(string connectionString)
        {
            _connectionString = connectionString;
            _repository = new DataAccess.Concrete.SqLite.SiemensTagConfigurationRepository(_connectionString);
        }

        public IEnumerable<SiemensTagConfiguration> GetAll(Expression<Func<SiemensTagConfiguration, bool>> filter = null)
        {
            List<SiemensTagConfiguration> siemensTagConfigurations = _repository.GetAll().ToList();
            foreach (var item in siemensTagConfigurations)
            {
                try
                {
                    item.DataType = item.DataType.ToLower();
                    item.DataType = item.DataType == "bit" ? "bool" : item.DataType;
                    item.DataType = (item.DataType == "int" || item.DataType == "ınt") ? "int" : item.DataType;
                    item.DataType = (item.DataType == "Real") ? "real" : item.DataType;
                    item.DataType = (item.DataType == "udint" || item.DataType == "udınt") ? "udint" : item.DataType;
                }
                catch
                {
                }
            }
            return siemensTagConfigurations;
        }

        public IEnumerable<SiemensTagConfiguration> GetAllBySqlQuery(string queryText)
        {
            return _repository.GetAllBySqlQuery(queryText);
        }

        public SiemensTagConfiguration GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool Insert(SiemensTagConfiguration entity)
        {
            return _repository.Insert(entity);
        }

        public bool Update(SiemensTagConfiguration entity)
        {
            return _repository.Update( entity);
        }

        public IEnumerable<SiemensReadRequestItem> ReadRequestItems(int plcId)
        {
            var siemensTagConfigurationRepository = new DataAccess.Concrete.SqLite.SiemensTagConfigurationRepository(_connectionString);

            var readRequestItems = siemensTagConfigurationRepository.ReadRequestList(plcId);

            List<SiemensReadRequestItem> filteredSiemensReadRequestItems = readRequestItems.GroupBy(x => x.DbNumber, (key, g) => g.OrderByDescending(e => e.ComputedSize).First()).ToList();

            foreach (SiemensReadRequestItem siemensReadRequestItem in filteredSiemensReadRequestItems)
            {
              
                try
                {
                    int dataSize = 0;

                    siemensReadRequestItem.DataType = siemensReadRequestItem.DataType.ToLower();
                    siemensReadRequestItem.DataType = siemensReadRequestItem.DataType == "bit" ? "bool" : siemensReadRequestItem.DataType;
                    siemensReadRequestItem.DataType = (siemensReadRequestItem.DataType == "int" || siemensReadRequestItem.DataType == "ınt") ? "int" : siemensReadRequestItem.DataType;
                    siemensReadRequestItem.DataType = (siemensReadRequestItem.DataType == "Real") ? "real" : siemensReadRequestItem.DataType;
                    siemensReadRequestItem.DataType = (siemensReadRequestItem.DataType == "udint" || siemensReadRequestItem.DataType == "udınt") ? "udint" : siemensReadRequestItem.DataType;
                    siemensReadRequestItem.DataType = (siemensReadRequestItem.DataType == "strıng") ? "string" : siemensReadRequestItem.DataType;

                    switch (siemensReadRequestItem.DataType)
                    {
                        case "bool": dataSize = 1; break;
                        case "byte": dataSize = 1; break;
                        case "int": dataSize = 2; break;
                        case "dword": dataSize = 4; break;
                        case "real": dataSize = 4; break;
                        case "udint": dataSize = 4; break;
                    }

                    if (siemensReadRequestItem.DataType.Contains("string"))
                    {
                        dataSize = Convert.ToByte(System.Text.RegularExpressions.Regex.Match(siemensReadRequestItem.DataType, @"\d+").ToString())+2;
                        siemensReadRequestItem.DataType = "string";
                    }

                    siemensReadRequestItem.ComputedSize = siemensReadRequestItem.ComputedSize + dataSize;

                }
                catch 
                {

                }

            }



            var onDemandItems = siemensTagConfigurationRepository.OnDemands(plcId);

            foreach (var item in filteredSiemensReadRequestItems)
            {
                bool isDemanded = (onDemandItems.Where(x => x.DBNumber == item.DbNumber).Count()) == 1 ? false : true;

                item.IsDemanded = isDemanded;
            }








            return filteredSiemensReadRequestItems;
        }

        public Dictionary<int, List<SiemensReadRequestItem>> MultiplePlcReadRequestItems(int[] plcIds)
        {
            Dictionary<int, List<SiemensReadRequestItem>> filteredSiemensReadRequestItemsList = new Dictionary<int, List<SiemensReadRequestItem>>();

            foreach (int plcId in plcIds)
            {
                var siemensTagConfigurationRepository = new DataAccess.Concrete.SqLite.SiemensTagConfigurationRepository(_connectionString);

                var readRequestItems = siemensTagConfigurationRepository.ReadRequestList(plcId);

                List<SiemensReadRequestItem> filteredSiemensReadRequestItems = readRequestItems.GroupBy(x => x.DbNumber, (key, g) => g.OrderByDescending(e => e.ComputedSize).First()).ToList();

                foreach (SiemensReadRequestItem siemensReadRequestItem in filteredSiemensReadRequestItems)
                {

                    try
                    {
                        int dataSize = 0;

                        siemensReadRequestItem.DataType = siemensReadRequestItem.DataType.ToLower();
                        siemensReadRequestItem.DataType = siemensReadRequestItem.DataType == "bit" ? "bool" : siemensReadRequestItem.DataType;
                        siemensReadRequestItem.DataType = (siemensReadRequestItem.DataType == "int" || siemensReadRequestItem.DataType == "ınt") ? "int" : siemensReadRequestItem.DataType;
                        siemensReadRequestItem.DataType = (siemensReadRequestItem.DataType == "Real") ? "real" : siemensReadRequestItem.DataType;
                        siemensReadRequestItem.DataType = (siemensReadRequestItem.DataType == "udint" || siemensReadRequestItem.DataType == "udınt") ? "udint" : siemensReadRequestItem.DataType;
                        siemensReadRequestItem.DataType = (siemensReadRequestItem.DataType == "strıng") ? "string" : siemensReadRequestItem.DataType;

                        switch (siemensReadRequestItem.DataType)
                        {
                            case "bool": dataSize = 1; break;
                            case "byte": dataSize = 1; break;
                            case "int": dataSize = 2; break;
                            case "dword": dataSize = 4; break;
                            case "real": dataSize = 4; break;
                            case "udint": dataSize = 4; break;
                        }

                        if (siemensReadRequestItem.DataType.Contains("string"))
                        {
                            dataSize = Convert.ToByte(System.Text.RegularExpressions.Regex.Match(siemensReadRequestItem.DataType, @"\d+").ToString()) + 2;
                            siemensReadRequestItem.DataType = "string";
                        }

                        siemensReadRequestItem.ComputedSize = siemensReadRequestItem.ComputedSize + dataSize;

                    }
                    catch
                    {

                    }

                }


                var onDemandItems = siemensTagConfigurationRepository.OnDemands(plcId);

                foreach (var item in filteredSiemensReadRequestItems)
                {
                    bool isDemanded = (onDemandItems.Where(x => x.DBNumber == item.DbNumber).Count()) == 1 ? false : true;

                    item.IsDemanded = isDemanded;
                }


                filteredSiemensReadRequestItemsList.Add(plcId,filteredSiemensReadRequestItems);
            }

            return filteredSiemensReadRequestItemsList;
        }
    }
}
