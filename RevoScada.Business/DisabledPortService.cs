using RevoScada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevoScada.Business
{
    public class DisabledPortService : GenericService<DisabledPort>
    {
       
        public DisabledPortService(string connectionString) : base(connectionString)
        {
            _repository = new DataAccess.Concrete.Postgresql.DisabledPortRepository(_connectionString);
        }

        public IEnumerable<DisabledPort> GetAllBySqlQuery(string queryText)
        {
            return _repository.GetAllBySqlQuery(queryText);
        }

        public DisabledPort GetById(long id)
        {
            return _repository.GetById(id);
        }

        public IEnumerable<DisabledPort> GetByBatch(int batchId)
        {
            return _repository.GetAllBySqlQuery($"SELECT * FROM  public.\"DisabledPorts\" WHERE  \"BatchId\" = {batchId} ");
        }

        /// <summary>
        /// Filters by batch and grouped by uniq received Date
        /// </summary>
        public Dictionary<DateTime,int[]> GetByBatchGroupedByReceivedDate(int batchId)
        {
            var disabledPorts = _repository.GetAllBySqlQuery($"SELECT * FROM  public.\"DisabledPorts\" WHERE  \"BatchId\" = {batchId} ");
            Dictionary<DateTime, int[]> groupedByDateList = new Dictionary<DateTime, int[]>();
            foreach (var item in disabledPorts)
            {
                int[] portList = item.TagConfigurationList.Select(x => x).ToArray();
                groupedByDateList.Add(item.ReceivedDate, portList);
            }
            return groupedByDateList;
        }

        /// <summary>
        /// Filters by batch and grouped by uniq received Date
        /// </summary>
        public Dictionary<DateTime, List<string>> GetByBatchGroupedByReceivedDateWithPortName(int batchId)
        {
            ActiveTagService activeTagService = new ActiveTagService(_connectionString);
            var activeTags = activeTagService.ActiveTagsByTagIdKey();

           // var disabledPorts = _repository.GetAllBySqlQuery($"SELECT * FROM  public.\"DisabledPorts\" WHERE  \"BatchId\" = {batchId} ORDER BY \"ReceivedDate\"");
            var disabledPorts = _repository.GetAllBySqlQuery($"SELECT \"BatchId\",\"ReceivedDate\",\"TagConfigurationList\" FROM  public.\"DisabledPorts\" WHERE  \"BatchId\" = {batchId} GROUP BY \"BatchId\",\"ReceivedDate\",\"TagConfigurationList\" ORDER BY \"ReceivedDate\"");
            Dictionary<DateTime, List<string>> groupedByDateList = new Dictionary<DateTime, List<string>>();
            foreach (var item in disabledPorts)
            {
                int[] portList = item.TagConfigurationList.Select(x => x).ToArray();

                List<string> portNameList = new List<string>();

                foreach (var port in portList)
                {
                    ActiveTag activeTag = activeTags[port];
                    portNameList.Add(activeTag.TagName);
                }

                groupedByDateList.Add(item.ReceivedDate, portNameList);
            }
            return groupedByDateList;
        }

        /// <summary>
        /// returns port list according to batch id and receiveddate
        /// </summary>
        public IEnumerable<int> GetPortListByBatchAndDate(int batchId, DateTime receivedDate)
        {
            var disabledPorts = _repository.GetAllBySqlQuery($"SELECT * FROM public.\"DisabledPorts\" WHERE  \"BatchId\" = {batchId} AND \"ReceivedDate\"= '{ receivedDate:yyyy-MM-dd HH:mm:ss}';"); ;
            return  disabledPorts.SelectMany(x => x.TagConfigurationList).ToArray();
        }

        /// <summary>
        /// returns port name list according to batch id and receiveddate
        /// </summary>
        public List<string> GetPortListByBatchAndDateWithPortName(int batchId, DateTime receivedDate)
        {
            ActiveTagService activeTagService = new ActiveTagService(_connectionString);
            var activeTags = activeTagService.ActiveTagsByTagIdKey();
            int[] tagConfigList = GetPortListByBatchAndDate(batchId, receivedDate).ToArray();
            List<string> tagNameLiteral = new List<string>();

            foreach (var item in tagConfigList)
            {
                tagNameLiteral.Add(activeTags[item].TagName);
            }
            return tagNameLiteral;
        }

        public bool Insert(DisabledPort entity)
        {
            return _repository.Insert(entity);
        }

    }
}
