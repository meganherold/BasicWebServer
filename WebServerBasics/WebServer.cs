//programmer: Megan McPherson
//this project is a simple console app that provides basic web server functionality.
//browse to it on a local machine on a web browser and see the response

using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections;

namespace CS422
{
	public class WebServer
	{

		public static void Main (string[] args)
		{
			string DefaultTemplate =
				"HTTP/1.1 200 OK\r\n" +
				"Content-Type: text/html\r\n" +
				"\r\n\r\n" +
				"<html>ID Number: {0}<br>" +
				"DateTime.Now: {1}<br>" +
				"Requested URL: {2}</html>";

			Console.WriteLine ("Starting web server on port 4220...");
			Start (4220, DefaultTemplate);
		}

		//creates a TcpListener and listens for a connection on the specified port
		public static bool Start(int port, string responseTemplate)
		{
			try
			{
				//create the tcplistener and start listening
				TcpListener listener = new TcpListener (IPAddress.Any, port);
				listener.Start ();

				Console.WriteLine ("Waiting for a client connection...");

				//accept a client
				TcpClient client;
			
				client = listener.AcceptTcpClient ();
				Console.WriteLine ("Connected to a client");

				//read request from client
				NetworkStream stream = client.GetStream();
				byte[] buffer = new byte[4096];
				int i = stream.Read (buffer, 0, buffer.Length);
				string request = "";
				string url = "";
				string response = "";

				while (i != 0) //loop to recieve all data
				{
					string requestPiece = System.Text.Encoding.ASCII.GetString(buffer, 0, i);
					Console.WriteLine("Received: " + requestPiece);
					request += requestPiece;

					int headerValidity = isValidHeader(request, ref url);

					if(headerValidity == 1)
					{
						Console.WriteLine("Valid HTTP header, writing response...");
						response = constructResponse(responseTemplate, url);
						byte[] byteResponse = System.Text.Encoding.ASCII.GetBytes(response);

						stream.Write(byteResponse, 0, byteResponse.Length);
						client.Close();
						stream.Dispose();
						return true;
					}
					else if (headerValidity == -1)
					{
						Console.WriteLine("Invalid HTTP header, terminating connection...");
						client.Close();
						stream.Dispose();
						return false;
					}

					//if isValidHeader() returned 0, just keep reading
					i = stream.Read (buffer, 0, buffer.Length);
				}

			}
			catch(Exception e)
			{
				Console.WriteLine (e.Message);
				return false;
			}
			return false;
		}

		public static int isValidHeader(string request, ref string url)  //(?<double>\w)
		{
			//we're sure it's valid, return 1
			Regex regex = new Regex(@"^GET\s{1}(?<url>[\w./0-9:?%&]+)\s{1}(HTTP/1.1){1}\r\n");
			MatchCollection matches = regex.Matches(request);

			if (matches.Count == 1)
			{
				GroupCollection groups = matches [0].Groups;
				url = groups ["url"].Value;
				return 1;
			} 

			//we're sure it's invalid, return -1
			else if (request.Length > 3 && request.Substring (0, 4) != "GET ")
				return -1;
			
			else if (request.Contains ("\r\n"))
				return -1;

			//we don't have enough info yet, keep reading
			return 0;
		}

		public static string constructResponse(string responseTemplate, string url)
		{
			string response = string.Format (responseTemplate, "11372881", DateTime.Now.ToString (), url);
			return response;
		}
	}
}
