using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using Shouldly;
using System.Net.Sockets;
using System.Net;

namespace CS422
{
	[TestFixture]
	public class ConcatStream_UnitTests
	{
		private static void AddText(FileStream fs, string value)
		{
			byte[] info = new UTF8Encoding(true).GetBytes(value);
			fs.Write(info, 0, info.Length);
		}

		public FileStream CreateFileStream(string path, string contents)
		{
			// Delete the file if it exists.
			if (File.Exists(path))
				File.Delete(path);
			
			//Create the file.
			FileStream filestream = File.Create (path);
			AddText (filestream, contents);
			filestream.Position = 0;

			return filestream;
		}

		public FileStream CreateFileStreamForWriting (string path)
		{
			// Delete the file if it exists.
			if (File.Exists(path))
				File.Delete(path);

			//Create the file.
			FileStream fileStream = new FileStream (path, FileMode.Create);
			return fileStream;
		}

		[Test]
		public void Constructor_BothStreamsHaveLength_LengthPropertyGetable ()
		{
			FileStream stream1 = CreateFileStream (@"test1.txt", "012345");
			FileStream stream2 = CreateFileStream (@"test2.txt", "6789101112");

			ConcatStream concatStream = new ConcatStream (stream1, stream2);
			concatStream.Length.ShouldBe (16);
		}

		[Test]
		public void Read_FirstConstructor_BothStreamsHaveLength_RightAmountOfBytes ()
		{
			FileStream stream1 = CreateFileStream (@"test1.txt", "012345");
			FileStream stream2 = CreateFileStream (@"test2.txt", "6789101112");

			ConcatStream concatStream = new ConcatStream (stream1, stream2);
			byte[] buffer = new byte[100];

			int bytesRead = concatStream.Read (buffer, 0, 100);
			bytesRead.ShouldBe (16);
		}

		[Test]
		public void Read_FirstConstructor_BothStreamsHaveLength_BufferIsCorrect ()
		{
			FileStream stream1 = CreateFileStream (@"test1.txt", "012345");
			FileStream stream2 = CreateFileStream (@"test2.txt", "6789101112");

			ConcatStream concatStream = new ConcatStream (stream1, stream2);
			byte[] buffer = new byte[100];

			concatStream.Read (buffer, 0, 100);

			((int)buffer [0]).ShouldBe (48);
			((int)buffer [1]).ShouldBe (49);
			((int)buffer [2]).ShouldBe (50);
			((int)buffer [3]).ShouldBe (51);
			((int)buffer [4]).ShouldBe (52);
			((int)buffer [5]).ShouldBe (53);
			((int)buffer [6]).ShouldBe (54);
			((int)buffer [7]).ShouldBe (55);
			((int)buffer [8]).ShouldBe (56);
			((int)buffer [9]).ShouldBe (57);
			((int)buffer [10]).ShouldBe (49);
			((int)buffer [11]).ShouldBe (48);
			((int)buffer [12]).ShouldBe (49);
			((int)buffer [13]).ShouldBe (49);
			((int)buffer [14]).ShouldBe (49);
			((int)buffer [15]).ShouldBe (50);
		}

		[Test]
		public void Read_FirstConstructor_StartInSecondStream_RightAmountOfBytes ()
		{
			FileStream stream1 = CreateFileStream (@"test1.txt", "012345");
			FileStream stream2 = CreateFileStream (@"test2.txt", "6789101112");

			ConcatStream concatStream = new ConcatStream (stream1, stream2);
			concatStream.Position = 12;
			byte[] buffer = new byte[100];

			int bytesRead = concatStream.Read (buffer, 0, 100);
			bytesRead.ShouldBe (4);
			//12 + 4 = totalbytes = 16
		}

		[Test]
		public void Read_FirstConstructor_StartInFirstStream_RightAmountOfBytes ()
		{
			FileStream stream1 = CreateFileStream (@"test1.txt", "012345");
			FileStream stream2 = CreateFileStream (@"test2.txt", "6789101112");

			ConcatStream concatStream = new ConcatStream (stream1, stream2);
			concatStream.Position = 2;
			byte[] buffer = new byte[100];

			int bytesRead = concatStream.Read (buffer, 0, 100);
			bytesRead.ShouldBe (14);
			//2 + 14 = totalbytes = 16
		}

		[Test]
		public void Read_FirstConstructor_StartAtBeginningOfSecondStream_RightAmountOfBytes ()
		{
			FileStream stream1 = CreateFileStream (@"test1.txt", "012345");
			FileStream stream2 = CreateFileStream (@"test2.txt", "6789101112");

			ConcatStream concatStream = new ConcatStream (stream1, stream2);
			concatStream.Position = 6;
			byte[] buffer = new byte[100];

			int bytesRead = concatStream.Read (buffer, 0, 100);
			bytesRead.ShouldBe (10);
			//12 + 4 = totalbytes = 16
		}

