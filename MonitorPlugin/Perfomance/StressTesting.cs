using System.Diagnostics;
using System.IO;
using NickStrupat;
using MonitorModel;
using MonitorBuilder;

namespace Perfomance
{
    /// <summary>
    /// Класс нагрузочного тестирования.
    /// </summary>
    public class StressTester
    {
        /// <summary>
        /// Метод для нагрузочного тестирования.
        /// </summary>
        public void StressTesting()
        {
            var builder = new Builder();
            var stopWatch = new Stopwatch();
            var parameters = new Parameters();
            Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            var count = 0;
            var streamWriter = new StreamWriter("log.txt");
            const double gigabyteInByte = 0.000000000931322574615478515625;
            while (true)
            {
                stopWatch.Start();
                builder.Build(parameters);
                stopWatch.Stop();
                var computerInfo = new ComputerInfo();
                var usedMemory = (computerInfo.TotalPhysicalMemory
                - computerInfo.AvailablePhysicalMemory)
                * gigabyteInByte;
                streamWriter.WriteLine($"{++count}\t{stopWatch.Elapsed:hh\\:mm\\:ss}\t{usedMemory}");
                streamWriter.Flush();
                stopWatch.Reset();
            }
        }
    }
}
