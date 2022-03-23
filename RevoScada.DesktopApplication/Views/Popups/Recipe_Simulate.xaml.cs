using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Models;
using RevoScada.Entities;

namespace RevoScada.DesktopApplication.Views.Popups
{
    /// <summary>
    /// Interaction logic for Recipe_Simulate.xaml
    /// </summary>
    public partial class Recipe_Simulate : Window
    {
        private DataGrid recipeTable;

        #region RECIPE SEQUENCES:
        const int SEQ_time = 0; //1
        const int SEQ_cont = 1;//2
        const int SEQ_tset = 2;//4
        const int SEQ_rate = 3;//3
        const int SEQ_cascade = 4;//5
        const int SEQ_vSet = 5;//8
        const int SEQ_circEn = 6;//17
        const int SEQ_heatEn = 7;//18
        const int SEQ_coolEn = 8;//19
        const int SEQ_vacEn = 9;//21
        const int SEQ_critItem_1 = 10;//9
        const int SEQ_criteria_1 = 11;//10
        const int SEQ_critValue_1 = 12;//11
        const int SEQ_critAction_1 = 13;//12
        const int SEQ_critItem_2 = 14;//13
        const int SEQ_criteria_2 = 15;//14
        const int SEQ_critValue_2 = 16;//15
        const int SEQ_critAction_2 = 17;//16
        const int SEQ_airTempMax = 18;//23
        const int SEQ_pressureEnable = 19;//20
        const int SEQ_pressRate = 20;//6
        const int SEQ_pressSet = 21;//7
        int SEQ_MAX = 21;
        //const int SEQ_MAX = 18;
        #endregion


        //int[] index = { 1, 2, 4, 3, 5, 6, 15, 16, 17, 18, 7, 8, 9, 10, 11, 12, 13, 14, 20 };
        //int[] index = { 1, 2, 4, 3, 5, 8, 17, 18, 19, 21, 9,10,11,12,13,14,15,16,23,20,6,7 };
        //#MAX_SEGMENT
        int MAX_SEGMENT = 0;

        #region CONTROL & CRITERIA TYPES
        const int TYP_AIRTC = 1;
        const int TYP_LOTC = 2;
        const int TYP_HITC = 3;
        const int TYP_AVGTC = 4;
        const int TYP_LOMON = 5;
        const int TYP_HIMON = 6;
        const int TYP_PRESSURE = 7;
        #endregion

        #region CRITERIA
        const int TYP_EQ_GREATER = 1;
        const int TYP_EQ_LESS = 2;
        const int TYP_GREATER = 3;
        const int TYP_LESS = 4;
        #endregion
        #region CRITERIA ACTIONS
        const int TYP_ACT_GO = 1;
        const int TYP_ACT_AND = 2;
        const int TYP_ACT_END = 3;
        #endregion

        //ENABLED STATS
        Boolean R_heatOn;
        Boolean R_vacOn;
        Boolean R_coolOn;
        Boolean R_circOn;
        Boolean R_pressureOn;
        //SEGMENT TIME
        UInt64 R_segTime;

        int R_cont;
        int R_crit_item_1;
        int R_criteria_1;
        int R_critAct_1;
        float R_critVal_1;
        int R_crit_item_2;
        int R_criteria_2;
        int R_critAct_2;
        float R_critVal_2;
        float R_cascade;
        float R_tempSet;
        float R_tempRate;
        float R_pressRate;
        float R_vacSet;
        float R_pressSet;

       
        float T_setair;
        float T_actair;
        float T_outside;
        float T_airTempMax;
        float T_high;
        float T_low;
        float T_avg;
        float V_act;
        float T_setPress;
        float T_actPress;


        //COEFFICIENTS
        float P = (float)8;
        float I = (float)1;
        float c_resistance = (float)0.12;
        float c_air_loss = (float)0.0006;
        float c_air_exhaust = (float)0.02;
        float c_air_fan = (float)0.00025;
        float c_air_total = (float)1;
        float c_hitc = (float)0.07;
        float c_lotc = (float)0.05;

