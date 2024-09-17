using CodeImp.Boss.Tests.Performance;
using System.Reflection;

string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

PerformanceTest test = new PerformanceTest();
const int REPEATS = 20;

OutputFiles();
test.RunBossBatches(REPEATS);
test.RunJsonBatches(REPEATS);


void OutputFiles()
{
	MemoryStream stream = test.SingleRunBoss();
	File.WriteAllBytes(Path.Combine(path, "Serialized.boss"), stream.ToArray());

	string json = test.SingleRunJson();
	File.WriteAllText(Path.Combine(path, "Serialized.json"), json);
}
