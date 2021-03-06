﻿/*
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
 * Summary  : KBA laser system server connection configuration
 * 
 * Author   : Mikhail Zverkov
 * Created  : 2017
 * Modified : 2017
 */

using System;
using System.Xml;

namespace Scada.Comm.Devices.KpKBA
{
    /// <summary>
    /// Mail server connection configuration
    /// <para>Конфигурация соединения с системами лазерной маркировки KBA</para>
    /// </summary>
    internal class Config
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public Config()
        {
            SetToDefault();
        }


        /// <summary>
        /// Получить или установить имя или IP-адрес сервера
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Получить или установить порт
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Получить или установить задержку между запросами внутри сессии сканирования в мс
        /// </summary>
        public int ReqDelay { get; set; }

        /// <summary>
        /// Получить или установить флаг проверки времени опроса 
        /// </summary>
        public bool CheckTimeSession { get; set; }


        /// <summary>
        /// Установить значения параметров конфигурации по умолчанию
        /// </summary>
        private void SetToDefault()
        {
            
            Host = "127.0.0.1";
            Port = 3490;
            CheckTimeSession = false;
            ReqDelay = 100;

        }


        /// <summary>
        /// Загрузить конфигурацию из файла
        /// </summary>
        public bool Load(string fileName, out string errMsg)
        {
            SetToDefault();

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(fileName);

                XmlElement rootElem = xmlDoc.DocumentElement;
                Host = rootElem.GetChildAsString("Host");
                Port = rootElem.GetChildAsInt("Port");
                CheckTimeSession = rootElem.GetChildAsBool("CheckTimeSession");
                ReqDelay = rootElem.GetChildAsInt("ReqDelay");
               

                errMsg = "";
                return true;
            }
            catch (Exception ex)
            {
                errMsg = CommPhrases.LoadKpSettingsError + ":" + Environment.NewLine + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Сохранить конфигурацию в файле
        /// </summary>
        public bool Save(string fileName, out string errMsg)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();

                XmlDeclaration xmlDecl = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                xmlDoc.AppendChild(xmlDecl);

                XmlElement rootElem = xmlDoc.CreateElement("KpKBAConfig");
                xmlDoc.AppendChild(rootElem);

                rootElem.AppendElem("Host", Host);
                rootElem.AppendElem("Port", Port);
                rootElem.AppendElem("CheckTimeSession", CheckTimeSession);
                rootElem.AppendElem("ReqDelay", ReqDelay);

                xmlDoc.Save(fileName);
                errMsg = "";
                return true;
            }
            catch (Exception ex)
            {
                errMsg = CommPhrases.SaveKpSettingsError + ":" + Environment.NewLine + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Получить имя файла конфигурации
        /// </summary>
        public static string GetFileName(string configDir, int kpNum)
        {
            return configDir + "KpKBA_" + CommUtils.AddZeros(kpNum, 3) + ".xml";
        }
    }
}
