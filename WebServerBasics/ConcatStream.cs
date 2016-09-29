//Programmer: Megan McPherson
//The ConcatStream class inherits from Stream and concatenates two streams together into one.

using System;
using System.IO;
using System.Diagnostics;

namespace CS422
{
	public class ConcatStream : Stream
	{
		public Stream firstStream;
		public Stream secondStream;

		bool lengthProvided;
		long length;

		long position;

		bool canSeek;
		bool canRead;
		bool canWrite;

		public void InitializeDependentProperties()
		{
			if (firstStream.CanSeek == true && secondStream.CanSeek == true)
				canSeek = true;
			else
				canSeek = false;

			if (firstStream.CanRead == true && secondStream.CanRead == true)
				canRead = true;
			else
				canRead = false;

			if (firstStream.CanWrite == true && secondStream.CanWrite == true)
				canWrite = true;
			else
				canWrite = false;

			Position = 0;
		}

		public ConcatStream (Stream first, Stream second)
		{
			//the first stream must have a length so we know where it ends and the next stream begins
			//if it doesn't, let the NotSupportedException bubble up outside of the constructor
			long firstStreamLength = first.Length;

			//if both streams support Length, ConcatStream should also
			try 
			{
				SetLength(firstStreamLength + second.Length);
			}
			catch (Exception e)
			{ 
				SetLength (-1); 
			}

			firstStream = first;
			secondStream = second;

			InitializeDependentProperties ();

			lengthProvided = false;
		}

		public ConcatStream (Stream first, Stream second, long fixedLength)
		{
			firstStream = first;
			secondStream = second;

			lengthProvided = true;
			InitializeDependentProperties ();

			SetLength (fixedLength);
		}

		public int ReadBytes(Stream stream, byte[] buffer, int offset, int count)
		{
			int bytesRead = 0;
			for (int i = offset; i < offset+count; i++)
			{
				int result = stream.ReadByte ();

				if (result != -1 && Position != Length)
				{
					buffer [i] = (byte)result;
					Position++;
					bytesRead++;
				} 
				else
					break;
			}
			return bytesRead;
		}

		public override int Read (byte[] buffer, int offset, int count)
		{
			if (CanRead == false)
				throw new NotSupportedException ();
			
			//start with the first stream
			int bytesRead = 0;

			if (Position < firstStream.Length - 1)
			{
				firstStream.Position = Position;
				bytesRead = ReadBytes (firstStream, buffer, offset, count);

				//do we still have more to go?
				if (bytesRead < count)
				{
					//continue from the start of the second stream
					bytesRead += ReadBytes (secondStream, buffer, offset + bytesRead, count - bytesRead);
				}
			} 

			else //starting in the second stream
			{
				secondStream.Position = Position - firstStream.Length;
				bytesRead = ReadBytes (secondStream, buffer, offset, count);
			}

			return bytesRead;
		}

		public int WriteBytes(Stream stream, byte[] buffer, int offset, int count)
		{
			int bytesWritten = 0;

			for (int i = offset; i < offset+count; i++)
			{
				if (stream.Position == stream.Length || Position == Length + 1)
					break;
				stream.WriteByte (buffer[i]);
				Position++;
				bytesWritten++;
			}
			return bytesWritten;
		}

		public override void Write (byte[] buffer, int offset, int count)
		{
			if (CanWrite == false)
				throw new NotSupportedException ();

			int bytesWritten;

			//start with the first stream
			if (Position <= firstStream.Length - 1)
			{
				firstStream.Position = Position;
				bytesWritten = WriteBytes (firstStream, buffer, offset, count);

				//do we still have more to go?
				if (bytesWritten < count)
				{
					//continue from the start of the second stream
					bytesWritten += WriteBytes (secondStream, buffer, offset + bytesWritten, count - bytesWritten);
				}
			} 

			else //starting in the second stream
			{
				secondStream.Position = Position - firstStream.Length;
				bytesWritten = ReadBytes (secondStream, buffer, offset, count);
			}
		}
			
		public override bool CanRead 
		{ 
			get { return canRead; }
		}

		public override bool CanSeek 
		{ 
			get { return canSeek; }
		}

		public override bool CanWrite 
		{ 
			get { return canWrite; }
		}

		public override void Flush ()
		{
			throw new NotImplementedException ();
		}


		public override long Length { 
			get
			{
				if (lengthProvided || length != -1)
					return length;
				else
					throw new NotSupportedException ();
			}
		}


		public override long Position 
		{ 
			get{ return position; }
			set 
			{
				if (value < 0)
					position = 0;
				else if (value > Length)
					position = Length;
				else
					position = value;
			}
		}

		public override long Seek (long offset, SeekOrigin origin)
		{
			if (CanSeek == false || Length == 0)
				throw new NotSupportedException ();
			
			if (origin == SeekOrigin.Begin)
				Position = offset;
			
			else if (origin == SeekOrigin.Current)
				Position = Position + offset;
			
			else if (origin == SeekOrigin.End)
				Position = Length + offset;

			return Position;
		}

		public override void SetLength (long value)
		{
			length = value;
		}
	}
}

