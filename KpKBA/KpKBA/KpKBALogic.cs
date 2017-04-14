/*
 * Copyright 2016 Mikhail Shiryaev
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * 
 * Product  : Rapid SCADA
 * Module   : KpKBA
 * Summary  : Device communication logic
 * 
 * Author   : Mikhail Zverkov
 * Created  : 2017
 * Modified : 2017
 * 
 * Description
 * KBA laser system communication notifications.
 */


using Scada.Comm.Devices.KpKBA;
using Scada.Data.Models;
using Scada.Data.Tables;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Threading;

namespace Scada.Comm.Devices
{
    /// <summary>
    /// Device communication logic
    /// <para>Логика работы КП</para>
    /// </summary>
    public class KpKBALogic : KPLogic
    {
       
        private Config config;              // конфигурация соединения с KBA system
        private TcpClient tcpClient;      // клиент TCP IP
        private Laser laser;
        private bool fatalError;            // фатальная ошибка при инициализации КП
        private string state;               // состояние КП
        private bool writeState;            // вывести состояние КП


        /// <summary>
        /// Конструктор
        /// </summary>
        public KpKBALogic(int number)
            : base(number)
        {
            CanSendCmd = true;
            ConnRequired = false;
            WorkState = WorkStates.Normal;
            
            config = new Config();
            tcpClient = new TcpClient();
            fatalError = false;
            state = "";
            writeState = false;

            InitKPTags(new List<KPTag>()
            {
                new KPTag(0, Localization.UseRussian ? "---" : "---"),
                new KPTag(1, Localization.UseRussian ? "Актуальный номер рулона по первому ручь" : "Actual roll number UM1"),
                 new KPTag(2, Localization.UseRussian ? "Актуальный номер рулона по второму ручь" : "Actual roll number UM2"),
                  new KPTag(3, Localization.UseRussian ? "Счетчик печати" : "Print counter"),
                  new KPTag(4, Localization.UseRussian ? "Счетчик печати Ok" : "Print counter Ok"),
                   new KPTag(5, Localization.UseRussian ? "Печать активна" : "Print active"),
                   new KPTag(6, Localization.UseRussian ? "Печатает" : "Printing"),
                   new KPTag(7, Localization.UseRussian ? "Предуприждение" : "Alarm"),
                    new KPTag(8, Localization.UseRussian ? "Код предуприждения" : "Alarm code")

            });
        }


        /// <summary>
        /// Загрузить конфигурацию соединения с KBA system
        /// </summary>
        private void LoadConfig()
        {
            string errMsg;
            fatalError = !config.Load(Config.GetFileName(AppDirs.ConfigDir, Number), out errMsg);

            if (fatalError)
            {
                state = Localization.UseRussian ? 
                    "соедининие с KBA невозможна" : 
                    "connecting to KBA is impossible";
                throw new Exception(errMsg);
            }
            else
            {
                state = Localization.UseRussian ? 
                    "Ожидание данных..." :
                    "Waiting for data...";
            }
        }

        /// <summary>
        /// Инициализировать клиент TCP на основе конфигурации соединения
        /// </summary>
        private void InitTcpClient()
        {
            tcpClient.Connect(config.Host, config.Port);
            tcpClient.ReceiveTimeout = ReqParams.Timeout;
                        
        }
   
        /// <summary>
        /// Выполнить сеанс опроса КП
        /// </summary>
        public override void Session()
        {
            if (writeState)
            {
                WriteToLog("");
                WriteToLog(state);
                writeState = false;
            }

            SetCurData(1, laser.reqActualNum(1), 1);


            SetCurData(2, laser.reqActualNum(2), 1);

            StatusPack status = laser.getStatus();
          
            SetCurData(3, status.printCount, 1);
            SetCurData(4, status.okPrintCount, 1);
            SetCurData(5, Convert.ToDouble(status.printIsStarted), 1);
            SetCurData(6, Convert.ToDouble(status.isPrinting), 1);
               SetCurData(7, Convert.ToDouble(status.isAlarm), 1);
               SetCurData(8, Convert.ToDouble(status.alarmCode), 1);

        
        }



        /// <summary>
        /// Выполнить действия при запуске линии связи
        /// </summary>
        public override void OnCommLineStart()
        {
            writeState = true;
            LoadConfig();
            InitTcpClient();
            laser = new Laser(tcpClient);

        }

    }
}