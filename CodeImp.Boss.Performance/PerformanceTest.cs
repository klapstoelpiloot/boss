using System.Diagnostics;
using System.Text.Json;

namespace CodeImp.Boss.Tests.Performance
{
	[Serializable]
	public struct Vector3
	{
		public float x { get; set; }
		public float y { get; set; }
		public float z { get; set; }

		public Vector3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}

	public class TestData
	{
		public int Something { get; set; }
		public int SomethingElse { get; set; }
		public float Adjustable { get; set; }
		public float AdjustableElse { get; set; }
		public Vector3 Position { get; set; }
		public Vector3 Direction { get; set; }
		public string? Story { get; set; }

		public virtual void Fill()
		{
			const string LoremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam dolor arcu, vehicula ut odio ut," +
			"sollicitudin molestie lacus. Etiam hendrerit nec sapien sed auctor. Quisque scelerisque urna quis convallis hendrerit." +
			"Praesent sit amet magna eu mi pretium sagittis ultricies suscipit urna. Praesent hendrerit pharetra pellentesque.";

			Random rnd = new Random();
			Something = rnd.Next(int.MaxValue);
			SomethingElse = rnd.Next(int.MaxValue);
			Adjustable = (float)rnd.NextDouble() * (float)ulong.MaxValue;
			AdjustableElse = 1.0f;
			Position = new Vector3((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
			Direction = new Vector3((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
			Story = LoremIpsum.Substring(0, rnd.Next(LoremIpsum.Length));
		}
	}

	public class TestExtraData : TestData
	{
		public int More { get; set; }
		public float Useless { get; set; }
		public Vector3 Crap { get; set; }

		public override void Fill()
		{
			Random rnd = new Random();
			More = rnd.Next(int.MaxValue);
			Useless = (float)rnd.NextDouble();
			Crap = new Vector3((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
		}
	}

	public class TestSubject
	{
		public Vector3 Position { get; set; }
		public Vector3[] Trajectory { get; set; } = new Vector3[50];
		public int[] Neighbours { get; set; } = new int[100];
		public List<int> Visibility { get; set; } = new List<int>();
		public List<TestData> Data { get; set; } = new List<TestData>();
		[BossDynamic]
		public List<TestData> DynamicData { get; set; } = new List<TestData>();

		public void Fill()
		{
			Random rnd = new Random();
			Position = new Vector3((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
			Trajectory = Enumerable.Range(0, 1000).Select(v => new Vector3((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble())).ToArray();
			Neighbours = Enumerable.Range(0, 1000).ToArray();
			Visibility = Enumerable.Range(0, 1000).ToList();
			Data = Enumerable.Range(0, 200).Select(x => new TestData()).ToList();
			Data.ForEach(d => d.Fill());
			DynamicData = Enumerable.Range(0, 200).Select(x => new TestExtraData()).Cast<TestData>().ToList();
			DynamicData.ForEach(d => d.Fill());
		}
	}

	public class TestPile
	{
		public List<TestSubject> Subjects { get; set; } = new List<TestSubject>();

		public void Fill()
		{
			Subjects = Enumerable.Range(0, 20).Select(v => new TestSubject()).ToList();
			Subjects.ForEach(s => s.Fill());
		}
	}

    public class Vector3TypeHandler : BossTypeHandler
    {
        public override byte BossType => 64;

        public override Type ClassType => typeof(Vector3);

        public override object? ReadFrom(BossReader reader, Type basetype)
        {
            Vector3 v = new Vector3();
            v.x = reader.ReadSingle();
            v.y = reader.ReadSingle();
            v.z = reader.ReadSingle();
            return v;
        }

        public override void WriteTo(BossWriter writer, object value)
        {
            Vector3 v = (Vector3)value;
            writer.Write(v.x);
            writer.Write(v.y);
            writer.Write(v.z);
        }
    }


	public class PerformanceTest
	{
		public void RunBossBatches()
		{
			BossSerializer.RegisterTypeHandler(new Vector3TypeHandler());

			// Do a single run which we do not measure, to ensure
			// the JIT is not messing with out measurement.
			BatchRunBoss(out long bosssize);
			Thread.Sleep(100);

			List<long> times = new List<long>();
			for(int i = 0; i < 5; i++)
			{
				long ms = BatchRunBoss(out _);
				times.Add(ms);
			}
			Console.WriteLine($"SerializeToMemory - Boss took {times.Average()} ms.");
		}

		public void RunJsonBatches()
		{
			// Do a single run which we do not measure, to ensure
			// the JIT is not messing with out measurement.
			BatchRunJson(out long jsonsize);
			Thread.Sleep(100);

			List<long> times = new List<long>();
			for(int i = 0; i < 5; i++)
			{
				long ms = BatchRunJson(out _);
				times.Add(ms);
			}
			Console.WriteLine($"SerializeToMemory - Json took {times.Average()} ms.");
		}

		public MemoryStream SingleRunBoss()
		{
			BossSerializer.RegisterTypeHandler(new Vector3TypeHandler());

			using MemoryStream stream = new MemoryStream(1000000);
			TestPile tp = MakePile();
			BossSerializer.Serialize(tp, stream);
			return stream;
		}

		public string SingleRunJson()
		{
			string result = string.Empty;
			TestPile tp = MakePile();
			result = JsonSerializer.Serialize(tp);
			return result;
		}

		private long BatchRunBoss(out long datasize)
		{
			// Initialize data
			using MemoryStream stream = new MemoryStream(1000000);
			TestPile tp = MakePile();
			BossSerializer.Serialize(tp, stream);
			datasize = stream.Length;

			Stopwatch stopwatch = Stopwatch.StartNew();

			for(int i = 0; i < 100; i++)
			{
				// Write to stream
				stream.Seek(0, SeekOrigin.Begin);
				BossSerializer.Serialize(tp, stream);
				datasize = stream.Length;
			}

			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		private long BatchRunJson(out long datasize)
		{
			// Initialize data
			string result = string.Empty;
			TestPile tp = MakePile();
			result = JsonSerializer.Serialize(tp);
			datasize = result.Length;

			Stopwatch stopwatch = Stopwatch.StartNew();

			for(int i = 0; i < 100; i++)
			{
				// Write to stream
				result = JsonSerializer.Serialize(tp);
				datasize = result.Length;
			}

			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}

		private TestPile MakePile()
		{
			TestPile tp = new TestPile();
			tp.Fill();
			return tp;
		}
	}
}