        Boolean PHASE_COOL = new Boolean();
        Boolean PHASE_HEAT = new Boolean();

        Boolean PHASE_PRESS_AZALTMA = new Boolean();
        Boolean PHASE_PRESS_ARTTIRMA = new Boolean();

        int current_seg_no = 1;
        UInt64 current_seg_minute = 0;
        UInt64 batch_total_minute = 0;
        float last_rate = 0;
        float last_pressure_rate = 0;
        int last_control_typ = 0;
        int last_seg_no = 0;
        float last_SP_temp = 0;

        float last_SP_Pressure = 0;

        float PI_Out_total = 0;
        float PI_Out_p = 0;
        float PI_Out_i = 0;
        float Q_resistance = 0;
        float Q_exhaust = 0;
        float Q_fan = 0;
        float Q_loss = 0;
        float Q_Total = 0;
        float SP_Temp = 0;
        float SP_Pressure = 0;
        float crit_pv1 = 0;
        float crit_pv2 = 0;
        Boolean crit_result1 = false;
        Boolean crit_result2 = false;
        Boolean RECIPE_CONTROL_NEXT_SEG = false;
        Boolean RECIPE_CONTROL_END_BATCH = false;
        float[] x = new float[5000];
        Random rnd = new Random();

        string[] seg_list = new string[3];
        string[] seriesNames = { "SET_AIR", "AIRTC", "HITC", "LOTC", "AVGTC", "SVAC", "SEG_NO", "SET_PRESS", "ACT_PRESS" };

        #region Collections
        private Dictionary<int, List<string>> _recipeValuesByOrderId;
        private List<string> _recipeValues;

        private List<KeyValuePair<int, float>> T_setairColl;
        private List<KeyValuePair<int, float>> T_actairColl;
        private List<KeyValuePair<int, float>> T_highColl;
        private List<KeyValuePair<int, float>> T_lowColl;
        private List<KeyValuePair<int, float>> T_avgColl;
        private List<KeyValuePair<int, float>> V_actColl;
        private List<KeyValuePair<int, float>> T_setPressureColl;
        private List<KeyValuePair<int, float>> T_actPressureColl;
        private List<KeyValuePair<int, int>> current_seg_noColl;
        #endregion

        public Recipe_Simulate(DataGrid _recipeTable)
        {
            InitializeComponent();

            recipeTable = _recipeTable;

            _recipeValuesByOrderId = new Dictionary<int, List<string>>();
            _recipeValues = new List<string>();

            T_setairColl = new List<KeyValuePair<int, float>>();
            T_actairColl = new List<KeyValuePair<int, float>>();
            T_highColl = new List<KeyValuePair<int, float>>();
            T_lowColl = new List<KeyValuePair<int, float>>();
            T_avgColl = new List<KeyValuePair<int, float>>();
            V_actColl = new List<KeyValuePair<int, float>>();
            T_setPressureColl = new List<KeyValuePair<int, float>>();
            T_actPressureColl = new List<KeyValuePair<int, float>>();
            current_seg_noColl = new List<KeyValuePair<int, int>>();

            GetRecipeSegValues();
            ChartBindData(0);
            CreateChartLineSeries();
        }

        private void GetRecipeSegValues()
        {
            IList list = recipeTable.Items as IList;
            IEnumerable<RecipeRowItemVM> items = list.Cast<RecipeRowItemVM>();

            foreach (var item in items)
            {
                _recipeValuesByOrderId.Add(item.RowHeaderOrderId, item.SegTextColl);
            }
        }

