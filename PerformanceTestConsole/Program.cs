using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;

namespace PerformanceTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Benchmark(
                () =>
                {
                    var containerBuilder = new ContainerBuilder();
                    containerBuilder.RegisterModule(new PerformanceTestModule());
                    var container = containerBuilder.Build();

                    using (var scope = container.BeginLifetimeScope())
                    {
                        var repository = scope.Resolve<IPersonRepository>();

                        for (int i = 0; i < 10000; i++)
                        {
                            var person = repository.GetNew();
                            if (person.IsValid)
                            {
                                throw new Exception("Person should not be valid");
                            }

                            person.Name = "Test Name";

                            if (person.IsValid)
                            {
                                throw new Exception("Person should not be valid");
                            }

                            person.Email = "adam.carr@gmail.com";

                            if (!person.IsValid)
                            {
                                throw new Exception("Person should be valid");
                            }
                        }
                    }
                });

            Console.Read();
        }


        static void Benchmark(Action operationToBenchmark)
        {
            var startTime = DateTime.Now;
            Console.WriteLine("Starting Benchmark: {0}", startTime);
            operationToBenchmark();
            var endTime = DateTime.Now;
            Console.WriteLine("Benchmark Complete: {0}", endTime);
            Console.WriteLine("Benchmark Duration: {0}", endTime.Subtract(startTime));
        }
    }
}
