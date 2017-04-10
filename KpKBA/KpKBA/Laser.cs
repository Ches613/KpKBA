using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Scada.Comm.Devices
{
    class Laser
    {
        TcpClient client;

        public Laser(TcpClient clientForLaserComm) {

            this.client = clientForLaserComm;
        }


        public long reqActualNum(byte numUM) {

            long temp = 0;
            byte[] readBuff = new byte[1024];

            int beginUm = 8;
            int endUm = 0;
            int dataCount = 0;

            Cmds cmds = new Cmds();
           using( NetworkStream laserStream = client.GetStream())
            {

                foreach (byte b in Cmds.getActualUmCmd(numUM))
                laserStream.WriteByte(b);


                laserStream.Read(readBuff, 0, readBuff.Length);

                if (readBuff[1] == 0x04 && readBuff[2] == 0x9d) {

                    dataCount = readBuff[4];
                    endUm = dataCount - 4;

                    string myString = System.Text.Encoding.ASCII.GetString(readBuff, beginUm, endUm);

                    temp = Convert.ToInt64(myString);

                }

            } 

           
            return temp;
        }


        private class Cmds {

            private static byte[] NUMBER_ACTUAL_MESSAGE_STAT =              // Шаблон для запроса User message. Для запроса требуется замена 
                                     { 0x02, 0x04, 0x9D, 0x01,          // 7-го элемента массива на номер запрашиваемого сообщения      
                                        0x02, 0x00, 0x02, 0x01, 0x03 }; // По умолчанию запрашивает первое сообщение        

            public static byte[] GET_STATUS_STAT = { 0x2, 0x2, 0x70, 0x00, 0x3 }; // Запрос статуса принтера

            public static byte startParcel = 0x02; // байт обозначающий начало посылки
            public static byte endParcel = 0x03; // байт обозначающий конец посылки       

            // 16 byte состояние печати
            public static byte PRINTING_WAITING_SIGNAL_MODE = 0x01; // 0x01 : System is in printing mode (waiting for photocell/PLC signal), but is actually not printing.
            public static byte ACTUALLY_PRINTING_MODE = 0x03; // 0x03: System is in printing mode and is actually printing.
            public static byte NOT_PRINTING_MODE = 0x0; //0x00: System is not in the printing mode

             //25 � 26: alarm (byte switched); codifies the alarm status of the system.
            public static byte[] NOALARMS = { 0x0, 0x0 }; // 0x0000: no alarms
            public static byte[] WRONGMESSAGPORT = { 0x0E, 0x0C }; // 0x0C0E: wrong messageport (no file found for the external message selection)
            public static byte[] ALARMSACTIVE = { 0x48, 0x8 }; // 0x0848: alarms active; some alarms are active (interlock, shutter,�.)
            public static byte[] HARDWAERFAULURE = { 0xFF, 0xFF }; //0xFFFF: initialization of ScanLinux has failed due to some hardware failure.


            public static byte[] getActualUmCmd(byte numUM)
            {

                byte[] retVal = NUMBER_ACTUAL_MESSAGE_STAT;
                retVal[7] = numUM;
                return retVal;
            }
        }
    }
}