        private void CreateChartLineSeries()
        {
            foreach (var name in seriesNames)
            {
                LineSeries lineSeries = new LineSeries();
                lineSeries.Title = name;

                var lineDataPStyle = new Style();
                lineDataPStyle.TargetType = typeof(LineDataPoint);
                SolidColorBrush bgColor = new SolidColorBrush();

                switch (name)
                {
                    case "SET_AIR":
                        lineSeries.ItemsSource = T_setairColl;
                        bgColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#5AE84D");
                        break;
                    case "AIRTC":
                        lineSeries.ItemsSource = T_actairColl;
                        bgColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#EBC140");
                        break;
                    case "HITC":
                        lineSeries.ItemsSource = T_highColl;
                        bgColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#ff0000");
                        break;
                    case "LOTC":
                        lineSeries.ItemsSource = T_lowColl;
                        bgColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#BC3E2E");
                        break;
                    case "AVGTC":
                        lineSeries.ItemsSource = T_avgColl;
                        bgColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#6EDEFF");
                        break;
                    case "SVAC":
                        lineSeries.ItemsSource = V_actColl;
                        bgColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#710A2B");
                        break;
                    case "SEG_NO":
                        lineSeries.ItemsSource = current_seg_noColl;
                        bgColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#CCCC00");
                        break;
                    case "SET_PRESS":
                        lineSeries.ItemsSource = T_setPressureColl;
                        bgColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#000099");
                        break;

                    case "ACT_PRESS":
                        lineSeries.ItemsSource = T_actPressureColl;
                        bgColor = (SolidColorBrush)new BrushConverter().ConvertFromString("#0000ff");
                        break;
                    default:
                        break;
                }

                lineDataPStyle.Setters.Add(new Setter(BackgroundProperty, bgColor));
                lineDataPStyle.BasedOn = (Style)FindResource("LineDataPointStyle");
                lineSeries.DataPointStyle = lineDataPStyle;

                lineSeries.IndependentValuePath = "Key";
                lineSeries.DependentValuePath = "Value";
                recipeSim.Series.Add(lineSeries);

                var maxArgumentValue = T_setairColl.Count;

                switch (maxArgumentValue)
                {
                    case int n when n <= 10:
                        simLinearAxis.Interval = 1;
                        break;
                    case int n when (n > 10 && n <= 100):
                        simLinearAxis.Interval = 2;
                        break;
                    case int n when (n > 100 && n <= 150):
                        simLinearAxis.Interval = 5;
                        break;
                    case int n when (n > 150 && n <= 500):
                        simLinearAxis.Interval = 10;
                        break;
                    case int n when (n > 500 && n <= 1000):
                        simLinearAxis.Interval = 20;
                        break;
                    default:
                        break;
                }
            }
        }


        //int[] index = { 2, 3, 5, 4, 6, 10, 19, 20, 21, 23, 11, 12, 13, 14, 15, 16, 17, 18, 26, 22, 7, 8 }; //ces

