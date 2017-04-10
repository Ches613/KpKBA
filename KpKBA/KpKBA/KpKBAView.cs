using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scada.Comm.Devices.KpKBA;

namespace Scada.Comm.Devices
{
    public sealed class KpKBAView : KPView
    {
        /// <summary>
        /// Конструктор для общей настройки библиотеки КП
        /// </summary>
        public KpKBAView()
            : this(0)
        {
        }

        /// <summary>
        /// Конструктор для настройки конкретного КП
        /// </summary>
        public KpKBAView(int number)
            : base(number)
        {
            CanShowProps = true;
        }


        /// <summary>
        /// Описание библиотеки КП
        /// </summary>
        public override string KPDescr
        {
            get
            {
                return "Библиотека КП для KBA лазера.";
            }
        }

        public override void ShowProps()
        {
            if (Number > 0)
                // отображение формы настройки свойств КП
                FrmConfig.ShowDialog(AppDirs, Number);
            
        }
    }
}
