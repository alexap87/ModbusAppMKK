using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Modbus.Device;

namespace ModbusAppMKK
{
    public class ModbusMy
    {

        string ipAddress;
        int tcpPort;
        string Message;
        delegate void allarmMessages(string t);
        public ModbusMy() { }
        public ModbusMy(string address, int port)
        {
            ipAddress = address;
            tcpPort = port;
            
        }
        [DllImport("WININET", CharSet = CharSet.Auto)]
        static extern bool InternetGetConnectedState(ref InternetConnectionState IpdwFlags, int dwReserved);
        enum InternetConnectionState : int
        {
            INTERNET_CONNECTION_MODEM = 0x1,
            INTERNET_CONNECTION_LAN = 0x2,
            INTERNET_C0NNECTION_PROXY = 0x4,
            INTERNET_RAS_INSTALLED = 0x10,
            INTERNET_CONNECTION_OFFLINE = 0x20,
            INTERNET_CONNECTION_CONFIGURED = 0x40
        }
        TcpClient Client;
        ModbusIpMaster master;

        DateTime dtDisconnect = new DateTime();
        DateTime dtNow = new DateTime();
        bool NetOk = false;
        private static System.Timers.Timer Timer1;

        public void btStart()
        {
            Timer1 = new Timer();
            NetOk = Connect();
            Timer1.Enabled = true;
        }
        private bool Connect()
        {
            Form1 allarm = new Form1();
            allarmMessages messa;
            messa = allarm.Alarm;
            if (master != null)
                master.Dispose();
            if (Client != null)
                Client.Close();
            if (CheckInternet())
            {
                try
                {
                    Client = new TcpClient();
                    IAsyncResult asyncResult = Client.BeginConnect(ipAddress, tcpPort, null, null);
                    asyncResult.AsyncWaitHandle.WaitOne(3000, true);
                    if (!asyncResult.IsCompleted)
                    {
                        Client.Close();
                        allarm.Alarm(DateTime.Now.ToString() + ":Cannot connect to server");
                        return false;
                    }
                    master = ModbusIpMaster.CreateIp(Client);
                    master.Transport.Retries = 0;
                    master.Transport.ReadTimeout = 1500;
                    allarm.Alarm(DateTime.Now.ToString() + ":Connect to server");
                    return true;
                }
                catch (Exception ex)
                {
                    allarm.Alarm(DateTime.Now.ToString() + ":Connect process" + ex.StackTrace + "==>" + ex.Message);
                    return false;
                }
            }
            return false;
        }
        private bool CheckInternet()
        {
            InternetConnectionState flag = InternetConnectionState.INTERNET_CONNECTION_LAN;
            return InternetGetConnectedState(ref flag, 0);
        }
        public ushort Run()
        {
                Form1 allarm = new Form1();
                ushort result = 0;
                Timer1 = new System.Timers.Timer(2000);
                Timer1.Start();
                try
                {
                    if (NetOk)
                    {

                        result = master.ReadHoldingRegisters(60, 1112, 1)[0];
                       
                    }
                    else
                    {
                        dtNow = DateTime.Now;
                        if ((dtNow - dtDisconnect) > TimeSpan.FromSeconds(10))
                        {

                        allarm.Alarm(DateTime.Now.ToString() + ":Start connecting");
                            NetOk = Connect();
                            if (!NetOk)
                            {

                            allarm.Alarm(DateTime.Now.ToString() + ":Connecting fail. Wait for retry");
                                dtDisconnect = DateTime.Now;
                            }
                        }
                        else
                        {

                        allarm.Alarm(DateTime.Now.ToString() + ":Wait for retry connecting");
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Source.Equals("System"))
                    {
                        NetOk = false;
                        Console.WriteLine(ex.Message);
                        dtDisconnect = DateTime.Now;
                    }
                }

                return result;
            
        }



    }
}