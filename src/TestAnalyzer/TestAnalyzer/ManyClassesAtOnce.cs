namespace TestAnalyzer
{
    class ManyClassesAtOnce
    {

        public void PassingClass()
        {
            var x = new FirstClass().GoToSecondClass();
        }

        public void FailingClass()
        {
            var x = new FirstClass().GoToSecondClass().GoToThirdClass();
        }

        public void PassingField()
        {
            var x = new SecondClass();
            var z = x.IntFieldSC;
        }

        public void FailingField()
        {
            var x = new FirstClass();
            var z = x.GoToSecondClass()
                .GoToThirdClass()
                .IntFieldTC;
        }

        public void PassingProperty()
        {
            var x = new SecondClass().thirdClass;
        }


        public void FailingProperty()
        {
            var x = new FirstClass()
                .GoToSecondClass()
                .thirdClass;
        }

        public void PassingMultiChain()
        {
            new FirstClass()
                .StayInFirstClass()
                .GoToSecondClass();
        }

        public void FailingMultiChain()
        {
            new FirstClass()
                .StayInFirstClass()
                .GoToSecondClass()
                .StayInSecondClass()
                .GoToThirdClass();
        }
        public void MultiChain()
        {
            var a = new FirstClass().GoToSecondClass();
            var b = new SecondClass().GoToThirdClass();
            var c = new SecondClass().thirdClass;
            var d = a.IntFieldSC;
        }

        public void ChainWithParams()
        {
            var a = new FirstClass()
                .StayInFirstClass(1)
                .GoToSecondClass(2);
        }
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

        public SecondClass GoToSecondClass(int i)
        {
            return new SecondClass();
        }

        public FirstClass StayInFirstClass(int i)
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
}
