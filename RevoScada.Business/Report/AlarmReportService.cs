using RevoScada.Entities;
using RevoScada.Entities.Complex.Report;
using RevoScada.Entities.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace RevoScada.Business.Report
{
    public class AlarmReportService
    {
        private string _connectionString { get; set; }
        public object AlarmTagConfigurations { get; set; }

        public AlarmReportService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public AlarmReportModel PlcAlarmReport(int batchId)
        {
            ReportHeaderInfoService reportHeaderInfoService = new ReportHeaderInfoService(_connectionString);

           
            AlarmReportModel plcAlarmReportModel = new AlarmReportModel();

            plcAlarmReportModel.ReportHeaderInfo = reportHeaderInfoService.ReportHeaderInfo(batchId);
            plcAlarmReportModel.PlcAlarmReportItems = PlcAlarmsByBatch(batchId);
            plcAlarmReportModel.ProcessEventLogReportItems = ProcessEventLogReportItem(batchId);


            return plcAlarmReportModel;
        }

        public List<PlcAlarmReportItem> PlcAlarmsByBatch(int batchId)
        {
            List<PlcAlarmReportItem> plcAlarmReportItems = new List<PlcAlarmReportItem>();

            PlcAlarmService plcAlarmService = new PlcAlarmService(_connectionString);

            ActiveTagService activeTagService = new ActiveTagService(_connectionString);

            var activeTags = activeTagService.ActiveTagsByTagIdKey();

            IEnumerable<PlcAlarm> plcAlarms = plcAlarmService.GetByBatchId(batchId);

            if (plcAlarms.Count() == 0)
            {
                return null;
            }
            
            //todo:h Alarm tagname düzenlenecek
            foreach (var plcAlarm in plcAlarms)
            {
                PlcAlarmReportItem plcAlarmReportItem = new PlcAlarmReportItem();
                plcAlarmReportItem.MessageName =((SiemensTagConfiguration)(((Dictionary<int, ITagConfiguration>)AlarmTagConfigurations)[plcAlarm.TagConfigurationId])).TagName;
                plcAlarmReportItem.Status = plcAlarm.Status;
                plcAlarmReportItem.InDateTime = plcAlarm.InDateTime;
                plcAlarmReportItem.OutDateTime = plcAlarm.OutDateTime;
                plcAlarmReportItem.AcknowledgedDateTime = plcAlarm.AcknowledgedDateTime;


                plcAlarmReportItems.Add(plcAlarmReportItem);
            }


            return plcAlarmReportItems;
        }

        public List<ProcessEventLogReportItem> ProcessEventLogReportItem(int batchId)
        {
            List<ProcessEventLogReportItem> processEventLogReportItems = new List<ProcessEventLogReportItem>();

            ProcessEventLogService processEventLogService = new ProcessEventLogService(_connectionString);

            ActiveTagService activeTagService = new ActiveTagService(_connectionString);

            var activeTags = activeTagService.ActiveTagsByTagIdKey();

            IEnumerable<ProcessEventLog> processEventLogs = processEventLogService.GetByBatchId(batchId);

            if (processEventLogs.Count() == 0)
            {
                return null;
            }

            foreach (var processEventLog in processEventLogs)
            {
                ProcessEventLogReportItem processEventLogReportItem = new ProcessEventLogReportItem();
                processEventLogReportItem.EventText = processEventLog.EventText;
                processEventLogReportItem.InTime = processEventLog.CreateDate;
                processEventLogReportItem.AlarmType = processEventLog.Type;


                processEventLogReportItems.Add(processEventLogReportItem);
            }


            return processEventLogReportItems;
        }
    }
}