using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTP_Server
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        public string HTMLV;
        public string METH;
        public string relativeCONTENT;
        public Dictionary<string, string> headerLines;
        string[] req;
        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {

            bool numOfLine;
            //TODO: parse the receivedRequest using the \r\n delimeter   
           
            string[] Delimeter = new string[] { "\r\n" };
            requestLines = requestString.Split(Delimeter, StringSplitOptions.None);

            if (!ValidateBlankLine())
            {
                return false;
            }
          
            relativeCONTENT = requestLines[3];
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length >= 3) numOfLine = true;
            else numOfLine = false;

            // Parse Request line 
            // Validate blank line exists
            // Load header lines into HeaderLines dictionary
            if (numOfLine && ParseRequestLine() && LoadHeaderLines())
                return true;
            else
                return false;

        }

        private bool ParseRequestLine()
        {
         
            req = requestLines[0].Split(new char[] {' '},StringSplitOptions.None);
            METH = req[0];
            relativeURI = req[1].Substring(1);
            if (!ValidateIsURI(relativeURI)) return false;

            HTMLV = req[2];
            if (HTMLV =="HTTP/1.1")
               httpVersion = HTTPVersion.HTTP11;
            else if (HTMLV == "HTTP/1.0​")
                httpVersion = HTTPVersion.HTTP10;
            else if (HTMLV == "HTTP/0.9")
                httpVersion = HTTPVersion.HTTP09;
            else
                return false;

            if (String.Equals(METH,"GET", StringComparison.InvariantCultureIgnoreCase))
                method = RequestMethod.GET;
            else if (METH == "HEAD​")
                method = RequestMethod.HEAD;
            else if (METH == "POST​")
                method = RequestMethod.POST;
            else
                return false;


            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            string[] headers = new string[] { };
            this.headerLines = new Dictionary<string, string>();
            string[] Delimeter = new string[] { ": " };
            for (int j = 1; j < requestLines.Length - 2; j++)
            {
                string[] Delimeter2 = new string[] { "\n" };
                headers = requestLines[j].Split(Delimeter2, StringSplitOptions.None);

            }
            if (headers.Length == 0) return false;

            for (int i = 0; i < headers.Length; i++)
            {
                string[] line = headers[i].Split(Delimeter, StringSplitOptions.None);
                
                this.headerLines.Add(line[0], line[1]);
            }

            return true;
           
        }

        private bool ValidateBlankLine()
        {
            if (requestLines[requestLines.Length - 2] == String.Empty)
            {
                return true;
            }
            else
            {
                return false;
            }
          

        }

    }
}
