using GalaSoft.MvvmLight.Messaging;
using SimpleCPUMiner.Messages;
using SimpleCPUMiner.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCPUMiner.Miners.Stratum
{
    public class SMTcpClient
    {
        public int ErrorCount { get; set; }
        public int ID { get; set; }
        public PoolSettingsXmlUI Pool { get; set; }
        private TcpClient _client;
        private IPHostEntry ip;

        public SMTcpClient()
        {
        }

        internal void Connect()
        {
            if (ip == null)
                ip = Dns.GetHostEntry(Pool.URL);

            if(ip != null && ip.AddressList.Length > 0)
                _client = new TcpClient(ip.AddressList[0].ToString(), Pool.Port);
            else
                _client = new TcpClient(Pool.URL, Pool.Port);

            if (ID>-1)
                Messenger.Default.Send(new MinerOutputMessage() { OutputText = $"Connecting to {Pool.URL}:{Pool.Port}" });

            Messenger.Default.Send(new ActivePoolMessage() { IsActiveCPUPool = false, IsActiveGPUPool = true, URL = Pool.URL, Port = Pool.Port });

            _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        }

        internal void Close()
        {
            if(_client != null)
                _client.Close();
        }

        internal NetworkStream GetStream()
        {
            return _client.GetStream();
        }
    }
}
