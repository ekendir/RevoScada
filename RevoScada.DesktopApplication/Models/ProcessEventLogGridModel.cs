using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class ProcessEventLogGridModel : ObservableObject
    {
        public long id { get; set; }
        public string EventText { get; set; }
        public DateTime CreateDate { get; set; }
        public int BatchId { get; set; }
        public string Type { get; set; }
        public int ModifiedByUserId { get; set; }

        private string _color = "#FFFFFF";
        public string Color 
        { 
            get
            {
                if (EventText.StartsWith("Segment changed"))
                {
                    _color = "#0083C4"; // blue color
                    FocusedColor = "#005F8F";
                }
                else if(EventText.StartsWith("Active Recipe segment no"))
                {
                    _color = "#62C400"; // green color
                    FocusedColor = "#4A9400";
                }

                return _color;
            }
            set
            {
                _color = value;
            } 
        }

        public string FocusedColor { get; set; }
    }
}
