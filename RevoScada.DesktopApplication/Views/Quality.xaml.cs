using DevExpress.Xpf.Core;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.ViewModels;
using RevoScada.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for Quality.xaml
    /// </summary>
    public partial class Quality : UserControl
    {
        #region Fields
        private QualityVM _viewModel;
        public Storyboard DbResultPositiveFadeOutAnim;
        public Storyboard DbResultNegativeFadeOutAnim;
        #endregion

        public Quality()
        {
            InitializeComponent();

            DbResultPositiveFadeOutAnim = Resources["dbResultPositiveFadeOutStoryBoard"] as Storyboard;
            DbResultNegativeFadeOutAnim = Resources["dbResultNegativeFadeOutStoryBoard"] as Storyboard;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as QualityVM;
            _viewModel.QualityView = this;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1,
                            (Duration)TimeSpan.FromSeconds(1));
            this.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        private void qualityCardsListBox_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            _viewModel.AllowEditingCardSettings = false;
            _viewModel.AllowEditingPhaseSettings = false;

            // Set null to selectedPhaseCard when selected quality card changed
            _viewModel.SelectedPhaseCard = null;

            BatchQualityModel batchQualityItem = (BatchQualityModel)qualityCardsListBox.SelectedItem;
            _viewModel.SelectedQualityCard = batchQualityItem;

            if (qualityCardsListBox.SelectedItem != null)
            {
                _viewModel.IsQualityItemSelected = true;
                _viewModel.IsQualityEditButtonEnabled = true;
                _viewModel.WizardText = "Lütfen faz seçimi yapınız veya ekleyiniz.";
            } else
            {
                _viewModel.IsQualityItemSelected = true;
                _viewModel.IsQualityEditButtonEnabled = false;
            }
        }

        private void phaseCardsListBox_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            BatchQualityDetailModel batchQualityDetailItemModel = (BatchQualityDetailModel)phaseCardsListBox.SelectedItem;
            _viewModel.SelectedPhaseCard = batchQualityDetailItemModel;

            _viewModel.AllowEditingPhaseSettings = false;

            if (phaseCardsListBox.SelectedItem != null)
            {
                _viewModel.IsPhaseItemSelected = true;
                _viewModel.IsPhaseEditButtonEnabled = true;
            }
            else
            {
                _viewModel.IsPhaseEditButtonEnabled = false;
            }
        }

        public BatchQualityModel GetFormValues()
        {
            BatchQualityModel currentBatchQuality = new BatchQualityModel();
            currentBatchQuality.CardName = cardNameTextbox.Text;
            currentBatchQuality.Description = descriptionTextbox.Text;
            currentBatchQuality.LastModified = DateTime.Now;
            return currentBatchQuality;
        }

        public BatchQualityDetail GetQualityDetailValues()
        {
            BatchQualityDetail batchQualityDetail = new BatchQualityDetail();
            batchQualityDetail.PhaseName = phaseNameTb.Text;
            batchQualityDetail.PhaseChange = phaseChangeCombobox.SelectedValue?.ToString() ?? string.Empty;
            batchQualityDetail.PhaseCriteria = criteriaCombobox.SelectedValue?.ToString() ?? string.Empty;
            batchQualityDetail.PhaseCriteriaValue = string.IsNullOrEmpty(phaseCriteriaValueTb.Text) ? 0 : Convert.ToInt32(phaseCriteriaValueTb.Text);
            batchQualityDetail.PhaseStyle = phaseStyleCheckbox.IsChecked ?? false;
            batchQualityDetail.PhaseTitle = phaseTitleTb?.Text ?? string.Empty;
            batchQualityDetail.PhaseMinTime = string.IsNullOrEmpty(phaseMinTimeTb.Text) ? 0 : Convert.ToDecimal(phaseMinTimeTb.Text);
            batchQualityDetail.PhaseMaxTime = string.IsNullOrEmpty(phaseMaxTimeTb.Text) ? 0 : Convert.ToDecimal(phaseMaxTimeTb.Text);
            batchQualityDetail.ProbeStyle = probeStyleCheckbox.IsChecked ?? false;
            batchQualityDetail.ProbeTitle = probeTitleTb?.Text ?? string.Empty;
            batchQualityDetail.ProbePhaseStartMin = string.IsNullOrEmpty(ProbePhaseStartMinTb.Text) ? 0 : Convert.ToDecimal(ProbePhaseStartMinTb.Text);
            batchQualityDetail.ProbePhaseStartMax = string.IsNullOrEmpty(ProbePhaseStartMaxTb.Text) ? 0 : Convert.ToDecimal(ProbePhaseStartMaxTb.Text);
            batchQualityDetail.ProbePhaseEndMin = string.IsNullOrEmpty(ProbePhaseEndMinTb.Text) ? 0 : Convert.ToDecimal(ProbePhaseEndMinTb.Text);
            batchQualityDetail.ProbePhaseEndMax = string.IsNullOrEmpty(ProbePhaseEndMaxTb.Text) ? 0 : Convert.ToDecimal(ProbePhaseEndMaxTb.Text);

            batchQualityDetail.PressureStyle = PressureStyleCheckbox.IsChecked ?? false;
            batchQualityDetail.PressureTitle = PressureTitleTb?.Text ?? string.Empty;
            batchQualityDetail.PressurePhaseStartMin = (string.IsNullOrEmpty(PressurePhaseStartMinTb.Text) ? 0 : float.Parse(PressurePhaseStartMinTb.Text.Trim(), CultureInfo.InvariantCulture.NumberFormat));
            batchQualityDetail.PressurePhaseStartMax = string.IsNullOrEmpty(PressurePhaseStartMaxTb.Text) ? 0 : float.Parse(PressurePhaseStartMaxTb.Text.Trim(), CultureInfo.InvariantCulture.NumberFormat);
            batchQualityDetail.PressurePhaseEndMin = string.IsNullOrEmpty(PressurePhaseEndMinTb.Text) ? 0 : float.Parse(PressurePhaseEndMinTb.Text.Trim(), CultureInfo.InvariantCulture.NumberFormat);
            batchQualityDetail.PressurePhaseEndMax = string.IsNullOrEmpty(PressurePhaseEndMaxTb.Text) ? 0 : float.Parse(PressurePhaseEndMaxTb.Text.Trim(), CultureInfo.InvariantCulture.NumberFormat);
            batchQualityDetail.PressureRateMin = string.IsNullOrEmpty(PressureRateMinTb.Text) ? 0 : float.Parse(PressureRateMinTb.Text.Trim(), CultureInfo.InvariantCulture.NumberFormat);
            batchQualityDetail.PressureRateMax = string.IsNullOrEmpty(PressureRateMaxTb.Text) ? 0 : float.Parse(PressureRateMaxTb.Text.Trim(), CultureInfo.InvariantCulture.NumberFormat);

            batchQualityDetail.AirTempStyle = AirTempStyleCheckbox.IsChecked ?? false;
            batchQualityDetail.AirTempTitle = AirTempTitleTb?.Text ?? string.Empty;
            batchQualityDetail.AirTempMin = string.IsNullOrEmpty(AirTempMinTb.Text) ? 0 : Convert.ToDecimal(AirTempMinTb.Text);
            batchQualityDetail.AirTempMax = string.IsNullOrEmpty(AirTempMaxTb.Text) ? 0 : Convert.ToDecimal(AirTempMaxTb.Text);

            batchQualityDetail.PartTempStyle = PartTempStyleCheckbox.IsChecked ?? false;
            batchQualityDetail.PartTempTitle = PartTempTitleTb?.Text ?? string.Empty;
            batchQualityDetail.PartTempRateMin = string.IsNullOrEmpty(PartTempRateMinTb.Text) ? 0 : float.Parse(PartTempRateMinTb.Text.Trim(), CultureInfo.InvariantCulture.NumberFormat); //(float)Convert.ToDouble(PartTempRateMinTb.Text);
            batchQualityDetail.PartTempRateMax = string.IsNullOrEmpty(PartTempRateMaxTb.Text) ? 0 : float.Parse(PartTempRateMaxTb.Text.Trim(), CultureInfo.InvariantCulture.NumberFormat); //(float)Convert.ToDouble(PartTempRateMaxTb.Text);
            batchQualityDetail.PartTempLowRange = string.IsNullOrEmpty(PartTempLowRangeTb.Text) ? 0 : Convert.ToDecimal(PartTempLowRangeTb.Text);
            batchQualityDetail.PartTempHighRange = string.IsNullOrEmpty(PartTempHighRangeTb.Text) ? 0 : Convert.ToDecimal(PartTempHighRangeTb.Text);
            batchQualityDetail.PartTempRateCalcInterval = string.IsNullOrEmpty(PartTempRateCalcIntervalTb.Text) ? 0 : Convert.ToInt32(PartTempRateCalcIntervalTb.Text);

            return batchQualityDetail;
        }

        private void generalNumbOnlyTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            var change = e.Changes.FirstOrDefault();

            TextBox txtBox = (TextBox)sender;

            Regex regex = new Regex(@"^[-]{0,1}[0-9]{0,8}$");

            // Step 2: call Match on Regex instance.
            Match match = regex.Match(txtBox.Text);

            // Step 3: test for Success.
            if (!match.Success)
            {
                txtBox.Text = txtBox.Text.Remove(change.Offset, change.AddedLength);
            }

            // Set position of caret end of the text.
            txtBox.CaretIndex = txtBox.Text.Length;
        }

        private void vacuumSecTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            var change = e.Changes.FirstOrDefault();

            TextBox txtBox = (TextBox)sender;

            Regex regex = new Regex(@"^[-]{0,1}[0-9]{0,8}$");

            // Step 2: call Match on Regex instance.
            Match match = regex.Match(txtBox.Text);

            // Step 3: test for Success.
            if (!match.Success)
            {
                txtBox.Text = txtBox.Text.Remove(change.Offset, change.AddedLength);
            }
        }

        private void floatNumbTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            var change = e.Changes.FirstOrDefault();

            TextBox txtBox = (TextBox)sender;

            Regex regex = new Regex(@"^[-]{0,1}[,.0-9]{0,8}$");

            // If text contains a coma then change it with dot.
            if (txtBox.Text.Contains(','))
            {
                txtBox.Text = txtBox.Text.Replace(',', '.');
            }

            // Step 2: call Match on Regex instance.
            Match match = regex.Match(txtBox.Text);

            // Step 3: test for Success.
            if (!match.Success)
            {
                txtBox.Text = txtBox.Text.Remove(change.Offset, change.AddedLength);
            }

            txtBox.Text = String.Format("{0:F2}", txtBox.Text);

            // Set position of caret end of the text.
            txtBox.CaretIndex = txtBox.Text.Length;
        }

        private void generalNumbOnlyTb_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if(string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "0";
            }
        }

        private void generalNumbOnlyTb_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "0";
            }
        }

        private void newPhaseContextItem_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            _viewModel.AddPhaseCardCommand.Execute(null);
        }

        private void newQualityContextItem_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            _viewModel.AddQualityCardCommand.Execute(null);
        }
    }
}
