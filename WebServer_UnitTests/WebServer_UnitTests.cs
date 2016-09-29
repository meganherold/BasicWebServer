using NUnit.Framework;
using System;
using CS422;
using Shouldly;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;

namespace CS422
{
	[TestFixture]
	public class WebServer_UnitTests
	{
		string ResponseTemplate = 
			"HTTP/1.1 200 OK\r\n" +
			"Content-Type: text/html\r\n" +
			"\r\n\r\n" +
			"<html>ID Number: {0}<br>" +
			"DateTime.Now: {1}<br>" +
			"Requested URL: {2}</html>";
		
		string url;

		[Test]
		public void isValidHeader_ValidGETRequest_ReturnsOne ()
		{
			string request = "GET http://www.somewebsite.com HTTP/1.1\r\nThis is the body";

			int result = WebServer.isValidHeader (request, ref url);

			result.ShouldBe (1);
			url.ShouldBe ("http://www.somewebsite.com");
		}

		[Test]
		public void isValidHeader_ValidPUTRequest_ReturnsNegativeOne ()
		{
			string request = "PUT http://www.somewebsite.com HTTP/1.1\r\nThis is the body";

			int result = WebServer.isValidHeader (request, ref url);

			result.ShouldBe (-1);
		}

		[Test]
		public void isValidHeader_ValidGETRequest_InvalidHTTPVersion_ReturnsNegativeOne ()
		{
			string request = "GET http://www.somewebsite.com HTTP/1.0\r\nThis is the body";

			int result = WebServer.isValidHeader (request, ref url);

			result.ShouldBe (-1);
		}

		[Test]
		public void isValidHeader_InvalidGETRequest_NoURL_ReturnsNegativeOne ()
		{
			string request = "GET  HTTP/1.0\r\nThis is the body";

			int result = WebServer.isValidHeader (request, ref url);

			result.ShouldBe (-1);
		}

		[Test]
		public void isValidHeader_InvalidMethod_ReturnsNegativeOne ()
		{
			string request = "GEO ";

			int result = WebServer.isValidHeader (request, ref url);

			result.ShouldBe (-1);
		}

		[Test]
		public void isValidHeader_NotEnoughInfo_ReturnsZero ()
		{
			string request = "GET http://www.somewebsite.com HTT";

			int result = WebServer.isValidHeader (request, ref url);

			result.ShouldBe (0);
		}

		[Test]
		public void constructResponse_ValidURL_CorrectlyFormattedResponse()
		{
			string url = "http://www.somewebsite.com";
			string expected = "HTTP/1.1 200 OK\r\n" +
				"Content-Type: text/html\r\n" +
				"\r\n\r\n" +
				"<html>ID Number: 11372881<br>" +
				String.Format("DateTime.Now: {0}<br>", DateTime.Now) +
				"Requested URL: http://www.somewebsite.com</html>";

			WebServer.constructResponse (ResponseTemplate, url).ShouldBe (expected);
		}

		/*[Test]
		public void Start_ValidRequest_RecieveResponse()
		{
			//start the server listening
			Thread serverThread = new Thread(new ThreadStart((delegate)WebServer.Start()));
			serverThread.Start (4220, ResponseTemplate);

			//create a request
			WebRequest request = WebRequest.Create ("localhost:4220");
			request.Method = "GET";
			((HttpWebRequest)request).ProtocolVersion = HttpVersion.Version11;

			string getData = "I guess this is a test";
			byte[] byteArray = Encoding.UTF8.GetBytes (getData);

			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = byteArray.Length;

			Stream datastream = request.GetRequestStream ();
			datastream.Write (byteArray, 0, byteArray.Length);
			datastream.Close ();

			WebResponse response = request.GetResponse ();
			((HttpWebResponse)response).StatusCode.ShouldBe (HttpStatusCode.OK);
		} */
	}
}

