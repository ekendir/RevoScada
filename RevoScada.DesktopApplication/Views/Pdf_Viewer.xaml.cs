using DevExpress.Mvvm.UI;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for Pdf_Viewer.xaml
    /// </summary>
    public partial class Pdf_Viewer : Window
    {
        public Uri PdfStream { get; set; }

        public Pdf_Viewer()
        {
            InitializeComponent();

            Uri baseUri = new Uri(AppDomain.CurrentDomain.BaseDirectory); 
            PdfStream = new Uri(baseUri, "Help_Documentation.pdf");

            DataContext = this;
            viewer.AttachmentsViewerSettings = new DevExpress.Xpf.PdfViewer.PdfAttachmentsViewerSettings();
            viewer.AttachmentsViewerSettings.HideAttachmentsViewer = true;
            viewer.ThumbnailsViewerSettings = new DevExpress.Xpf.PdfViewer.PdfThumbnailsViewerSettings();
            viewer.ThumbnailsViewerSettings.HideThumbnailsViewer = true;
            viewer.ZoomMode = DevExpress.Xpf.DocumentViewer.ZoomMode.PageLevel;
        }
    }
}
