using System;

namespace TestAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = new TestObject()
                .Method1()
                .Method2()
                .Method3()
                .Method4()
                .NextObject()
                .NMethod1()
                .NMethod2()
                .NMethod3()
                .NMethod1();

            Console.WriteLine("Hello World!");
        }

        static void Test()
        {
            var x = new TestObject();


            x.Method1()
            .Method2()
            .Method3()
            .Method4()
            .NextObject()
            .NMethod1()
            .NMethod2()
            .NMethod3()
            .NMethod1();
        }


        static void test2(string[] args)
        {
            var test = new TestDataObject();
            
            var x = new TestObject()
                .Method5(test.name, test.age)
                .Method6(test.name)
                .Method4();

            Console.WriteLine("Hello World!");
        }

    }

    public class TestDataObject
    {
        public string name;
        public string age;
    }
}//ChainAnalyzer
