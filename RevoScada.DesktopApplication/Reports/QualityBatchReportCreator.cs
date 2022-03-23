using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Reports
{
    public class QualityBatchReportCreator
    {
        public int getTimes(string columnNamePhaseChange, string criteriaPhaseChange, int criteriaPhaseChangeValue, int LastMins, DataTable numericDataTable)
        {
            int returnValue = 0;

            DataRow getTimesDataRow = numericDataTable.Select($"Mins >={LastMins} and {columnNamePhaseChange + criteriaPhaseChange + criteriaPhaseChangeValue}").FirstOrDefault();

            if (getTimesDataRow == null)
            { return 0; }
            int getMins = Convert.ToInt32(getTimesDataRow["Mins"]);

            if (getTimesDataRow == null)
                returnValue = -1;

            returnValue = getMins;

            return returnValue;
        }
        public string[] getMaxPressureValue(int Mins, DataTable numericDataTable)
        {
            string[] returnValue = { "", "" };

            double maxPressureValue = Convert.ToDouble(numericDataTable.Compute("max([Pressure_Actual])", $"Mins ={Mins}"));

            returnValue[0] = maxPressureValue.ToString("0.000");
            returnValue[1] = "Pressure_Actual";
            return returnValue;
        }
        public string[] getMinPressureValue(int Mins, DataTable numericDataTable)
        {
            string[] returnValue = { "", "" };

            double minPressureValue = Convert.ToDouble(numericDataTable.Compute("min([Pressure_Actual])", $"Mins ={Mins}"));

            returnValue[0] = minPressureValue.ToString("0.000");
            returnValue[1] = "Pressure_Actual";
            return returnValue;
        }
        public string[] getMaxPressureDuringValue(int MinStart, int MinEnd, DataTable numericDataTable)
        {
            string[] returnValue = { "", "" };

            double maxPressureValue = Convert.ToDouble(numericDataTable.Compute("max([Pressure_Actual])", $"Mins >={MinStart} and Mins <= {MinEnd}"));

            returnValue[0] = maxPressureValue.ToString("0.000");
            returnValue[1] = "Pressure_Actual";
            return returnValue;
        }
        public string[] getMinPressureDuringValue(int MinStart, int MinEnd, DataTable numericDataTable)
        {
            string[] returnValue = { "", "" };

            double minPressureValue = Convert.ToDouble(numericDataTable.Compute("min([Pressure_Actual])", $"Mins >={MinStart} and Mins <= {MinEnd}"));

            returnValue[0] = minPressureValue.ToString("0.000");
            returnValue[1] = "Pressure_Actual";
            return returnValue;
        }
        public string[] getMaxMonValue(int Mins, DataTable numericDataTable)
        {
            string[] returnValue = { "", "" };

            string[] columnNames = (from dc in numericDataTable.Columns.Cast<DataColumn>()
                                    select dc.ColumnName).Where(x => x.Contains("MON")).ToArray();

            DataRow getMonDataRow = numericDataTable.Select($"Mins = {Mins}").FirstOrDefault();
            Dictionary<string, double> keyValuePairs = new Dictionary<string, double>();

            foreach (var selectedMon in columnNames)
            {
                //int minLavel = Convert.ToInt32(batchNumericReportModel.NumericDataTable.Compute($"MAX([{itemx}])", string.Empty));

                double monValue = Convert.ToDouble(getMonDataRow[selectedMon]);

                keyValuePairs.Add(selectedMon, monValue);
            }

            //var max = keyValuePairs.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

            var maxValue = keyValuePairs.OrderByDescending(x => x.Value).FirstOrDefault();
            returnValue[0] = Convert.ToString(maxValue.Value.ToString("0.000"));// keyValuePairs.Keys.Max(); //
            returnValue[1] = maxValue.Key;// Convert.ToString(keyValuePairs.Values.Max()); // 
            //double maxValue = keyValuePairs.Values.Max();

            return returnValue;
        }
        public string[] getMinMonValue(int Mins, DataTable numericDataTable)
        {
            string[] returnValue = { "", "" };

            string[] columnNames = (from dc in numericDataTable.Columns.Cast<DataColumn>()
                                    select dc.ColumnName).Where(x => x.Contains("MON")).ToArray();

            DataRow getMonDataRow = numericDataTable.Select($"Mins = {Mins}").FirstOrDefault();
            Dictionary<string, double> keyValuePairs = new Dictionary<string, double>();

            foreach (var selectedMon in columnNames)
            {
                double monValue = Convert.ToDouble(getMonDataRow[selectedMon]);

                keyValuePairs.Add(selectedMon, monValue);
            }

            var minValue = keyValuePairs.OrderBy(x => x.Value).FirstOrDefault();
            returnValue[0] = Convert.ToString(minValue.Value.ToString("0.000"));
            returnValue[1] = minValue.Key;

            return returnValue;
        }
        public string[] getMaxAirValue(int MinStart, int MinEnd, DataTable numericDataTable)
        {
            string[] returnValue = { "", "" };
            try
            {
                double maxAirValue = Convert.ToDouble(numericDataTable.Compute("max([Air_Tc])", $"Mins >={MinStart} and Mins <= {MinEnd}"));

                returnValue[0] = maxAirValue.ToString("0.000");
                returnValue[1] = "Air_Tc";
            }
            catch 
            {
            }
            return returnValue;
        }
        public string[] getMinAirValue(int MinStart, int MinEnd, DataTable numericDataTable)
        {
            string[] returnValue = { "", "" };
            try
            {

                double minAirValue = Convert.ToInt32(numericDataTable.Compute("min([Air_Tc])", $"Mins >={MinStart} and Mins <= {MinEnd}"));
                returnValue[0] = minAirValue.ToString("0.000");
                returnValue[1] = "Air_Tc";
            }
            catch
            {
            }
            return returnValue;
        }
        public PtcRate GetPhasePartRate(DataTable numericDataTable, int MinStart, int MinEnd, int LowRange, int HighRange, int Interval, bool RangeControl = false)
        {
            PtcRate ptcRate = new PtcRate();
            DataTable numericDataTableFilterMins = new DataTable();

            try
            {
                string[] allColumnsName = (from dc in numericDataTable.Columns.Cast<DataColumn>()
                                           select dc.ColumnName).Where(x => x.Contains("PTC")).ToArray();

                numericDataTable.DefaultView.RowFilter = $"Mins >={MinStart} and Mins <= {MinEnd}";

                List<string> filterColumnsName = new List<string>();

                foreach (string columnName in allColumnsName)
                {
                    numericDataTableFilterMins = numericDataTable.DefaultView.ToTable(false, columnName);
                    var firstValue = numericDataTableFilterMins.Rows[0].ItemArray[0];
                    var endValue = numericDataTableFilterMins.Rows[numericDataTableFilterMins.Rows.Count - 1].ItemArray[0];

                    //todo:m QUALITY koşula bağlı buraya girecek. - kartta range girilmişse
                    if (RangeControl)
                    {
                        if (Convert.ToDouble(firstValue) >= LowRange && HighRange <= Convert.ToDouble(endValue))
                        {
                            filterColumnsName.Add(columnName);
                        }
                    }
                    else { filterColumnsName.Add(columnName); }

                }

                if (numericDataTableFilterMins.Rows.Count == 0)
                {
                    ptcRate.maxRateValue = 0;
                    ptcRate.minRateValue = 0;
                }

                foreach (string ptcPortName in filterColumnsName)
                {
                    numericDataTableFilterMins = numericDataTable.DefaultView.ToTable(false, ptcPortName, "Mins");
                    if (numericDataTableFilterMins.Rows.Count == 0)
                    {
                        ptcRate.maxRateValue = 0;
                        ptcRate.minRateValue = 0;
                    }



                    int rowCount = numericDataTableFilterMins.Rows.Count; //Convert.ToInt32(numericDataTableFilterMins.Rows[numericDataTableFilterMins.Rows.Count]["Mins"]);
                    int rowIndex = 0;

                    for (int i = 1; i < numericDataTableFilterMins.Rows.Count; i++)
                    {
                        //double diff =( Convert.ToDouble(numericDataTableFilterMins.Rows[i].ItemArray[0]) - Convert.ToDouble(numericDataTableFilterMins.Rows[(i+Interval)].ItemArray[0])) /Interval;

                        int lastRowIndex = i + Interval;


                        if (rowCount <= lastRowIndex)
                        {
                            rowIndex = (i - 1);
                        }
                        else
                        {
                            rowIndex = lastRowIndex;
                        }


                        double diff = (Convert.ToDouble(numericDataTableFilterMins.Rows[rowIndex].ItemArray[0]) - Convert.ToDouble(numericDataTableFilterMins.Rows[i].ItemArray[0])) / Interval;
                        diff = Convert.ToDouble(diff.ToString("0.000"));
                        if (ptcRate.maxRateValue < diff)
                        {
                            ptcRate.maxRateValue = diff;
                            ptcRate.maxRateCHName = ptcPortName;
                            ptcRate.maxRateMins = Convert.ToInt32(numericDataTableFilterMins.Rows[i - 1]["Mins"]);
                        }

                        if (ptcRate.minRateValue > diff)
                        {
                            ptcRate.minRateValue = diff;
                            ptcRate.minRateCHName = ptcPortName;
                            ptcRate.minRateMins = Convert.ToInt32(numericDataTableFilterMins.Rows[i - 1]["Mins"]);
                        }
                    }
                }
            }
            catch
            {
            }
            return ptcRate;
        }
        public PtcRate GetPhasePressureRate(DataTable numericDataTable, int MinStart, int MinEnd)
        {
            PtcRate ptcRate = new PtcRate();
            DataTable numericDataTableFilterMins = new DataTable();

            try
            {
                numericDataTable.DefaultView.RowFilter = $"Mins >={MinStart} and Mins <= {MinEnd}";

                numericDataTableFilterMins = numericDataTable.DefaultView.ToTable(false, "Pressure_Actual", "Mins");

                if (numericDataTableFilterMins.Rows.Count == 0)
                {
                    ptcRate.maxRateValue = 0;
                    ptcRate.minRateValue = 0;
                }

                if (numericDataTableFilterMins.Rows.Count == 0)
                {
                    ptcRate.maxRateValue = 0;
                    ptcRate.minRateValue = 0;
                }

                for (int i = 1; i < numericDataTableFilterMins.Rows.Count; i++)
                {
                    double diff = Convert.ToDouble(numericDataTableFilterMins.Rows[i].ItemArray[0]) - Convert.ToDouble(numericDataTableFilterMins.Rows[i - 1].ItemArray[0]);
                    diff = Convert.ToDouble(diff.ToString("0.000"));
                    if (ptcRate.maxRateValue < diff)
                    {
                        ptcRate.maxRateValue = diff;
                        ptcRate.maxRateCHName = "Pressure_Actual";
                        ptcRate.maxRateMins = Convert.ToInt32(numericDataTableFilterMins.Rows[i]["Mins"]);
                        // ptcRate.maxRateMins = Convert.ToInt32(numericDataTableFilterMins.Rows[i - 1]["Mins"]);
                    }

                    if (ptcRate.minRateValue > diff)
                    {
                        ptcRate.minRateValue = diff;
                        ptcRate.minRateCHName = "Pressure_Actual";
                        ptcRate.minRateMins = Convert.ToInt32(numericDataTableFilterMins.Rows[i]["Mins"]);
                        // ptcRate.minRateMins = Convert.ToInt32(numericDataTableFilterMins.Rows[i - 1]["Mins"]);
                    }
                }
            }
            catch
            {
            }
            return ptcRate;
        }
        public class PtcRate
        {
            public double? maxRateValue { get; set; }
            public int maxRateMins { get; set; }
            public string maxRateCHName { get; set; }
            public double? minRateValue { get; set; }
            public int minRateMins { get; set; }
            public string minRateCHName { get; set; }

            public PtcRate()
            {
                minRateValue = double.MaxValue;
                maxRateValue = double.MinValue;
            }
        }


    }
}
