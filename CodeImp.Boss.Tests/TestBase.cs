using System.Reflection;

namespace CodeImp.Boss.Tests
{
	[TestFixture]
	public abstract class TestBase
	{
		private List<IDisposable> disposables = new List<IDisposable>();

		[TearDown]
		public void Cleanup()
		{
			foreach(IDisposable disposable in disposables)
				disposable.Dispose();
			disposables.Clear();
		}

		/// <summary>
		/// Returns the path where the executable is running.
		/// </summary>
		private string GetAppPath()
		{
			string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if(path == null)
				throw new IOException("Unable to determine application path");
			return path;
		}

		/// <summary>
		/// Returns the path where the test data is located.
		/// </summary>
		protected string GetTestDataPath()
		{
			string apppath = GetAppPath();
			return Path.Combine(apppath, "..\\..\\..\\TestData");
		}

		/// <summary>
		/// Opens a FileStream for the specified test data file.
		/// </summary>
		protected FileStream StreamTestData(string filename)
		{
			string pathfilename = Path.Combine(GetTestDataPath(), filename);
			FileStream fs = File.OpenRead(pathfilename);
			disposables.Add(fs);
			return fs;
		}

		protected string ReadAllTestData(string filename)
		{
			string pathfilename = Path.Combine(GetTestDataPath(), filename);
			return File.ReadAllText(pathfilename);
		}

		protected string BytesToHex(byte[] bytes)
		{
			return BytesToHex(bytes, false);
		}

		protected string BytesToHex(byte[] bytes, bool removeDashes)
		{
			string hex = BitConverter.ToString(bytes);
			if(removeDashes)
			{
				hex = hex.Replace("-", "");
			}

			return hex;
		}

		protected byte[] HexToBytes(string hex)
		{
			string fixedHex = hex.Replace("-", string.Empty);

			// array to put the result in
			byte[] bytes = new byte[fixedHex.Length / 2];
			// variable to determine shift of high/low nibble
			int shift = 4;
			// offset of the current byte in the array
			int offset = 0;
			// loop the characters in the string
			foreach(char c in fixedHex)
			{
				// get character code in range 0-9, 17-22
				// the % 32 handles lower case characters
				int b = (c - '0') % 32;
				// correction for a-f
				if(b > 9)
				{
					b -= 7;
				}
				// store nibble (4 bits) in byte array
				bytes[offset] |= (byte)(b << shift);
				// toggle the shift variable between 0 and 4
				shift ^= 4;
				// move to next byte
				if(shift != 0)
				{
					offset++;
				}
			}
			return bytes;
		}

		protected void AssertStreamIsEqualTo(MemoryStream stream, string expectedhex)
		{
			byte[] bytes = stream.ToArray();
			string hex = BytesToHex(bytes);
			TestContext.WriteLine($"Hex: {hex}");
			Assert.That(hex, Is.EqualTo(expectedhex));
		}
	}
}
