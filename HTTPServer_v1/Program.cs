using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTP_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();
            //Start server
            string redirecrules = "../../Text Files/redirectionRules.txt";
            // 1) Make server object on port 1000
            Server server = new Server(1000, redirecrules);
            // 2) Start Server
            server.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            FileStream FS = new FileStream("../../Text Files/redirectionRules.txt", FileMode.Open);
            StreamWriter sr = new StreamWriter(FS);  //this.fileName
            sr.WriteLine("{0},{1}", "aboutus.html", "aboutus2.html");
            sr.Close();
            FS.Close();

        
        }

    }
}
