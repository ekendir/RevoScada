using RevoScada.Configurator;
using RevoScada.DesktopApplication.Helpers;
using RevoScada.DesktopApplication.Views;
using RevoScada.DesktopApplication.Views.TrendViews;
using System;

namespace RevoScada.DesktopApplication.ViewModels.TrendViewModels
{
    public class TrendManagementVM
    {
        public object CurrentTrend { get; set; }

        public TrendManagementVM(WaitIndicatorControl waitIndicatorControl)
        {
            switch (ApplicationConfigurations.Instance.Configuration.Furnace.FurnaceSoftwareId)
            {
                case 20:
                    CurrentTrend = new Trend_View_Type_20();
                    var trendType20 = (Trend_View_Type_20)CurrentTrend;
                    trendType20.DataContext = new TrendVM(waitIndicatorControl, trendViewType20: trendType20);
                    break;
                default:
                    CurrentTrend = new Trend_View();
                    var trendDefault = (Trend_View)CurrentTrend;
                    trendDefault.DataContext = new TrendVM(waitIndicatorControl, trendView: trendDefault);
                    break;
            }
        }
    }
}
