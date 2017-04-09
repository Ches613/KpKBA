using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scada.Comm.Devices
{
    public sealed class KpKBAView : KPView
    {
        public override string KPDescr
        {
            get
            {
                return "Библиотека КП для KBA лазера.";
            }
        }
    }
}
