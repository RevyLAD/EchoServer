using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Collections;
using System.Threading;

namespace EchoServer
{

    class Program
    {
        
        static bool aCheck = true;
        static void Main(string[] args)
        {

            
            TcpListener server = null;

            List<string> txtList = new List<string>();

           
            TcpClient client;
            try
            {
                server = new TcpListener(IPAddress.Parse("127.0.0.1"), 5557);
                server.Start();

                Console.WriteLine("TCP메아리 서버 시작...");

                while (true)
                {
                    
                    //클라이언트가 요청될때마다 새로운 쓰레드를 생성하여 따로 작업을 함
                    client = server.AcceptTcpClient();
                    new Thread(new ThreadStart(() => processData(client))).Start();
                    
                   
                    //ThreadStart라는 델리게이트가 void형이기 떄문에   processData(TcpClient client);함수를 바로 호출할수 없다
                    //따라서 새로운 void 형 함수를 만들어서 해야하는데 그때에는 client 인자를 넘길수 없으므로 이름없는 메서드로 바로 전달
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {

                server.Stop();
            }

            Console.WriteLine("서버를 종료합니다.");

            //sw.Close();
        }



        //private static void ServiceStart()
        //{
        //    processData(client);
        //}

        private static void processData(TcpClient client)
        {
            aCheck = true;
          
            Console.WriteLine("클라이언트 접속 : {0}",
                ((IPEndPoint)client.Client.RemoteEndPoint).ToString());

            
            NetworkStream stream = client.GetStream();
            
            int length;
            string data = null;
            byte[] bytes = new byte[256];

            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                data = Encoding.Default.GetString(bytes, 0, length);

                if(data.ToLower() == "bye")
                {
                    Console.WriteLine("클라연결 종료");
                    Thread.CurrentThread.Abort();
                    client.Client.Close();
                    aCheck = false;
                    break;
                }
                Console.WriteLine(String.Format("수신:[{0}] {1}",client.Client.RemoteEndPoint, data));
                

                byte[] msg = Encoding.Default.GetBytes(data);
                stream.Write(msg, 0, msg.Length);
                Console.WriteLine(String.Format("송신:[{0}] {1}", client.Client.RemoteEndPoint, data));


            }


              stream.Close();
              client.Close();




        }
    }
}
