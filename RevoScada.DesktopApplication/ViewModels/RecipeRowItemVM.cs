using DevExpress.Xpf.WindowsUI;
using RevoScada.ProcessController;
using RevoScada.Synchronization.Enums;
using RevoScada.Business;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.ViewModels;
using RevoScada.Entities;
using RevoScada.Entities.Complex;
using RevoScada.Entities.Configuration;
using RevoScada.Entities.Configuration.Service;
using RevoScada.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace RevoScada.DesktopApplication.Models
{
    public class RecipeRowItemVM
    {
        public RecipeDetail RecipeDetail { get; set; }

        public string RowHeader { get; set; }

        public short RowHeaderId { get; set; }
        public short RowHeaderOrderId { get; set; }

        public List<KeyValuePair<string, string>> PredefinedRecipeFields;

        public ushort RowColorIndex { get; set; }

        public List<string> SegTextColl { get; set; } = new List<string>();

        public List<int> SegOffsetColl { get; set; } = new List<int>();

        public bool IsTableEnabled { get; set; }

        public List<bool> ValueChangedStates { get; set; } = new List<bool>();

        public List<string> OriginalRecipeValues { get; set; } = new List<string>();

        public string[] ComboboxList
        {
            get
            {
                if (PredefinedRecipeFields == null || PredefinedRecipeFields.Count == 0)
                    return null;

                List<string> values = PredefinedRecipeFields.Where(p => p.Key == RowHeader)
                                      .Select(p => p.Value).ToList();
                if (values.Count == 0)
                    return null;

                values.Insert(0, string.Empty);
                return values.ToArray();
            }
        }

        public List<KeyValuePair<string, string>> TightComboboxList
        {
            get
            {
                if (PredefinedRecipeFields == null || PredefinedRecipeFields.Count == 0)
                    return null;

                List<KeyValuePair<string, string>> comboBoxPairs = new List<KeyValuePair<string, string>>();
                List<string> values = PredefinedRecipeFields.Where(p => p.Key == RowHeader)
                                     .Select(p => p.Value).ToList();
                if (values.Count == 0)
                    return null;

                foreach (var item in values)
                {
                    int itemLength = item.Length;

                    if (item.Length > 3)
                        itemLength = 3;

                    comboBoxPairs.Add(new KeyValuePair<string, string>(item, item.Substring(0, itemLength)));
                }

                comboBoxPairs.Insert(0, new KeyValuePair<string, string>(string.Empty, string.Empty));
                return comboBoxPairs;
            }
        }

        public string UnitName
        {
            get
            {
                switch (RowHeader)
                {
                    case "Segment Time": return ProcessManager.Instance.ApplicationProperties["RecipeTimeFormatTitle"].Value;
                    case "Temp. Rate": return "°C / min.";
                    case "Temp. Value": return "°C";
                    case "Vacuum Value": return ProcessManager.Instance.ApplicationProperties["VacuumUnitTitle"].Value;
                    case "Max Air Temp.": return "°C";
                    case "Min Part Temp.": return "°C";
                    case "- Grace1": return ProcessManager.Instance.ApplicationProperties["RecipeTimeFormatTitle"].Value;
                    case "- Grace2": return ProcessManager.Instance.ApplicationProperties["RecipeTimeFormatTitle"].Value;
                    case "Max Part Temp.": return "°C";
                    case "- Grace3": return ProcessManager.Instance.ApplicationProperties["RecipeTimeFormatTitle"].Value;
                    case "Max Airpart Spread": return "°C";
                    case "- Grace4": return ProcessManager.Instance.ApplicationProperties["RecipeTimeFormatTitle"].Value;
                    case "Max Part Spread": return "°C";
                    case "- Grace5": return ProcessManager.Instance.ApplicationProperties["RecipeTimeFormatTitle"].Value;
                    case "- Grace6": return ProcessManager.Instance.ApplicationProperties["RecipeTimeFormatTitle"].Value;
                    case "- Grace7": return ProcessManager.Instance.ApplicationProperties["RecipeTimeFormatTitle"].Value;
                    case "Max Mon. Press": return ProcessManager.Instance.ApplicationProperties["VacuumUnitTitle"].Value;
                    case "Min Mon Vacuum": return ProcessManager.Instance.ApplicationProperties["VacuumUnitTitle"].Value;
                }
                return null;
            }
        }

        public ActiveRecipeVM ActiveRecipeVM;
        public RecipeEditorVM RecipeEditorVM;
        public string columnType
        {
            get
            {
                int collCount = 0;

                if (IsComboBoxTight)
                    collCount = TightComboboxList?.Count ?? 0;
                else if(ComboboxList != null)
                    collCount = ComboboxList.Count();

                if (collCount > 0 && !IsReportingActive)
                    return "combobox";
                else
                    return "textbox";
            }
        }

        public bool IsReportingActive { get; set; } = false;
        public bool IsComboBoxTight { get; set; } = false;

        public Style TextBoxStyle { get; set; }
        public Style ComboBoxStyle { get; set; }
        public Style ComboBoxValueChangedStyle { get; set; }
        public string HeaderBrushString { get; set; } = "#303030";
        public int PlcDeviceId { get; set; }
        public short RecipeDbNumber { get; set; }
        public short RecipeOffsetLength { get; set; }

        #region Fields
        private List<TextBox> _textBoxList = new List<TextBox>();

        private List<ComboBox> _comboBoxList = new List<ComboBox>();

        public short[] TwoOffsetFieldIdNumbers;
        private string _comboBoxOldValue;
        private string _textBoxOldValue;
        private DispatcherTimer _timer;
        private bool _preventComboBoxChangedEvent;
        #endregion

        #region Control Properties
        public Control MyControlTable { get { return GetControlForTableName(); } }
        public Control MyControlUnit { get { return GetControlForUnitName(); } }
        public Control MyControl1 { get { return GetControlForSegText("SegTextColl[0]"); } }
        public Control MyControl2 { get { return GetControlForSegText("SegTextColl[1]"); } }
        public Control MyControl3 { get { return GetControlForSegText("SegTextColl[2]"); } }
        public Control MyControl4 { get { return GetControlForSegText("SegTextColl[3]"); } }
        public Control MyControl5 { get { return GetControlForSegText("SegTextColl[4]"); } }
        public Control MyControl6 { get { return GetControlForSegText("SegTextColl[5]"); } }
        public Control MyControl7 { get { return GetControlForSegText("SegTextColl[6]"); } }
        public Control MyControl8 { get { return GetControlForSegText("SegTextColl[7]"); } }
        public Control MyControl9 { get { return GetControlForSegText("SegTextColl[8]"); } }
        public Control MyControl10 { get { return GetControlForSegText("SegTextColl[9]"); } }
        public Control MyControl11 { get { return GetControlForSegText("SegTextColl[10]"); } }
        public Control MyControl12 { get { return GetControlForSegText("SegTextColl[11]"); } }
        public Control MyControl13 { get { return GetControlForSegText("SegTextColl[12]"); } }
        public Control MyControl14 { get { return GetControlForSegText("SegTextColl[13]"); } }
        public Control MyControl15 { get { return GetControlForSegText("SegTextColl[14]"); } }
        public Control MyControl16 { get { return GetControlForSegText("SegTextColl[15]"); } }
        public Control MyControl17 { get { return GetControlForSegText("SegTextColl[16]"); } }
        public Control MyControl18 { get { return GetControlForSegText("SegTextColl[17]"); } }
        public Control MyControl19 { get { return GetControlForSegText("SegTextColl[18]"); } }
        public Control MyControl20 { get { return GetControlForSegText("SegTextColl[19]"); } }
        public Control MyControl21 { get { return GetControlForSegText("SegTextColl[20]"); } }
        public Control MyControl22 { get { return GetControlForSegText("SegTextColl[21]"); } }
        public Control MyControl23 { get { return GetControlForSegText("SegTextColl[22]"); } }
        public Control MyControl24 { get { return GetControlForSegText("SegTextColl[23]"); } }
        public Control MyControl25 { get { return GetControlForSegText("SegTextColl[24]"); } }
        public Control MyControl26 { get { return GetControlForSegText("SegTextColl[25]"); } }
        public Control MyControl27 { get { return GetControlForSegText("SegTextColl[26]"); } }
        public Control MyControl28 { get { return GetControlForSegText("SegTextColl[27]"); } }
        public Control MyControl29 { get { return GetControlForSegText("SegTextColl[28]"); } }
        public Control MyControl30 { get { return GetControlForSegText("SegTextColl[29]"); } }
        #endregion


        //todo:h Timer control hasn't finished yet -Onur.
        //public RecipeRowItemVM()
        //{
        //    _timer = new DispatcherTimer();
        //    _timer.Interval = TimeSpan.FromSeconds(10);
        //    _timer.Tick += Timer_Tick;
        //    _timer.Start();
        //}
        //void Timer_Tick(object sender, EventArgs e)
        //{
        //    if (specificTableNames.Contains(RowHeader) && !IsReportingActive)
        //        GetComboBoxValFromPLC();
        //    else
        //        GetTextboxValFromPLC();
        //}

        private void GetTextboxValFromPLC()
        {
            if (_textBoxList.Count == 0)
                return;

            foreach (var textboxItem in _textBoxList)
            {
                int val = (int)textboxItem.Tag;
                int offsetVal = 0;
                int offsetCollCount = SegOffsetColl.Count;
                string oldValue = SegTextColl[val];
                string newValue = string.Empty;

                if (offsetCollCount == 0)
                    return;

                if (val < SegOffsetColl.Count)
                    offsetVal = SegOffsetColl[val];

                PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

                // If you're editing time based row continue here
                if (TwoOffsetFieldIdNumbers.Contains(RowHeaderId))
                {
                    string twoValue = string.Empty;

                    for (int j = 0; j < 2; j++)
                    {
                        string[] segTimeVals = Regex.Matches(textboxItem.Text, @"[a-zA-Z]+|\d+")
                                                    .Cast<Match>()
                                                    .Select(m => m.Value)
                                                    .ToArray();

                        SiemensTagConfiguration siemensTagConfig = new SiemensTagConfiguration
                        {
                            DBNumber = RecipeDbNumber,
                            DataType = "string[8]",
                            PlcId = PlcDeviceId,
                            Offset = offsetVal
                        };

                        twoValue += plcCommandManager.Get<string>(siemensTagConfig, false);

                        offsetVal += RecipeOffsetLength;
                    }

                    textboxItem.Text = twoValue;
                    return;
                }

                SiemensTagConfiguration siemensTagConfiguration = new SiemensTagConfiguration
                {
                    DBNumber = RecipeDbNumber,
                    DataType = "string[8]",
                    PlcId = PlcDeviceId,
                    Offset = offsetVal
                };

                Random random = new Random();

                //_textBox.Text = plcCommandManager.Get<string>(siemensTagConfiguration, false);
                textboxItem.Text = random.Next(0, 10).ToString();
            }
        }

        private void GetComboBoxValFromPLC()
        {
            if (_comboBoxList.Count == 0)
                return;

            foreach (var comboBoxItem in _comboBoxList)
            {
                string selectedItemValue = comboBoxItem.SelectedItem.ToString();
                int val = (int)comboBoxItem.Tag;
                int offsetVal = 0;
                int offsetCollCount = SegOffsetColl.Count;
                string oldValue = _comboBoxOldValue;

                if (offsetCollCount == 0)
                    return;

                if (val < SegOffsetColl.Count)
                    offsetVal = SegOffsetColl[val];

                PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

                SiemensTagConfiguration siemensTagConfiguration = new SiemensTagConfiguration
                {
                    DBNumber = RecipeDbNumber,
                    DataType = "string[8]",
                    PlcId = PlcDeviceId,
                    Offset = offsetVal
                };

                string comboboxVal = plcCommandManager.Get<string>(siemensTagConfiguration, false);
                comboBoxItem.SelectedItem = comboboxVal;
            }
        }

        private Control GetControlForSegText(string bindingName)
        {
            string result = Regex.Replace(bindingName, @"[^\d]", "");
            int index = Convert.ToInt32(result);

            if (columnType == "textbox")
            {
                var textBox = new TextBox();
                textBox.Padding = new Thickness(5, 0, 0, 10);

                textBox.Style = TextBoxStyle;

                textBox.Tag = index;
                textBox.KeyDown += TextBox_KeyDown;
                textBox.TextChanged += TextBox_TextChanged;
                textBox.GotFocus += TextBox_GotFocus;
                Binding binding = new Binding(bindingName);
                binding.Source = this;
                textBox.SetBinding(TextBox.TextProperty, binding);

                if (ValueChangedStates.Count > index)
                {
                    if (ValueChangedStates[index])
                    {
                        // Change color of the textbox to indicate value has changed
                        textBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#F1F5D4"); // Light yellow
                        textBox.CaretBrush = Brushes.Black;

                        // Show original value on the tooltip
                        string originalValue = OriginalRecipeValues[index];
                        if (string.IsNullOrEmpty(originalValue))
                            originalValue = "yok";

                        textBox.ToolTip = $"Orijinal değer: {originalValue}";
                    }
                }
                return textBox;
            }
            else if (columnType == "combobox")
            {
                var comboBox = new ComboBox();
                comboBox.Style = ComboBoxStyle;

                if(IsComboBoxTight)
                    comboBox.ItemsSource = TightComboboxList;
                else
                    comboBox.ItemsSource = ComboboxList;

                comboBox.Tag = index;
                comboBox.SelectionChanged += ComboBox_SelectionChanged;
                comboBox.DropDownOpened += ComboBox_DropDownOpened;

                Binding binding = new Binding(bindingName);
                binding.Source = this;
                comboBox.SetBinding(ComboBox.SelectedValueProperty, binding);

                if (ValueChangedStates.Count > index)
                {
                    if (ValueChangedStates[index])
                    {
                        // Change style of the combobox to indicate value has changed
                        comboBox.Style = ComboBoxValueChangedStyle;

                        // Show original value on the tooltip
                        string originalValue = OriginalRecipeValues[index];
                        if (string.IsNullOrEmpty(originalValue))
                            originalValue = "yok";

                        comboBox.ToolTip = $"Orijinal değer: {originalValue}";
                    }
                }

                return comboBox;
            }
            return null;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            int segIndex = (int)textBox.Tag;
            if (segIndex < SegTextColl.Count)
                _textBoxOldValue = SegTextColl[segIndex];
        }

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            int segIndex = (int)comboBox.Tag;
            if (segIndex < SegTextColl.Count)
                _comboBoxOldValue = SegTextColl[segIndex];
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_preventComboBoxChangedEvent)
            {
                _preventComboBoxChangedEvent = false;
                return;
            }

            ComboBox comboBox = (ComboBox)sender;

            // Prevent setting value to PLC on load
            if (!comboBox.IsLoaded)
                return;

            string selectedItemValue = comboBox.SelectedItem.ToString();
            int segIndex = (int)comboBox.Tag;
            int offsetVal = 0;
            int offsetCollCount = SegOffsetColl.Count;
            string oldValue = _comboBoxOldValue;

            if (RecipeEditorVM != null)
                RecipeEditorVM.IsRecipeTableDataChanged.Value = true;

            if (offsetCollCount == 0)
                return;

            var setResult = WinUIMessageBox.Show("Seçtiğiniz değeri göndermek istediğinize emin misiniz?", "Uyarı",
                          MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (setResult == MessageBoxResult.No)
            {
                _preventComboBoxChangedEvent = true;
                comboBox.SelectedItem = _comboBoxOldValue;
                return;
            }

            if (segIndex < SegOffsetColl.Count)
                offsetVal = SegOffsetColl[segIndex];

            ActiveRecipeVM.WaitIndicatorControl.IsWaitIndicatorVisible = true;

            PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

            SiemensTagConfiguration siemensTagConfiguration = new SiemensTagConfiguration
            {
                DBNumber = RecipeDbNumber,
                DataType = "string[8]",
                PlcId = PlcDeviceId,
                Offset = offsetVal
            };

            Guid guid = Guid.NewGuid();

            plcCommandManager.Set(siemensTagConfiguration, SegTextColl[segIndex], guid);

            bool plcResult = await plcCommandManager.IsUpdatedResultAsync(guid, false);

            ActiveRecipeVM.WaitIndicatorControl.IsWaitIndicatorVisible = false;

            if (plcResult == false)
            {
                WinUIMessageBox.Show("Seçtiğiniz değer PLC'ye setlenemedi lütfen servislerinizi kontrol ediniz.", "Başarısız",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                _preventComboBoxChangedEvent = true;
                comboBox.SelectedItem = _comboBoxOldValue;
                return; // Do not insert process info if setting value to PLC failed.
            }

            // Change style of the combobox to indicate value has changed
            comboBox.Style = ComboBoxValueChangedStyle;

            // Show original value on the tooltip
            string originalValue = OriginalRecipeValues[segIndex];
            if (string.IsNullOrEmpty(originalValue))
                originalValue = "yok";

            comboBox.ToolTip = $"Orijinal değer: {originalValue}";

            // Save process event log to DB
            int segNo = segIndex + 1;
            if (string.IsNullOrEmpty(oldValue))
                oldValue = "Empty";

            if (string.IsNullOrEmpty(selectedItemValue))
                selectedItemValue = "Empty";

            SaveActiveRecipeProcessEvent(segNo, RowHeader, oldValue, selectedItemValue);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (!textBox.IsLoaded)
                return;

            var change = e.Changes.FirstOrDefault();

            if (TwoOffsetFieldIdNumbers.Contains(RowHeaderId))
            {

                Regex regex = new Regex(ProcessManager.Instance.ApplicationProperties["RecipeTimeFormatRegex"].Value);

                if (RowHeaderId != 2)
                    regex = new Regex(ProcessManager.Instance.ApplicationProperties["RecipeTimeFormatGraceRegex"].Value);

                Match match = regex.Match(textBox.Text);

                if (!match.Success)
                {
                    textBox.Text = textBox.Text.Remove(change.Offset, change.AddedLength);
                }

                // Check if textbox contains a space.
                if (Regex.IsMatch(textBox.Text, @"\s"))
                {
                    // Delete the space.
                    textBox.Text = textBox.Text.Replace(" ", "");
                }

                // If this specific letter is lowercase, then make it uppercase.
                if (match.Success)
                {
                    textBox.Text = textBox.Text.ToUpper();
                }

                // Set position of the caret at end of the text or before the last letter.
                char specificChar = '0';

                if (textBox.Text.Length > 0)
                    specificChar = textBox.Text.ToCharArray()[textBox.Text.Length - 1];

                //todo:h check....
                if (ProcessManager.Instance.ApplicationProperties["RecipeTimeFormatSpecificChars"].Value.ToCharArray().Select(x => x).Contains(specificChar))
                    textBox.CaretIndex = textBox.Text.Length - 1;
                else
                    textBox.CaretIndex = textBox.Text.Length;


                /*
                 * 
                  if (specificChar.Equals('M') || specificChar.Equals('H'))
                    textBox.CaretIndex = textBox.Text.Length - 1;
                else
                    textBox.CaretIndex = textBox.Text.Length;
                 
                 */

            }
            else
            {
                // Allow entering either numbers or "X" letter for this items. Not both.
                Regex regex = new Regex(@"^([Xx]{0,1}|[.,0-9]{0,15})$");

                //"Min Mon Vacuum", "Max Mon. Press", "Vacuum Value", "Vacuum Value 2", "-W1 Criterion Val.", "-W2 Criterion Val."
                if (RowHeaderId == 36 || RowHeaderId == 34 || RowHeaderId == 9 || RowHeaderId == 47 || RowHeaderId == 12 || RowHeaderId == 16)
                    regex = new Regex(@"^([Xx]{0,1}|[-]{0,1}[.,0-9]{0,15})$");
                else if (RowHeaderId == 6) // Cascade Ratio
                    // Allow entering either numbers or "A" letter for this item. Not both.
                    regex = new Regex(@"^([AaXx]{0,1}|[0-9]+)$");
                else if (RowHeaderId == 5 || RowHeaderId == 8) // Temp Value and Pressure Value
                    regex = new Regex(@"^([Cc]{0,1}[Uu]{0,1}[Rr]{0,2}|[.,0-9]{1,6})$");

                Match match = regex.Match(textBox.Text);

                if (!match.Success)
                {
                    textBox.Text = textBox.Text.Remove(change.Offset, change.AddedLength);
                }

                // If text contains a coma then change it with dot.
                if (textBox.Text.Contains(','))
                {
                    textBox.Text = textBox.Text.Replace(',', '.');
                }

                // If this specific letter is lowercase, then make it uppercase.
                if (match.Success)
                {
                    textBox.Text = textBox.Text.ToUpper();
                }

                // Set position of caret end of the text.
                textBox.CaretIndex = textBox.Text.Length;
            }

            if (RecipeEditorVM != null)
                RecipeEditorVM.IsRecipeTableDataChanged.Value = true;
        }

        private async void SetTwoOffsetValue(TextBox textBox, int offsetVal, short segNo, int segIndex, string oldValue)
        {
            PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
            bool plcResult = false;

            for (short j = 0; j < 2; j++)
            {
                string[] segTimeVals = Regex.Matches(textBox.Text, @"[a-zA-Z]+|\d+")
                                            .Cast<Match>()
                                            .Select(m => m.Value)
                                            .ToArray();

                if (textBox.Text == string.Empty)
                {
                    segTimeVals = new string[2] { string.Empty, string.Empty };
                }
                else if (textBox.Text == "X")
                {
                    segTimeVals = new string[2] { "X", string.Empty };
                }

                SiemensTagConfiguration siemensTagConfig = new SiemensTagConfiguration
                {
                    DBNumber = RecipeDbNumber,
                    DataType = "string[8]",
                    PlcId = PlcDeviceId,
                    Offset = offsetVal
                };

                if (segTimeVals.Count() != 2)
                {
                    ActiveRecipeVM.WaitIndicatorControl.IsWaitIndicatorVisible = false;
                    WinUIMessageBox.Show("Lütfen değer ve zaman dilimi formatında gönderiniz.", "Uyarı",
                        MessageBoxButton.OK, MessageBoxImage.Warning);

                    textBox.Text = _textBoxOldValue;
                    return;
                }

                Guid guid = Guid.NewGuid();
                plcCommandManager.Set(siemensTagConfig, segTimeVals[j], guid);
                plcResult = await plcCommandManager.IsUpdatedResultAsync(guid, false);
                if (plcResult == false && j == 1)
                {
                    ActiveRecipeVM.WaitIndicatorControl.IsWaitIndicatorVisible = false;
                    WinUIMessageBox.Show("Gönderdiğiniz değer PLC'ye setlenemedi lütfen servislerinizi kontrol ediniz.", "Başarısız",
                                            MessageBoxButton.OK, MessageBoxImage.Error);

                    textBox.Text = _textBoxOldValue;
                    return; // Do not insert process info if setting value to PLC failed.
                }

                offsetVal += RecipeOffsetLength;
            }

            if (plcResult)
            {
                textBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#F1F5D4"); // Light yellow
                textBox.CaretBrush = Brushes.Black;

                // Show original value on the tooltip
                string originalValue = OriginalRecipeValues[segIndex];
                if (string.IsNullOrEmpty(originalValue))
                    originalValue = "yok";

                textBox.ToolTip = $"Orijinal değer: {originalValue}";
            }

            // Save process event log to DB
            segNo = (short)(segIndex + 1);
            if (string.IsNullOrEmpty(oldValue))
                oldValue = "Empty";

            SaveActiveRecipeProcessEvent(segNo, RowHeader, oldValue, textBox.Text);
            ActiveRecipeVM.WaitIndicatorControl.IsWaitIndicatorVisible = false;
        }

        private async void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (e.Key == Key.Enter)
            {
                int segIndex = (int)textBox.Tag;
                int offsetVal = 0;
                int offsetCollCount = SegOffsetColl.Count;
                string oldValue = SegTextColl[segIndex];
                string newValue = string.Empty;
                short segNo = 0;
                bool plcResult = false;
                Guid guid;

                if (offsetCollCount == 0)
                    return;

                var setResult = WinUIMessageBox.Show("Girdiğiniz değeri göndermek istediğinize emin misiniz?.", "Uyarı",
                                      MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (setResult == MessageBoxResult.No)
                {
                    textBox.Text = _textBoxOldValue;
                    return;
                }

                if (segIndex < SegOffsetColl.Count)
                    offsetVal = SegOffsetColl[segIndex];

                ActiveRecipeVM.WaitIndicatorControl.IsWaitIndicatorVisible = true;
                PlcCommandManager plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);

                // If you're editing two offset based row continue here
                if (TwoOffsetFieldIdNumbers.Contains(RowHeaderId))
                {
                    SetTwoOffsetValue(textBox, offsetVal, segNo, segIndex, oldValue);
                    return;
                }

                // Check Temp Value and Pressure Value header 
                if (RowHeaderId == 5 || RowHeaderId == 8)
                {
                    char[] currLetters = new char[] { 'C', 'U', 'R' };
                    foreach (var letter in currLetters)
                    {
                        if (textBox.Text.Contains(letter) && !textBox.Text.Equals("CURR"))
                        {
                            ActiveRecipeVM.WaitIndicatorControl.IsWaitIndicatorVisible = false;
                            WinUIMessageBox.Show("Lütfen 'CURR' değerini doğru yazdığınızdan emin olunuz.", "Uyarı",
                                MessageBoxButton.OK, MessageBoxImage.Warning);

                            textBox.Text = _textBoxOldValue;
                            return;
                        }
                    }
                }

                SiemensTagConfiguration siemensTagConfiguration = new SiemensTagConfiguration
                {
                    DBNumber = RecipeDbNumber,
                    DataType = "string[8]",
                    PlcId = PlcDeviceId,
                    Offset = offsetVal
                };

                guid = Guid.NewGuid();
                plcCommandManager.Set(siemensTagConfiguration, textBox.Text, guid);
                plcResult = await plcCommandManager.IsUpdatedResultAsync(guid, false);

                if (plcResult == false)
                {
                    ActiveRecipeVM.WaitIndicatorControl.IsWaitIndicatorVisible = false;
                    WinUIMessageBox.Show("Gönderdiğiniz değer PLC'ye setlenemedi lütfen servislerinizi kontrol ediniz.", "Başarısız",
                                        MessageBoxButton.OK, MessageBoxImage.Error);

                    textBox.Text = _textBoxOldValue;
                    return; // Do not insert process info if setting value to PLC failed.
                }

                // Change color of the textbox to indicate value has changed
                textBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#F1F5D4"); // Light yellow
                textBox.CaretBrush = Brushes.Black;

                // Show original value on the tooltip
                string originalValue = OriginalRecipeValues[segIndex];
                if (string.IsNullOrEmpty(originalValue))
                    originalValue = "yok";

                textBox.ToolTip = $"Orijinal değer: {originalValue}";

                // Save process event log to DB
                segNo = (short)(segIndex + 1);
                if (string.IsNullOrEmpty(oldValue))
                    oldValue = "Empty";

                newValue = textBox.Text;
                if (string.IsNullOrEmpty(newValue))
                    newValue = "Empty";

                SaveActiveRecipeProcessEvent(segNo, RowHeader, oldValue, newValue);
                ActiveRecipeVM.WaitIndicatorControl.IsWaitIndicatorVisible = false;
            }
        }

        private void SaveActiveRecipeProcessEvent(int segNo, string rowTitle, string oldValue, string newValue)
        {
            string _connectionString = ApplicationConfigurations.Instance.Configuration.PostgreSqlConnectionString;
            ProcessEventLogService processEventLogService = new ProcessEventLogService(_connectionString);
            ProcessEventLog recipeProcessEventLog = new ProcessEventLog();

            if (ProcessManager.Instance.CurrentProcess.IsBatchLoaded)
                recipeProcessEventLog.BatchId = ProcessManager.Instance.CurrentProcess.BatchId;

            recipeProcessEventLog.CreateDate = DateTime.Now;
            recipeProcessEventLog.Type = ProcessEventLogType.Manual.ToString();
            recipeProcessEventLog.EventText = string.Format("Active Recipe segment no: {0}, row title: {1}, old value: {2}, new value: {3}",
                                                        segNo, rowTitle, oldValue, newValue);
            recipeProcessEventLog.ModifiedByUserId = ActiveRecipeVM.ActiveUser.id;
            processEventLogService.Insert(recipeProcessEventLog);


            var fromToDirection = ApplicationConfigurations.Instance.Configuration.WorkingEnvironment == WorkingEnvironment.pc ? FromToDirection.PCtoServer : FromToDirection.ServerToPC;
            ProcessEventLogAdapter processEventLogAdapter = new ProcessEventLogAdapter(ApplicationConfigurations.Instance.Configuration.RedisServer);
            processEventLogAdapter.CreateProcessEventLogSyncIssue(recipeProcessEventLog, fromToDirection, ApplicationConfigurations.Instance.Configuration.PlcDevice.Id);
        }

        private Control GetControlForTableName()
        {
            var label = new Label();
            label.Padding = new Thickness(5, 2, 0, 0);
            label.Margin = new Thickness(0, 0, 3, 0);
            label.FontWeight = FontWeights.Bold;
            label.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(HeaderBrushString);

            Binding binding = new Binding("RowHeader");
            binding.Source = this;
            label.SetBinding(Label.ContentProperty, binding);
            return label;
        }

        private Control GetControlForUnitName()
        {
            var label = new Label();
            label.Padding = new Thickness(5, 2, 0, 0);
            label.Margin = new Thickness(0, 0, 3, 0);
            label.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#810201"); /// Dark Red Color
            Binding binding = new Binding("UnitName");
            binding.Source = this;
            label.SetBinding(Label.ContentProperty, binding);
            return label;
        }
    }
}