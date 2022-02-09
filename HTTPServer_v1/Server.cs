using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTP_Server
{
    class Server
    {
        Socket serverSocket;
        int MAXBACkLOG= 100;
        string request;
       
        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);

            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint TheHostPoint = new IPEndPoint(IPAddress.Any, portNumber);
            this.serverSocket.Bind(TheHostPoint);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            this.serverSocket.Listen(MAXBACkLOG);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                
                //TODO: accept connections and start thread for each accepted connection.
                Socket CLIENTSOCK = this.serverSocket.Accept();
                Console.WriteLine("New Client Accepted: {0}", CLIENTSOCK.RemoteEndPoint);
                Thread NEW_THREAD = new Thread(new ParameterizedThreadStart(HandleConnection));
                NEW_THREAD.Start(CLIENTSOCK);

            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket Recieved_clientsock = (Socket)obj;

            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            Recieved_clientsock.ReceiveTimeout = 0;

            // TODO: receive requests in while true until remote client closes the socket.

            while (true)
            {
                try
                
                {
                    byte[] requestbuffer = new byte[1024 * 10];
                    // TODO: Receive request
                    Recieved_clientsock.Receive(requestbuffer);
                    request = Encoding.ASCII.GetString(requestbuffer);
                    // TODO: break the while loop if receivedLen==0
                    if(request.Length == 0)
                    {
                        Console.WriteLine("Client Ended The Connection {0}",Recieved_clientsock.RemoteEndPoint);
                        break;
                    }

                           // TODO: Create a Request object using received request string
                           Console.WriteLine("Recieved Successfully {0} from Client {1}",request, Recieved_clientsock.RemoteEndPoint);
                           Request newreq = new Request(request);

                            // TODO: Call HandleRequest Method that returns the response
                            Response resp = HandleRequest(newreq);

                   
                            // TODO: Send Response back to client
                            Recieved_clientsock.Send(Encoding.ASCII.GetBytes(resp.ResponseString));
                            

                       }
                        catch (Exception ex)
                        {
                            // TODO: log exception using Logger class
                           Logger.LogException(ex);
                        }
                    }

                    // TODO: close client socket
                    Recieved_clientsock.Close();
                 
        }

        Response HandleRequest(Request request)
        {
           
            string content = "";
            string PhysicalPath;
            //Request newrequest = new Request(request.ToString());
            Response bad_resp, redir_resp, nofile_resp, intern_resp;

            try
            {
                //.throw new Exception();
                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    content = File.ReadAllText(Configuration.RootPath + Configuration.BadRequestDefaultPageName);
                    bad_resp = new Response(StatusCode.BadRequest, "text/html", content, "");
                    return bad_resp;
                }

                //TODO: map the relativeURI in request to get the physical path of the resource.


                //TODO: check for redirect
                if (Configuration.RedirectionRules.Any(kvp => kvp.Key.Contains(request.relativeURI)))
                {

                    PhysicalPath = Configuration.RootPath + '\\' + GetRedirectionPagePathIFExist(request.relativeURI).ToString();
                    content = File.ReadAllText(PhysicalPath);
                   
                    redir_resp = new Response(StatusCode.Redirect, "text/html", content, GetRedirectionPagePathIFExist(request.relativeURI));
                    return redir_resp;
                }

                //  TODO: check file exists
                PhysicalPath = Path.Combine(Configuration.RootPath, request.relativeURI);
                if (!File.Exists(PhysicalPath))
                {
                    PhysicalPath = Configuration.RootPath + Configuration.NotFoundDefaultPageName;
                    content = File.ReadAllText(PhysicalPath);
                    nofile_resp = new Response(StatusCode.NotFound, "text/html", content, "");
                    return nofile_resp;
                }

                //TODO: read the physical file
                // Create OK response
                else
                {
                    content = LoadDefaultPage(request.relativeURI);

                    Response okresponse = new Response(StatusCode.OK, "text/html", content, "");
                    return okresponse;


                }
                  


            }
            catch (Exception ex)
            {
                
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error.
                content = File.ReadAllText(Configuration.RootPath + Configuration.InternalErrorDefaultPageName);
                intern_resp = new Response(StatusCode.InternalServerError, "text/html", content, Configuration.RootPath + Configuration.InternalErrorDefaultPageName);
                return intern_resp;
            }

        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if(Configuration.RedirectionRules.Any(kvp => kvp.Key.Contains(relativePath)))
            {
             
                return Configuration.RedirectionRules[relativePath];
            }
            else
            {
                return string.Empty;
            }
                
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            //TODO: Read file and return its content
            try
            {
                string content = File.ReadAllText(filePath);
                return content;
            }
            // TODO: ELSE Check if filepath not exist log exception using Logger class and return empty string
            catch (Exception ex)
            {
                if (!File.Exists(filePath))
                {
                    Logger.LogException(ex);
                    
                }
                return string.Empty;
            }
        

        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                FileStream FS = new FileStream(filePath, FileMode.Open);
                StreamReader sr = new StreamReader(FS);

                while(sr.Peek()!=-1)
                {
                    string rules = sr.ReadLine();
                    string[] split = rules.Split(',');
                    if(split[0] == "")
                    {
                        break;
                    }
                    Configuration.RedirectionRules = new Dictionary<string, string>();
                    Configuration.RedirectionRules.Add(split[0], split[1]);

                }
                FS.Close();
             
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
