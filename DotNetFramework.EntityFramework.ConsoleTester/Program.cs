using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;

namespace DotNetFramework.EntityFramework.ConsoleTester
{
    class Program
    {
        private static IContainer _container;
        
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ConsoleModule());
            _container = builder.Build();

            uint age = 49;
            string firstName = "Adam";
            string lastName = "Carr";
            int iterations = 10000;
            IPersonRepository personRepository = _container.Resolve<IPersonRepository>();

            Benchmark(() => RunMultipleTimes(() => CreatePocoPerson(firstName, lastName, age), iterations));
            Benchmark(() => RunMultipleTimes(() => CreatePersonData(firstName, lastName, age), iterations));
            Benchmark(() => RunMultipleTimes(() => CreateEFPerson(personRepository, firstName, lastName, age), iterations));

            Console.WriteLine("Tests complete");
            Console.Read();
        }

        static void Benchmark(Action action)
        {
            var startTime = DateTime.Now;
            Console.WriteLine("Started at {0}", startTime);
            action();
            var endTime = DateTime.Now;
            Console.WriteLine("Ended at {0} :: Duration {1}", endTime, endTime.Subtract(startTime));
        }

        static void RunMultipleTimes(Action action, int times)
        {
            for (int i = 0; i < times; i++)
            {
                action();
            }
        }

        static void CreatePocoPerson(string firstName, string lastName, uint age)
        {
            var person = new PocoPerson();
            person.FirstName = firstName;
            person.LastName = lastName;
            person.Age = age;
        }

        static void CreatePersonData(string firstName, string lastName, uint age)
        {
            var person = new PersonData();
            person.FirstName = firstName;
            person.LastName = lastName;
            person.Age = age;
        }

        static void CreateEFPerson(IPersonRepository personRepository, string firstName, string lastName, uint age)
        {
            var person = personRepository.GetNew();
            person.FirstName = firstName;
            person.LastName = lastName;
            person.Age = age;
        }
    }
}
