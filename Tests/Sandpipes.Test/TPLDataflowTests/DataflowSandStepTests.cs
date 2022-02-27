using NUnit.Framework;
using Sandpipes.TPLDataflow;
using System.Threading.Tasks.Dataflow;

namespace Sandpipes.Test.TPLDataflowTests
{
    public class DataflowSandStepTests
    {
        [Test]
        public void Constructor_One_Param_Test()
        {
            var result = new DataflowSandStep(new TransformBlock<string, string>(s => ""));

            Assert.IsNotNull(result);

            Assert.IsNotNull(result.Block);
            Assert.IsFalse(result.IsAsync);
        }

        [Test]
        public void Constructor_Two_Param_Test()
        {
            var result = new DataflowSandStep(new TransformBlock<string, string>(s => ""), true);

            Assert.IsNotNull(result);

            Assert.IsNotNull(result.Block);
            Assert.IsTrue(result.IsAsync);
        }
    }
}
