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
    public class ChainReturnTypeTest : CodeFixVerifier
    {
        private const string classes = @"
    namespace TestAnalyzer
{
    class ManyClassesAtOnce
    {
REPLACE

    }

    public class FirstClass
    {
        
        public SecondClass GoToSecondClass()
        {
            return new SecondClass();
        }

        public FirstClass StayInFirstClass()
        {
            return this;
        }
    }


    public class SecondClass
    {
        public ThirdClass thirdClass { get; set; }
        public int IntFieldSC;
        public SecondClass StayInSecondClass()
        {
            return this;
        }

        public ThirdClass GoToThirdClass()
        {
            return new ThirdClass();
        }
    }

    public class ThirdClass
    {
        public int IntFieldTC;
        public ThirdClass TCMethod1()
        {
            return this;
        }
    }
}";

        public const string PassingClass = @"
        public void PassingClass()
        {
            var x = new FirstClass().GoToSecondClass();
        }";

        public const string FailingClass = @"
        public void FailingClass()
        {
            var x = new FirstClass().GoToSecondClass().GoToThirdClass();
        }
";

        public const string PassingField = @"
        public void PassingField()
        {
            var x = new FirstClass();
            var z = x.GoToSecondClass()
                .IntFieldSC;
        }
";

        public const string FailingField = @"
        public void FailingField()
        {
            var x = new FirstClass();
            var z = x.GoToSecondClass()
                .GoToThirdClass()
                .IntFieldTC;
        }
";

        public const string PassingProperty = @"
        public void PassingProperty()
        {
            var x = new SecondClass().thirdClass;
        }
";


        public const string FailingProperty = @"
        public void FailingProperty()
        {
            var x = new FirstClass()
                .GoToSecondClass()
                .thirdClass;
        }
";

        public const string PassingMultiChain = @"
        public void PassingMultiChain()
        {
            new FirstClass()
                .StayInFirstClass()
                .GoToSecondClass();
        }
";

        public const string FailingMultiChain = @"
        public void FailingMultiChain()
        {
            new FirstClass()
                .StayInFirstClass()
                .GoToSecondClass()
                .StayInSecondClass();
        }
";
        //No diagnostics expected to show up
        [DataTestMethod]
        [DataRow("")]
        public void WhenTestCodeIsEmptyNoDiagnosticIsTriggered(string testCode)
        {
            VerifyCSharpDiagnostic(testCode);
        }

        [DataTestMethod]
        [
            DataRow(""),
            DataRow(PassingClass),
            DataRow(PassingProperty),
            DataRow(PassingField),
            DataRow(PassingMultiChain)]
        public void WhenTestCodeIsValidNoDiagnosticIsTriggered(string insert)
        {
            var testCode = BuildTestCode(insert);
            Console.WriteLine(testCode);
            VerifyCSharpDiagnostic(testCode);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [DataTestMethod]
        [DataRow(FailingClass, 10, 13),
         DataRow(FailingField, 10, 13),
         DataRow(FailingProperty, 10, 13),
         DataRow(FailingMultiChain, 10, 13)]
        public void WhenDiagnosticIsRaised(string insert, int line, int column)
        {
            var testCode = BuildTestCode(insert);
            //todo setup proper messeges
            var expected = new DiagnosticResult
            {
                Id = ChainLenghtAnalyzer.DiagnosticId,
                Message = new LocalizableResourceString(nameof(ChainAnalyzer.Resources.ChainReturnTypesAnalyzerMessageFormat), ChainAnalyzer.Resources.ResourceManager, typeof(ChainAnalyzer.Resources)).ToString(),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", line, column)
                        }
            };

            VerifyCSharpDiagnostic(testCode, expected);
        }

        private string BuildTestCode(string insert)
        {
            var test = classes.Replace("REPLACE", insert);
            return test;
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            //return new Analyzer1CodeFixProvider();
            throw new NotImplementedException();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ChainReturnTypeAnalyzer();
        }
    }
}

