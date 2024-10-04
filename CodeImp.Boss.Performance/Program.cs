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
    string bossfile = Path.Combine(path, "Serialized.boss");
	File.WriteAllBytes(bossfile, stream.ToArray());

	string json = test.SingleRunJson();
    string jsonfile = Path.Combine(path, "Serialized.json");
	File.WriteAllText(jsonfile, json);

    FileInfo bossinfo = new FileInfo(bossfile);
    FileInfo jsoninfo = new FileInfo(jsonfile);
    
    float ratio = ((float)bossinfo.Length / (float)jsoninfo.Length) * 100.0f;
    Console.WriteLine($"Boss file size is {ratio:0.00}% compared to Json.");
}
