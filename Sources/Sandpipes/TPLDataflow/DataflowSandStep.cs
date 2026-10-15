using System.Threading.Tasks.Dataflow;

namespace Sandpipes.TPLDataflow
{
    /// <summary>
    /// Represents a single processing step within a TPL Dataflow pipeline.
    /// </summary>
    public sealed class DataflowSandStep
    {
        /// <summary>
        /// Gets the underlying <see cref="IDataflowBlock"/> associated with this pipeline step.
        /// </summary>
        public IDataflowBlock Block  { get; }

        /// <summary>
        /// Gets a value indicating whether the step executes asynchronously.
        /// </summary>
        public bool IsAsync { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataflowSandStep"/> class with the specified dataflow block.
        /// </summary>
        /// <param name="block">The dataflow block that represents this pipeline step.</param>
        public DataflowSandStep(IDataflowBlock block)
        {
            Block = block;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataflowSandStep"/> class with the specified dataflow block and asynchronous execution flag.
        /// </summary>
        /// <param name="block">The dataflow block that represents this pipeline step.</param>
        /// <param name="isAsync"><see langword="true"/> if the step executes asynchronously; otherwise, <see langword="false"/>.</param>
        public DataflowSandStep(IDataflowBlock block, bool isAsync)
        {
            Block = block;
            IsAsync = isAsync;
        }
    }
}