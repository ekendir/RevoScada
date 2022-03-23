using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.Entities.PageTagConfigurations
{

	public class VacuumLinesTagConfigurations
	{
		public Dictionary<string, VacuumLinesVacuumItemTag> VacuumPorts { get; set; }
		public int VacPumpControlStateIsAuto { get; set; }
		public int VacPumpControlStateOnOff { get; set; }
		public int VacSetControlStatusIsAuto { get; set; }
		public int VacSetControlStatusSp { get; set; }
		public int VacSetControlStatusPid { get; set; }
		public int SystemVacuumValuesPv { get; set; }
		public int SystemVacuumValuesRate { get; set; }
		public int SystemVacuumValuesSP { get; set; }
		public int RightVacPumpControlStateIsAuto { get; set; }
		public int RightVacPumpControlStateOnOff { get; set; }
		public int RightVacSetControlStatusIsAuto { get; set; }
		public int RightVacSetControlStatusSp { get; set; }
		public int RightVacSetControlStatusPid { get; set; }
		public int RightSystemVacuumValuesPv { get; set; }
		public int RightSystemVacuumValuesRate { get; set; }
		public int RightSystemVacuumValuesSP { get; set; }
		public int MonitoringLinesHighMonVacuumValue { get; set; }
		public int MonitoringLinesHighMonPort { get; set; }
		public int MonitoringLinesHighMonVacuumValueInTime { get; set; }
		public int MonitoringLinesLowMonVacuumValue { get; set; }
		public int MonitoringLinesLowMonPort { get; set; }
		public int MonitoringLinesLowMonVacuumValueInTime { get; set; }
		public int AllVacuumLineControlActionAuto { get; set; }
		public int AllVacuumLineControlActionManuel { get; set; }
		public int AllVacuumLineControlActionVac { get; set; }
		public int AllVacuumLineControlActionVent { get; set; }
		public int AllVacuumLineControlActionOff { get; set; }
		public List<int> DbNumbers { get; set; }

	}
}