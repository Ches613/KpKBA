using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scada.Comm.Devices.KpKBA;
using Scada.Data.Tables;

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
                return Localization.UseRussian ? "Библиотека КП для MACSA lasersystem. \n\n" +
                    "Тег 1 - Актуальный номер рулона по первому ручь \n" +
                    "Тег 2 - Актуальный номер рулона по второму ручь \n" +
                    "Тег 3 - Счетчик печати \n" +
                    "Тег 4 - Счетчик печати Ok \n" +
                    "Тег 5 - Печать активна \n" +
                    "Тег 6 - Печатает \n" +
                    "Тег 7 - Предуприждение \n" +
                    "Тег 8 - Код предуприждения \n" :

                    "Library Kp for TCP/IP connection to a MACSA lasersystem running ScanLinux \n\n" +
                        "Tag 1 - Actual roll number UM1 \n" +
                        "Tag 2 - Actual roll number UM2 \n" +
                        "Tag 3 - Print counter \n" +
                        "Tag 4 - Print counter Ok \n" +
                        "Tag 5 - Print active \n" +
                        "Tag 6 - Printing \n" +
                        "Tag 7 - is Alarm \n" +
                        "Tag 8 - Alarm code \n";

                    
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
