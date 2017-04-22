using System;
using System.Net.Sockets;

namespace Scada.Comm.Devices
{

    /// <summary>
    /// Класс общения с лазером
    /// </summary>
    class Laser
    {

        private TcpClient client;
        private string hostName;
        private int port;
        private int timeoutReq = 3000; // таймаут ожидания ответа на TCP запрос 
        private Cmds cmds = new Cmds();
        private byte[] readBuff = new byte[64];
        private bool isConnected;
               

        /// <summary>
        /// Конструктор с временем ождания ответа по умолчанию (1 сек.)
        /// </summary>
        public Laser(string hostName, int port) {

           
            this.hostName = hostName;
            this.port = port;
                                  
            isConnected = Connect();
          
           
        }

      
        /// <summary>
        /// Подключение к лазеру. 
        /// Если подключение прошло успешно либо подключение активно возвращает true,
       /// в противном случае false.
        /// </summary>
        private bool Connect() {

            if (!isConnected)
            {

                try {
                  
                    client = new TcpClient();
                    client.Connect(hostName, port);                   
                    client.ReceiveTimeout = timeoutReq;
                    client.SendTimeout = timeoutReq;
                    isConnected = true;
                   

                }
                catch(Exception e)
                {

                   if(isConnected)
                    client.Close();
                    
                    isConnected = false;
                   
                   
                }
            }

            return isConnected;
        }

       
        private bool Send(byte[] parsel) {

            bool sent = false;
           
                try
                {

                    if(!isConnected)
                   isConnected = Connect();


                if (isConnected)
                {
                    client.Client.Send(parsel);
                    sent = true;
                    isConnected = true;
                }

                }
                catch (SocketException e)
                {

               
                isConnected = false;
                sent = false;

                    
                }

            return sent;
        }

        private bool Receive(ref byte[] buff) {

            bool rec = false;
                try
                {

                    if(!isConnected)
                   isConnected = Connect();

                if (isConnected)
                {
                    client.Client.Receive(buff);
                    rec = true;
                    isConnected = true;
                }

                }
                catch (SocketException e)
                {

                
                isConnected = false;
                rec = false;
                   

                   
                }

            return rec;

        }

        /// <summary>
        /// запрос актуального номера рулока. 
        /// в качестве аргумента задается номер пользовательского сообщения
        /// </summary>
        public double reqActualNum(byte numUM) {

            bool isRec = false;
            double d = 0;
           int beginUm = 0;
            int dataCount = 0;


                    
                Send(Cmds.getActualUmCmd(numUM));



            isRec = Receive(ref readBuff);

                if(!(readBuff[2] == 0x9d))
                isRec = Receive(ref readBuff);
                
           

               
            

            if (readBuff[1] == 0x04 && readBuff[2] == 0x9d) {

                dataCount = readBuff[4] - 2;
                beginUm = 8;
            }

            byte[] b = new byte[dataCount];

            for (int i = 0; i < dataCount; i++)
                b[i] = readBuff[beginUm + i];

            string s = System.Text.Encoding.UTF8.GetString(b);

           
            if(isRec)
            d = Convert.ToDouble(s);

           

            return d;
        }

        public StatusPack getStatus() {

            StatusPack stausPack = new StatusPack();

            Send(Cmds.GET_STATUS_STAT);


            Receive(ref readBuff);

            if (!(readBuff[2] == 0x70))
           Receive(ref readBuff);
            

            if (readBuff[2] == 0x70 && readBuff[3] == 0x00)
            {
                stausPack.okPrintCount = BitConverter.ToInt32(readBuff, 4);
                stausPack.printCount = BitConverter.ToInt32(readBuff, 8);
                
                switch (readBuff[19]) {
                    
                    case Cmds.NOT_PRINTING_MODE:

                        stausPack.isPrinting = false;
                        stausPack.printIsStarted = false;

                        break;

                    case Cmds.PRINTING_WAITING_SIGNAL_MODE:

                        stausPack.isPrinting = false;
                        stausPack.printIsStarted = true;

                        break;

                    case Cmds.ACTUALLY_PRINTING_MODE:

                        stausPack.isPrinting = true;
                        stausPack.printIsStarted = true;

                        break;
                }

                if ((readBuff[28] == Cmds.ALARMSACTIVE) || (readBuff[28] == Cmds.HARDWAERFAULURE))
                {

                    stausPack.isAlarm = true;
                    stausPack.alarmCode = readBuff[30];

                }
                else {

                    stausPack.isAlarm = false;
                }

            }

         

            return stausPack;
        }


        


        private class Cmds {

            private static byte[] NUMBER_ACTUAL_MESSAGE_STAT =              // Шаблон для запроса User message. Для запроса требуется замена 
                                     { 0x02, 0x04, 0x9D, 0x01,          // 7-го элемента массива на номер запрашиваемого сообщения      
                                        0x02, 0x00, 0x02, 0x01, 0x03 }; // По умолчанию запрашивает первое сообщение        

            public static byte[] GET_STATUS_STAT = { 0x2, 0x2, 0x70, 0x00, 0x3 }; // Запрос статуса принтера

            public const byte startParcel = 0x02; // байт обозначающий начало посылки
            public const byte endParcel = 0x03; // байт обозначающий конец посылки       

            // 16 byte состояние печати
            public const byte PRINTING_WAITING_SIGNAL_MODE = 0x01; // 0x01 : System is in printing mode (waiting for photocell/PLC signal), but is actually not printing.
            public const byte ACTUALLY_PRINTING_MODE = 0x03; // 0x03: System is in printing mode and is actually printing.
            public const byte NOT_PRINTING_MODE = 0x0; //0x00: System is not in the printing mode

             //25 � 26: alarm (byte switched); codifies the alarm status of the system.
            public const byte NOALARMS = 0x0; // 0x0000: no alarms
          //  public static byte[] WRONGMESSAGPORT = { 0x0E, 0x0C }; // 0x0C0E: wrong messageport (no file found for the external message selection)
            public const byte ALARMSACTIVE =  0x48; // 0x0848: alarms active; some alarms are active (interlock, shutter,�.)
           public const byte HARDWAERFAULURE = 0xFF; //0xFFFF: initialization of ScanLinux has failed due to some hardware failure.


            public static byte[] getActualUmCmd(byte numUM)
            {

                byte[] retVal = NUMBER_ACTUAL_MESSAGE_STAT;
                retVal[7] = numUM;
                return retVal;
            }
        }
    }

    public class StatusPack
    {

        public StatusPack() {

            printIsStarted = false;
            isPrinting = false;
            isAlarm = false;
            alarmCode = 0x0;
            okPrintCount = 0;
            printCount = 0;
        }

        public bool printIsStarted { get; set;}
        public bool isPrinting { get; set; }
        public bool isAlarm { get; set; }
        public byte alarmCode { get; set; }
        public int okPrintCount { get; set; }
        public int printCount { get; set; }
    

    }
}
