using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAPoTNumLib.Framework.Samples;
using GAPoTNumLib.Framework.Samples.Construction;

namespace GAPoTNumLib.Framework
{
    class Program
    {
        static void Main(string[] args)
        {
            ClarkeRotation4DSample.Execute();

            Console.WriteLine();
            Console.WriteLine(@"Press any key to exit...");
            Console.ReadKey();
        }
    }
}
