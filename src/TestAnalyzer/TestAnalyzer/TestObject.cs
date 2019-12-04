using System;
using System.Collections.Generic;
using System.Text;

namespace TestAnalyzer
{
    public class TestObject
    {
        public TestObject Method1()
        {
            return this;
        }

        public TestObject Method2()
        {
            return this;
        }


        public TestObject Method3()
        {
            return this;
        }


        public TestObject Method4()
        {
            return this;
        }

        public TestObject Method5(string name, string age)
        {
            return this;
        }



        public TestObject Method6(string name)
        {
            return this;
        }

        public NextObject NextObject()
        {
            return new NextObject();
        }
    }
}
