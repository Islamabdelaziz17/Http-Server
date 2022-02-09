using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTP_Server
{
    class Logger
    {
        StreamWriter sr = new StreamWriter("log.txt");
        private string currentDirectory { get; set; }
        private string fileName { get; set; }
        private string filePath { get; set; }

        public Logger()
        {
            this.currentDirectory = Directory.GetCurrentDirectory();
            this.fileName = "log.txt";
            this.filePath = this.currentDirectory + "/" + this.fileName;
        }

        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 
            FileStream FS = new FileStream("../../Text Files/log.txt", FileMode.Open);
            StreamWriter sr = new StreamWriter(FS); //this.fileName
            sr.WriteLine("{0} : {1}\n", DateTime.Now.ToLongTimeString(), ex.Message);
            sr.Close();

            FS.Close();
        }
    }
}