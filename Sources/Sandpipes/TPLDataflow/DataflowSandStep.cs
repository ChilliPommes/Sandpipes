using System.Threading.Tasks.Dataflow;

namespace Sandpipes.TPLDataflow
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DataflowSandStep
    {
        /// <summary>
        /// 
        /// </summary>
        public IDataflowBlock Block  { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsAsync { get; } = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        public DataflowSandStep(IDataflowBlock block)
        {
            Block = block;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <param name="isAsync"></param>
        public DataflowSandStep(IDataflowBlock block, bool isAsync)
        {
            Block = block;
            IsAsync = isAsync;
        }
    }
}
