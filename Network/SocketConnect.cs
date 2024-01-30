using System;
using System.Net;
using System.Net.Sockets;

namespace SocketClient.Network
{
	public class SocketConnect
	{
        /*
        private static SocketConnect? _instance = null;

        public static SocketConnect Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SocketConnect();
                return _instance;
            }
        }
        */

        public SocketConnect()
		{
		}

        public Socket? socketClient = null;

        // 연결 요청용 SocketAsyncEventArgs 객체
        SocketAsyncEventArgs acceptArgs = new SocketAsyncEventArgs();
        // 데이터 송신용 SocketAsyncEventArgs 객체
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        // 데이터 수신용 SocketAsyncEventArgs 객체
        SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();

        public void Connect()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11200);

            socketClient = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp );
            
            // 완료 콜백 함수로 OnConnectCompleted()함수를 추가하고
            // 서버의 식별정보를 가진 IPEndPoint로 넣어줍니다.
            acceptArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnectCompleted);
            acceptArgs.RemoteEndPoint = endPoint;
                
            // 연결 요청용 SocketAsyncEventArgs 객체를 인수로 보내 비동기로 연결 요청을 합니다.
            socketClient.ConnectAsync(acceptArgs);

            Console.WriteLine("Server Connect Start!");
        }

        public void OnConnectCompleted( object? obj, SocketAsyncEventArgs args )
        {
            //Console.WriteLine("ServerConnect OnComplete!");

            if (args.SocketError == SocketError.Success)
            {
                Console.WriteLine("Server Connect Success!!!");

                //sendArgs.Completed += OnSendCompleted;
                sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

                receiveArgs.SetBuffer(new byte[1024], 0, 1024);
                receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);


                // 데이터 수신 준비를 합니다.
                bool pending = socketClient.ReceiveAsync(receiveArgs);
                if (pending == false)
                    OnRecvCompleted(null, receiveArgs);
            } else {
                Console.WriteLine("Socket Connect Fail!!!");
            }
            
            //SendMsg("Hello World!!!");
        }

        public void OnRecvCompleted(object? obj, SocketAsyncEventArgs args )
        {
            if( args.BytesTransferred > 0 && args.SocketError == SocketError.Success )
            {
                string recvData = System.Text.Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                
                //Console.WriteLine();
                Console.WriteLine("SERVER RE : {0}", recvData);
                //Console.WriteLine();

                // 새로운 데이터 수신을 준비합니다.
                bool pending = socketClient.ReceiveAsync(args);
                if (pending == false)
                    OnRecvCompleted(null, args);
            }
        }

        public void SendMsg( string message )
        {
            if( message.Equals(string.Empty) == false )
            {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
                sendArgs.SetBuffer(buffer, 0, buffer.Length);
                socketClient.SendAsync(sendArgs);

                //Console.WriteLine("SENDING...");
            } else {
                //Console.WriteLine("!!!SEND FAIL");
            }
        }

        public void OnSendCompleted(object? obj, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                sendArgs.BufferList = null;
            }
        }
    }
}