		[Test]
		public void Read_SecondConstructor_RightAmountOfBytes ()
		{
			FileStream stream1 = CreateFileStream (@"test1.txt", "012345");
			FileStream stream2 = CreateFileStream (@"test2.txt", "6789101112");

			ConcatStream concatStream = new ConcatStream (stream1, stream2, 10);
			byte[] buffer = new byte[100];

			int bytesRead = concatStream.Read (buffer, 0, 100);
			bytesRead.ShouldBe (10);
		}

		[Test]
		public void Read_SecondConstructor_StartInSecondStream_RightAmountOfBytes ()
		{
			FileStream stream1 = CreateFileStream (@"test1.txt", "012345");
			FileStream stream2 = CreateFileStream (@"test2.txt", "6789101112");

			ConcatStream concatStream = new ConcatStream (stream1, stream2, 10);
			concatStream.Position = 6;
			byte[] buffer = new byte[100];

			int bytesRead = concatStream.Read (buffer, 0, 100);
			bytesRead.ShouldBe (4);
		}

		[Test]
		public void Read_SecondConstructor_BufferIsCorrect ()
		{
			FileStream stream1 = CreateFileStream (@"test1.txt", "012345");
			FileStream stream2 = CreateFileStream (@"test2.txt", "6789101112");

			ConcatStream concatStream = new ConcatStream (stream1, stream2, 10);
			byte[] buffer = new byte[100];

			int bytesRead = concatStream.Read (buffer, 0, 100);

			((int)buffer [0]).ShouldBe (48);
			((int)buffer [1]).ShouldBe (49);
			((int)buffer [2]).ShouldBe (50);
			((int)buffer [3]).ShouldBe (51);
			((int)buffer [4]).ShouldBe (52);
			((int)buffer [5]).ShouldBe (53);
			((int)buffer [6]).ShouldBe (54);
			((int)buffer [7]).ShouldBe (55);
			((int)buffer [8]).ShouldBe (56);
			((int)buffer [9]).ShouldBe (57);

			bytesRead.ShouldBe (10);
		}

		[Test]
		public void Write_FirstConstructor_FilesHaveThingsInThem()
		{
			FileStream stream1 = CreateFileStreamForWriting (@"test1.txt");
			FileStream stream2 = CreateFileStreamForWriting (@"test2.txt");
			stream1.SetLength (50);
			stream2.SetLength (10);

			byte[] buffer = new byte[100];
			new Random ().NextBytes (buffer);

			ConcatStream concatStream = new ConcatStream (stream1, stream2);
			concatStream.Write (buffer, 0, 100);
		

			1.ShouldBe (1);
		}

		[Test]
		public void Write_FirstConstructor_File2HasThingsInIt()
		{
			FileStream stream1 = CreateFileStreamForWriting (@"test1.txt");
			FileStream stream2 = CreateFileStreamForWriting (@"test2.txt");
			stream1.SetLength (10);
			stream2.SetLength (10);

			byte[] buffer = new byte[100];
			new Random ().NextBytes (buffer);

			ConcatStream concatStream = new ConcatStream (stream1, stream2);
			concatStream.Position = 10;

			concatStream.Write (buffer, 0, 100);

			1.ShouldBe (1);
		}

		[Test]
		public void Write_FirstConstructor_FilesMatch()
		{
			FileStream stream1 = CreateFileStreamForWriting (@"test1.txt");
			FileStream stream2 = CreateFileStreamForWriting (@"test2.txt");
			stream1.SetLength (50);
			stream2.SetLength (50);

			byte[] buffer = new byte[100];
			new Random ().NextBytes (buffer);

			ConcatStream concatStream = new ConcatStream (stream1, stream2);
			concatStream.Write (buffer, 0, 100);

			concatStream.firstStream.Seek (0, SeekOrigin.Begin);
			concatStream.secondStream.Seek (0, SeekOrigin.Begin);

			for (int i = 0; i < stream1.Length; i++)
			{
				int result1 = concatStream.firstStream.ReadByte ();
				result1.ShouldBe (buffer [i]);
				int result2 = concatStream.secondStream.ReadByte ();
				result2.ShouldBe (buffer [i + 50], String.Format("at location i = {0}", i));
			}

		}

		[Test]
		public void Write_SecondConstructor_FilesMatch()
		{
			FileStream stream1 = CreateFileStreamForWriting (@"test1.txt");
			FileStream stream2 = CreateFileStreamForWriting (@"test2.txt");
			stream1.SetLength (50);
			stream2.SetLength (50);

			byte[] buffer = new byte[100];
			new Random ().NextBytes (buffer);

			ConcatStream concatStream = new ConcatStream (stream1, stream2, 90);
			concatStream.Write (buffer, 0, 100);

			concatStream.firstStream.Seek (0, SeekOrigin.Begin);
			concatStream.secondStream.Seek (0, SeekOrigin.Begin);

			for (int i = 0; i < stream1.Length; i++)
			{
				int result1 = concatStream.firstStream.ReadByte ();
				result1.ShouldBe (buffer [i]);
			}

			for (int i = 0; i < stream2.Length; i++)
			{
				int result2 = concatStream.secondStream.ReadByte ();
				result2.ShouldBe (buffer [i + 50], String.Format("at location i = {0}", i));
			}

		}
			
	}

}

