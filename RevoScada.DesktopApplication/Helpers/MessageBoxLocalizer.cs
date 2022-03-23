using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Helpers
{
    public class MessageBoxLocalizer : DXMessageBoxLocalizer
    {
        public bool BoolValue;

        protected override void PopulateStringTable()
        {
            base.PopulateStringTable();

            if(BoolValue)
            {
                AddString(DXMessageBoxStringId.Yes, "Ok");
                AddString(DXMessageBoxStringId.No, "Ignore");
            } else
            {
                AddString(DXMessageBoxStringId.Yes, "Yes");
                AddString(DXMessageBoxStringId.No, "No");
            }
        }
    }
}
