using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKMusic.Tests {
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class TestClass {
        private TestContext testContextInstance;
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }
        [TestMethod]
         public void MyTestMethod() {
            
        }
    }
}
