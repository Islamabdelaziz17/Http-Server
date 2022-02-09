using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTP_Server
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectionPath="None")
        {
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            this.code = code;
            headerLines.Add(GetStatusLine(code));
            headerLines.Add(" Content-Type : text/html");
            headerLines.Add(" Content-Length : " + content.Length.ToString());
            headerLines.Add(" Date : " + DateTime.Now.ToString());

            if (!String.IsNullOrEmpty(redirectionPath))
            {
                headerLines.Add("Location: " + redirectionPath);
            }
           
            // TODO: Create the response string 
            foreach (string str in headerLines)
            {

                responseString += str + "\r\n";
            }
            responseString += "\r\n" + content;

        }

        private string GetStatusLine(StatusCode code)
        {
       // TODO: Create the response status line and return it
       
            int codee;
            if (code == StatusCode.OK)
            {
               
                codee = (int)StatusCode.OK;
            }
            else if (code == StatusCode.InternalServerError)
            {
               
                codee = (int)StatusCode.InternalServerError;
            }
            else if (code == StatusCode.NotFound)
            {
           
                codee = (int)StatusCode.NotFound;
            }
            else if (code == StatusCode.BadRequest)
            {
               
                codee = (int)StatusCode.BadRequest;
            }
            else
            {
              
                codee = (int)StatusCode.Redirect;

            }
            string statusLine = Configuration.ServerHTTPVersion + " " + codee.ToString() + " " + code.ToString();
            return statusLine;

        }
    }
}