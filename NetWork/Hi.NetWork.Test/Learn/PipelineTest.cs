using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Test {

    [TestClass]
    public class PipelineTest {

        [TestMethod]
        public void pipeline_test() {
            string handling = "aabb1122zzyy";
            StandardPipeline pipeline = new StandardPipeline();
            BasicValve basicValve = new BasicValve();
            SecondValve secondValve = new SecondValve();
            ThirdValve thirdbalve = new ThirdValve();
            pipeline.setBasic(basicValve);
            pipeline.addValve(secondValve);
            pipeline.addValve(thirdbalve);
            pipeline.getFirst().invoke(handling);
        }
    }

    public interface IValve {
        IValve GetNext();
        void setNext(IValve valve);
        void invoke(string handling);
    }

    public interface IPipeline {
        IValve getFirst();
        IValve getBasic();
        void setBasic(IValve valve);
        void addValve(IValve valve);
    }

    public class BasicValve : IValve {

        protected IValve next = null;

        public IValve GetNext() {
            return next;
        }

        public void invoke(string handling) {
            handling = handling.Replace("aa", "bb");
            Console.WriteLine("基础阀门处理完后：" + handling);
        }

        public void setNext(IValve valve) {
            this.next = valve;
        }
    }

    public class SecondValve : IValve {

        protected IValve next = null;

        public IValve GetNext() {
            return next;
        }

        public void invoke(string handling) {
            handling = handling.Replace("11", "22");
            Console.WriteLine("Second阀门处理完后：" + handling);
            GetNext().invoke(handling);
        }

        public void setNext(IValve valve) {
            this.next = valve;
        }
    }

    public class ThirdValve : IValve {

        protected IValve next = null;

        public IValve GetNext() {
            return next;
        }

        public void invoke(string handling) {
            handling = handling.Replace("zz", "yy");
            Console.WriteLine("Third阀门处理完后：" + handling);
            GetNext().invoke(handling);
        }

        public void setNext(IValve valve) {
            this.next = valve;
        }
    }

    public class StandardPipeline : IPipeline {

        protected IValve first = null;
        protected IValve basic = null;

        public void addValve(IValve valve) {
            if (first == null) {
                first = valve;
                valve.setNext(basic);
            } else {
                IValve current = first;
                while (current != null)
                {
                    if (current.GetNext() == basic) {
                        current.setNext(valve);
                        valve.setNext(basic);
                        break;
                    }
                    current = current.GetNext();
                }
            }
        }

        public IValve getBasic() {
            return basic;
        }

        public IValve getFirst() {
            return first;
        }

        public void setBasic(IValve valve) {
            basic = valve;
        }
    }
}
