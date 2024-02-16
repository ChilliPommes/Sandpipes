using NUnit.Framework;
using Sandpipes.TPLDataflow;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Sandpipes.Test.TPLDataflowTests
{
    public class DataflowSandPipelineTests
    {
        DataflowSandPipeline pipeline;

        [SetUp]
        public void SetUp()
        {
            pipeline = new DataflowSandPipeline();
        }

        [Test]
        public void Constructor_Test()
        {
            var result = new DataflowSandPipeline();

            Assert.IsNotNull(result);
        }

        [Test]
        public void AddStep_Test()
        {
            Assert.DoesNotThrow(() => pipeline.AddOneToOne<string, string>(s => ""));
        }

        [Test]
        public void AddStepAsync_Test()
        {
            Assert.DoesNotThrow(() => pipeline.AddOneToOne<string, Task<string>>(s => Task.FromResult("")));
        }

        [Test]
        public void CreatePipeline_Test()
        {
            pipeline.AddOneToOne<string, string>(s => "");

            Assert.DoesNotThrow(() => pipeline.CreatePipeline<string>(s => { int a = 5; }));
        }

        [Test]
        public void CreatePipeline_Raise_Exception_Test()
        {
            Assert.Throws(typeof(InvalidOperationException),() => pipeline.CreatePipeline<string>(s => { int a = 5; }));
        }

        [Test]
        public async Task Execute_Test()
        {
            string result = "";

            pipeline.AddOneToOne<string, string>(s => s.Replace("a", ""));
            pipeline.AddOneToOne<string, int>(s => s.Length);
            pipeline.AddOneToOne<int, string>(x => x.ToString());
            pipeline.AddOneToOneAsync<string, string>(x => Task.FromResult("test"));
            pipeline.AddOneToOne<string, bool>(x => true);

            pipeline.CreatePipeline<bool>(s => { result = s.ToString(); });

            pipeline.Execute("Hallo mein Name ist Hans.");

            await Task.Delay(500);

            Assert.AreEqual("22", result);
        }
    }
}
