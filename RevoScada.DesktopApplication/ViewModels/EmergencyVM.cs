using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RevoScada.ProcessController;
using RevoScada.DesktopApplication.Models;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Complex.Alarm;

namespace RevoScada.DesktopApplication.ViewModels
{
    public class EmergencyVM : ObservableObject
    {
        private  string _emergencyList;
        public  string EmergencyList
        {
            get => _emergencyList;
            set => OnPropertyChanged(ref _emergencyList, value);
        }

        public EmergencyVM()
        {
         
        }

        public void UpdateWriteCommandList()
        {
            EmergencyList = string.Empty;

            List<SiemensWriteCommandItem> siemensWriteCommandItems = new List<SiemensWriteCommandItem>();

            siemensWriteCommandItems = ProcessManager.Instance.SiemensWriteCommandItems();

            if (siemensWriteCommandItems.Count>0)
            {
                StringBuilder stringBuilder = new StringBuilder();
               // stringBuilder.AppendLine($"Command Id\t\t\t\tDB\t\t\tOffset\t\tDescription");

                foreach (var siemensWriteCommandItem in siemensWriteCommandItems)
                {
                    stringBuilder.AppendLine($"{siemensWriteCommandItem.Description}");
                }

                EmergencyList = stringBuilder.ToString();
            }
            else
            {
                EmergencyList = "There is no item in write queue!";
            }
       
        }
    }
}