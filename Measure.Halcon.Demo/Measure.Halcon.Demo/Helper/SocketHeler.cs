using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;


namespace Measure.Halcon.Demo.Helper
{
    internal class SocketHeler
    {


        private Socket _socket;

        private bool _isSend;

        private LogHelper _log;

        private List<string> _sendData;
        public SocketHeler(Socket socket, LogHelper log)
        {
            _socket = socket;
            this._log = log;
        }

        public OperationResult StartSockeServer()
        {
            try
            {
                _socket.Bind(new IPEndPoint(IPAddress.Any, 5000));
                _socket.Listen(1);
                Task.Factory.StartNew(
                    () => Pross());
                return OperationResult.CreateSuccessResult("通信初始化成功");
            }
            catch (Exception ex)
            {

                return OperationResult.CreateFailResult("通信初始化失败", ex);
            }


        }


        public void SendData(List<string> list)
        {
            _isSend = true;
            _sendData = list;
        }

        private void Pross()
        {
            while (true)
            {
                Socket conn = _socket.Accept();
                try
                {
                    while (true)
                    {
                        if (_isSend)
                        {
                            foreach (var item in _sendData)
                            {
                                conn.Send(Encoding.ASCII.GetBytes(item));
                                conn.Send(Encoding.ASCII.GetBytes(","));
                            }
                            _isSend = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Result(OperationResult.CreateFailResult("通信线程异常退出", ex));

                }
                finally
                {
                    conn.Close();
                }
            }


        }
    }
}