        //int[] index = { 2, 3, 5, 4, 6, 9, 18, 19, 20, 22, 10, 11, 12, 13, 14, 15, 16, 17, 24, 21, 7, 8 };// tai
        // old
        //int[] index = { 1, 2, 4, 3, 5, 8, 17, 18, 19, 21, 9, 10, 11, 12, 13, 14, 15, 16, 23, 20, 6, 7 };
        //int[] index = { 1, 2, 4, 3, 5, 8, 17, 18, 19, 21, 9, 10, 11, 12, 13, 14, 15, 16, 20, 6, 7, 23 };
        //int[] index = { 1, 2, 4, 3, 5, 6, 15, 16, 17, 18, 7, 8, 9, 10, 11, 12, 13, 14, 20 };
        private void RefreshSegment(int segNo)
        {
            string connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;

            ApplicationPropertyService applicationPropertyService = new ApplicationPropertyService(connectionString);
            var simulateRecipeFieldsIndex = applicationPropertyService.GetByName("SimulateRecipeFieldsIndex")?.Value ?? string.Empty;
            Dictionary<string, int> recipeFieldIndex = new Dictionary<string, int>();
            recipeFieldIndex = JsonConvert.DeserializeObject<Dictionary<string, int>>(simulateRecipeFieldsIndex).ToDictionary(t => t.Key, t => t.Value);

            List<int> index = recipeFieldIndex.Select(t => t.Value).ToList();



            for (int seq = 0; seq <= SEQ_MAX; seq++)
            {
                string txtStr = String.Empty;

                try
                {
                    txtStr = _recipeValuesByOrderId[index[seq]][segNo - 1];
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                try
                {
                    switch (seq)
                    {
                        case SEQ_time: // 2   //2

                            if (String.IsNullOrWhiteSpace(txtStr)) R_segTime = 0;
                            else { txtStr = txtStr.Substring(0, txtStr.Length - 1); R_segTime = Convert.ToUInt64(txtStr); }
                            break;
                        case SEQ_cont: // 3    3
                            if (String.Equals(txtStr, "AIRTC")) R_cont = TYP_AIRTC;
                            else if (String.Equals(txtStr, "LOTC")) R_cont = TYP_LOTC;
                            else if (String.Equals(txtStr, "HITC")) R_cont = TYP_HITC;
                            else if (String.Equals(txtStr, "AVGTC")) R_cont = TYP_AVGTC;
                            break;
                        case SEQ_tset: // 5   //5
                            if (txtStr == "CURR")
                                R_tempSet = T_actair;
                            else if (!String.IsNullOrWhiteSpace(txtStr)) R_tempSet = Convert.ToSingle(txtStr);
                            break;
                        case SEQ_rate: // 4    //4
                            if (!String.IsNullOrWhiteSpace(txtStr)) R_tempRate = Convert.ToSingle(txtStr);
                            break;
                        case SEQ_cascade: // 6  // 6
                            if (txtStr == "A" || String.IsNullOrEmpty(txtStr))
                                R_cascade = 2;
                            else if (!String.IsNullOrWhiteSpace(txtStr)) R_cascade = Convert.ToSingle(txtStr);
                            break;
                        case SEQ_vSet: // 9   //10
                            if (!String.IsNullOrWhiteSpace(txtStr)) R_vacSet = Convert.ToSingle(txtStr);
                            break;
                        case SEQ_circEn: // 18  //19
                            if (string.Equals(txtStr, "ON")) R_circOn = true;
                            else if (string.Equals(txtStr, "OFF")) R_circOn = false;
                            break;
                        case SEQ_heatEn: // 19    //20
                            if (string.Equals(txtStr, "ON")) R_heatOn = true;
                            else if (string.Equals(txtStr, "OFF")) R_heatOn = false;
                            break;
                        case SEQ_coolEn: // 20  //21
                            if (string.Equals(txtStr, "ON")) R_coolOn = true;
                            else if (string.Equals(txtStr, "OFF")) R_coolOn = false;
                            break;
                        case SEQ_vacEn: // 22    //23
                            if (string.Equals(txtStr, "ON")) R_vacOn = true;
                            else if (string.Equals(txtStr, "OFF")) R_vacOn = false;
                            break;
                        case SEQ_critItem_1: // 10     //11
                            if (String.Equals(txtStr, "AIRTC")) R_crit_item_1 = TYP_AIRTC;
                            else if (String.Equals(txtStr, "LOTC")) R_crit_item_1 = TYP_LOTC;
                            else if (String.Equals(txtStr, "HITC")) R_crit_item_1 = TYP_HITC;
                            else if (String.Equals(txtStr, "AVGTC")) R_crit_item_1 = TYP_AVGTC;
                            else if (String.Equals(txtStr, "HIMON")) R_crit_item_1 = TYP_HIMON;
                            else if (String.Equals(txtStr, "LOMON")) R_crit_item_1 = TYP_LOMON;
                            else if (String.Equals(txtStr, "PRESS")) R_crit_item_1 = TYP_PRESSURE;
                            else R_crit_item_1 = 0;
                            break;
                        case SEQ_criteria_1: // 11    //12
                            if (String.Equals(txtStr, "<")) R_criteria_1 = TYP_LESS;
                            else if (String.Equals(txtStr, ">")) R_criteria_1 = TYP_GREATER;
                            else if (String.Equals(txtStr, "<=")) R_criteria_1 = TYP_EQ_LESS;
                            else if (String.Equals(txtStr, ">=")) R_criteria_1 = TYP_EQ_GREATER;
                            else R_criteria_1 = 0;
                            break;
                        case SEQ_critValue_1: // 12  //13
                            if (!String.IsNullOrWhiteSpace(txtStr)) R_critVal_1 = Convert.ToSingle(txtStr); else R_critVal_1 = 0;
                            break;
                        case SEQ_critAction_1: // 13    // 14
                            if (String.Equals(txtStr, "GO")) R_critAct_1 = TYP_ACT_GO;
                            else if (String.Equals(txtStr, "AND")) R_critAct_1 = TYP_ACT_AND;
                            else if (String.Equals(txtStr, "END")) R_critAct_1 = TYP_ACT_END;
                            else R_critAct_1 = 0;
                            break;
                        case SEQ_critItem_2: // 14    //15
                            if (String.Equals(txtStr, "AIRTC")) R_crit_item_2 = TYP_AIRTC;
                            else if (String.Equals(txtStr, "LOTC")) R_crit_item_2 = TYP_LOTC;
                            else if (String.Equals(txtStr, "HITC")) R_crit_item_2 = TYP_HITC;
                            else if (String.Equals(txtStr, "AVGTC")) R_crit_item_2 = TYP_AVGTC;
                            else if (String.Equals(txtStr, "HIMON")) R_crit_item_2 = TYP_HIMON;
                            else if (String.Equals(txtStr, "LOMON")) R_crit_item_2 = TYP_LOMON;
                            else if (String.Equals(txtStr, "PRESS")) R_crit_item_2 = TYP_PRESSURE;
                            else R_crit_item_2 = 0;
                            break;
                        case SEQ_criteria_2: // 15    //16
                            if (String.Equals(txtStr, "<")) R_criteria_2 = TYP_LESS;
                            else if (String.Equals(txtStr, ">")) R_criteria_2 = TYP_GREATER;
                            else if (String.Equals(txtStr, "<=")) R_criteria_2 = TYP_EQ_LESS;
                            else if (String.Equals(txtStr, ">=")) R_criteria_2 = TYP_EQ_GREATER;
                            else R_criteria_2 = 0;
                            break;
                        case SEQ_critValue_2: // 16    //17
                            if (!String.IsNullOrWhiteSpace(txtStr))
                                R_critVal_2 = Convert.ToSingle(txtStr);
                            else R_critVal_2 = 0;
                            break;
                        case SEQ_critAction_2: // 17    //18
                            if (String.Equals(txtStr, "GO")) R_critAct_2 = TYP_ACT_GO;
                            else if (String.Equals(txtStr, "AND")) R_critAct_2 = TYP_ACT_AND;
                            else if (String.Equals(txtStr, "END")) R_critAct_2 = TYP_ACT_END;
                            else R_critAct_2 = 0;
                            break;
                        case SEQ_airTempMax: // 24      //26
                            if (txtStr == "X")
                                T_airTempMax = 0;
                            else if (!String.IsNullOrWhiteSpace(txtStr))
                                T_airTempMax = Convert.ToSingle(txtStr);
                            else T_airTempMax = 0;
                            break;
                        case SEQ_pressureEnable: // 21   // 22
                            if (string.Equals(txtStr, "AIR") || String.Equals(txtStr, "AZOT")) R_pressureOn = true;
                            else if (string.Equals(txtStr, "OFF")) R_pressureOn = false;
                            break;

                        case SEQ_pressRate: // 7    //7
                            if (!String.IsNullOrWhiteSpace(txtStr)) R_pressRate = Convert.ToSingle(txtStr);
                            break;
                        case SEQ_pressSet: // 8    //8
                            if (!String.IsNullOrWhiteSpace(txtStr)) R_pressSet = Convert.ToSingle(txtStr);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        private void ChartBindData(int page)
        {
            if(ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId ==3)
            {
                SEQ_MAX = 18;
            }
           

            if (page == 0)
                recipeSim.Series.Clear();

            // Subtract 2 from the total columns count for table and unit names. 
            // It'll get only total number of segments columns.
            MAX_SEGMENT = recipeTable.Columns.Count - 2;

            //INITIAL VALUES:
            PI_Out_p = 0;
            PI_Out_i = 0;
            PI_Out_total = 0;

            Q_loss = 0;
            Q_resistance = 0;
            Q_exhaust = 0;
            Q_fan = 0;
            Q_Total = 0;

            T_setPress = 0;
            T_actPress = 0;


            T_setair = 25;
            T_actair = 25;
            T_outside = 25;
            T_high = 25;
            T_low = 25;
            T_avg = 25;
            T_airTempMax = 0;

            for (int minute = 0; minute < 1000; minute++)
            {
                if (page == 0)
                {
                    T_setairColl.Add(new KeyValuePair<int, float>(minute, T_setair));
                    T_actairColl.Add(new KeyValuePair<int, float>(minute, T_actair));
                    T_highColl.Add(new KeyValuePair<int, float>(minute, T_high));
                    T_lowColl.Add(new KeyValuePair<int, float>(minute, T_low));
                    T_avgColl.Add(new KeyValuePair<int, float>(minute, T_avg));
                    V_actColl.Add(new KeyValuePair<int, float>(minute, V_act));
                    T_setPressureColl.Add(new KeyValuePair<int, float>(minute, T_setPress));
                    T_actPressureColl.Add(new KeyValuePair<int, float>(minute, T_actPress));



                    current_seg_noColl.Add(new KeyValuePair<int, int>(minute, current_seg_no));
                }
                //chartTable.Rows.Add(minute, T_setair, T_actair, T_high, T_low, T_avg, V_act, current_seg_no);

                //SEGMENT REFRESH:
                if (last_seg_no != current_seg_no)
                {
                    last_seg_no = current_seg_no;
                    current_seg_minute = 0;
                    RefreshSegment(current_seg_no);
                }

                //SEGMENT CONTROL...
                batch_total_minute = batch_total_minute + 1;
                current_seg_minute = current_seg_minute + 1;
                if (R_segTime > 0)
                {
                    RECIPE_CONTROL_NEXT_SEG = (R_segTime <= current_seg_minute);
                }
                else
                {
                    switch (R_crit_item_1)
                    {
                        case TYP_AIRTC:
                            crit_pv1 = T_actair;
                            break;
                        case TYP_LOTC:
                            crit_pv1 = T_low;
                            break;
                        case TYP_HITC:
                            crit_pv1 = T_high;
                            break;
                        case TYP_AVGTC:
                            crit_pv1 = T_avg;
                            break;
                        case TYP_HIMON:
                            crit_pv1 = V_act;
                            break;
                        case TYP_LOMON:
                            crit_pv1 = V_act;
                            break;
                        case TYP_PRESSURE:
                            crit_pv1 = T_actPress;
                            break;
                    }
                    switch (R_criteria_1)
                    {
                        case TYP_EQ_LESS:
                            crit_result1 = (crit_pv1 <= R_critVal_1);
                            break;
                        case TYP_LESS:
                            crit_result1 = (crit_pv1 < R_critVal_1);
                            break;
                        case TYP_EQ_GREATER:
                            crit_result1 = (crit_pv1 >= R_critVal_1);
                            break;
                        case TYP_GREATER:
                            crit_result1 = (crit_pv1 > R_critVal_1);
                            break;
                    }
                    switch (R_crit_item_2)
                    {
                        case TYP_AIRTC:
                            crit_pv2 = T_actair;
                            break;
                        case TYP_LOTC:
                            crit_pv2 = T_low;
                            break;
                        case TYP_HITC:
                            crit_pv2 = T_high;
                            break;
                        case TYP_AVGTC:
                            crit_pv2 = T_avg;
                            break;
                        case TYP_HIMON:
                            crit_pv2 = V_act;
                            break;
                        case TYP_LOMON:
                            crit_pv2 = V_act;
                            break;
                        case TYP_PRESSURE:
                            crit_pv2 = T_actPress;
                            break;
                    }
                    switch (R_criteria_2)
                    {
                        case TYP_EQ_LESS:
                            crit_result2 = (crit_pv2 <= R_critVal_2);
                            break;
                        case TYP_LESS:
                            crit_result2 = (crit_pv2 < R_critVal_2);
                            break;
                        case TYP_EQ_GREATER:
                            crit_result2 = (crit_pv2 >= R_critVal_2);
                            break;
                        case TYP_GREATER:
                            crit_result2 = (crit_pv2 > R_critVal_2);
                            break;
                    }

                    switch (R_critAct_1)
                    {
                        case 0:
                            RECIPE_CONTROL_NEXT_SEG = (R_critAct_2 == 0);
                            break;
                        case TYP_ACT_GO:
                            RECIPE_CONTROL_NEXT_SEG = crit_result1;
                            break;
                        case TYP_ACT_AND:
                            RECIPE_CONTROL_NEXT_SEG = crit_result1 && crit_result2;
                            break;
                        case TYP_ACT_END:
                            RECIPE_CONTROL_END_BATCH = crit_result1;
                            break;
                    }
                    if (!RECIPE_CONTROL_END_BATCH && !RECIPE_CONTROL_NEXT_SEG)
                    {
                        switch (R_critAct_2)
                        {
                            case TYP_ACT_GO:
                                RECIPE_CONTROL_NEXT_SEG = crit_result2;
                                break;
                            case TYP_ACT_AND:
                                RECIPE_CONTROL_NEXT_SEG = crit_result1 && crit_result2;
                                break;
                            case TYP_ACT_END:
                                RECIPE_CONTROL_END_BATCH = crit_result2;
                                break;
                        }
                    }
                }
                if (RECIPE_CONTROL_NEXT_SEG) current_seg_no = current_seg_no + 1;
                RECIPE_CONTROL_NEXT_SEG = false;
                if (current_seg_no > MAX_SEGMENT) RECIPE_CONTROL_END_BATCH = true;
                if (RECIPE_CONTROL_END_BATCH) break;


                //VACUUM ACTUAL:
                float vacOscRandom = (float)rnd.Next(-49, 49);
                vacOscRandom = vacOscRandom / 10;
                if (R_vacOn) V_act = R_vacSet + vacOscRandom; //(float)(50 * Math.Sin(freqVac));
                else V_act = 0;

                //PHASE DEFINITION & SP_TEMP INIT:
                if (last_SP_temp != R_tempSet || last_control_typ != R_cont || last_rate != R_tempRate)
                {
                    last_control_typ = R_cont;
                    last_rate = R_tempRate;
                    last_SP_temp = R_tempSet;

                    switch (R_cont)
                    {
                        case TYP_AIRTC:
                            SP_Temp = T_actair;
                            break;
                        case TYP_LOTC:
                            SP_Temp = T_low;
                            break;
                        case TYP_HITC:
                            SP_Temp = T_high;
                            break;
                        case TYP_AVGTC:
                            SP_Temp = T_avg;
                            break;

                    }
                    if (R_tempSet > SP_Temp)
                    {
                        PHASE_HEAT = true;
                        PHASE_COOL = false;
                    }
                    else
                    {
                        PHASE_HEAT = false;
                        PHASE_COOL = true;
                    }


                }


                //PHASE DEFINITION & SP_PRESSURE INIT:
                if (last_SP_Pressure != R_pressSet || last_pressure_rate != R_pressRate)
                {

                    last_pressure_rate = R_pressRate;
                    last_SP_Pressure = R_pressSet;


                    SP_Pressure = T_actPress;

                    if (R_pressSet > SP_Pressure)
                    {
                        PHASE_PRESS_ARTTIRMA = true;
                        PHASE_PRESS_AZALTMA = false;
                    }
                    else
                    {
                        PHASE_PRESS_ARTTIRMA = false;
                        PHASE_PRESS_AZALTMA = true;
                    }


                }


                //RATE INCREMENT TEMPERATURE:
                if (minute > 0)
                {
                    if (PHASE_HEAT)
                    {
                        SP_Temp = SP_Temp + R_tempRate;
                        if (SP_Temp > R_tempSet) SP_Temp = R_tempSet;
                    }
                    else if (PHASE_COOL)
                    {
                        SP_Temp = SP_Temp - R_tempRate;
                        if (SP_Temp < R_tempSet) SP_Temp = R_tempSet;
                    }
                }



                //Pressure actual:
                //T_actPress = T_actPress + (R_pressRate * (float)0.9);
                T_setPress = SP_Pressure;

                //RATE INCREMENT PRESSURE:
                if (minute > 0)
                {
                    if (PHASE_PRESS_ARTTIRMA)
                    {
                        SP_Pressure = SP_Pressure + R_pressRate;
                        if (R_pressureOn) T_actPress = T_actPress + (R_pressRate * (float)0.9);
                        if (SP_Pressure > R_pressSet) SP_Pressure = R_pressSet;
                        if (T_actPress > T_setPress) T_actPress = T_setPress;
                    }
                    else if (PHASE_PRESS_AZALTMA)
                    {
                        SP_Pressure = SP_Pressure - R_pressRate;
                        if (R_pressureOn) T_actPress = T_actPress - (R_pressRate * (float)0.9);

                        if (SP_Pressure < R_pressSet) SP_Pressure = R_pressSet;
                        if (T_actPress < T_setPress) T_actPress = T_setPress;
                    }
                }




                //SET AIR VALUE:
                switch (R_cont)
                {
                    case TYP_AIRTC:
                        T_setair = SP_Temp;
                        break;
                    case TYP_LOTC:
                        T_setair = T_low + R_cascade * (SP_Temp - T_low);
                        if (T_airTempMax > SP_Temp && T_setair > T_airTempMax) T_setair = T_airTempMax;
                        break;
                    case TYP_HITC:
                        T_setair = T_high + R_cascade * (SP_Temp - T_high);
                        if (T_airTempMax > SP_Temp && T_setair > T_airTempMax) T_setair = T_airTempMax;
                        break;
                    case TYP_AVGTC:
                        T_setair = T_avg + R_cascade * (SP_Temp - T_avg);
                        break;
                }
                if (T_setair > 400) T_setair = 400;
                if (T_setair < 0) T_setair = 0;


                //MATH MODEL:
                //pi control:
                PI_Out_p = P * (T_setair - T_actair);
                if (PI_Out_p > 100) PI_Out_p = 100;
                if (PI_Out_p < -50) PI_Out_p = -50;
                PI_Out_i = PI_Out_i + I * (T_setair - T_actair);
                if (PI_Out_i > 10) PI_Out_i = 10;
                if (PI_Out_i < -5) PI_Out_i = -5;
                PI_Out_total = PI_Out_i + PI_Out_p;
                if (PI_Out_total > 100) PI_Out_total = 100;
                if (PI_Out_total < -50) PI_Out_total = -50;
                //enabled checks:
                if ((!R_heatOn || (!R_circOn && T_actair < 150)) && PI_Out_total > 0) PI_Out_total = 0; //circ off NOT ALLOWED! under 150
                if (!R_coolOn && PI_Out_total < -15) PI_Out_total = -15;                                //coolOn has effect under PI_out: -15

                //PI_Out_total = 0;

                //heat of resistance 
                if ((PI_Out_total >= 0)) Q_resistance = c_resistance * PI_Out_total;
                else Q_resistance = 0;
                //heat of furnace loss
                if ((T_actair >= T_outside)) Q_loss = c_air_loss * (T_outside - T_actair);
                else Q_loss = 0;
                //heat of exhaust
                if ((T_actair >= T_outside) && (PI_Out_total < 0)) Q_exhaust = c_air_exhaust * (T_outside - T_actair);
                else Q_exhaust = 0;
                //heat of cooling fan
                if ((PHASE_COOL) && (PI_Out_total < -15)) Q_fan = c_air_fan * (T_outside - T_actair) * -PI_Out_total;
                else Q_fan = 0;
                //Total Heat
                Q_Total = Q_resistance + Q_loss + Q_exhaust + Q_fan;

                //air actual:
                T_actair = T_actair + Q_Total * c_air_total;

                //part temperatures
                T_high = T_high + (T_actair - T_high) * c_hitc;
                T_low = T_low + (T_actair - T_low) * c_lotc;

                T_avg = (T_high + T_low) / 2;

            }
        }
    }
}
