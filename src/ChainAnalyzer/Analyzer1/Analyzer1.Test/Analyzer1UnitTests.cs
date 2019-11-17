using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using ChainAnalyzer;

namespace ChainAnalyzer.Test
{
    [TestClass]
    public class ChainLenghtTest : CodeFixVerifier
    {
        private const string ChainTooLong = @"
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
        }
    }
}";

        private const string ChainTooLongNoDeclaration = @"
using System;

namespace TestAnalyzer
{
    class Program
    {
        static void Main(string[] args)
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
    }
}";
        private const string ChainTooLongStatic= @"
using System;

namespace TestAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = TestObject.Method1()
                .Method2()
                .Method3()
                .Method4()
                .NextObject()
                .NMethod1()
                .NMethod2()
                .NMethod3()
                .NMethod1();
        }
    }
}";
        private const string ChainProperLength = @"
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
                .NextObject();
        }
    }
}";

        private const string ChainProperLengthWithArguments = @"
using System;

namespace TestAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = new TestObject()
                .Method5(test.name, test.age)
                .Method6(test.name)
                .Method4();
        }
    }
}";

        //No diagnostics expected to show up
        [DataTestMethod]
        [
            DataRow(""),
            DataRow(ChainProperLength),
            DataRow(ChainProperLengthWithArguments)]
        public void WhenTestCodeIsValidNoDiagnosticIsTriggered(string testCode)
        {
            VerifyCSharpDiagnostic(testCode);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [DataTestMethod]
        [DataRow(ChainTooLong, 10,13),
         DataRow(ChainTooLongNoDeclaration, 12, 17),
         DataRow(ChainTooLongStatic,10,13)]
        public void WhenDiagnosticIsRaised(string testCode, int line, int column)
        {

            var expected = new DiagnosticResult
            {
                Id = ChainLenghtAnalyzer.DiagnosticId,
                Message = new LocalizableResourceString(nameof(ChainAnalyzer.Resources.AnalyzerMessageFormat), ChainAnalyzer.Resources.ResourceManager, typeof(ChainAnalyzer.Resources)).ToString(),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", line, column)
                        }
            };

            VerifyCSharpDiagnostic(testCode, expected);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new Analyzer1CodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ChainLenghtAnalyzer();
        }
    }
}
