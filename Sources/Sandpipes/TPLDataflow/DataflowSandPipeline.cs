using System.Threading.Tasks.Dataflow;

namespace Sandpipes.TPLDataflow
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DataflowSandPipeline
    {
        private List<DataflowSandStep> _steps = new List<DataflowSandStep>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="stepFunc"></param>
        public void AddStep<TInput, TOutput>(Func<TInput, TOutput> stepFunc)
        {
            if (_steps.Count == 0)
            {
                var block = new TransformBlock<TInput, TOutput>(stepFunc);
                _steps.Add(new DataflowSandStep(block));
            }
            else
            {

                var lastStep = _steps.Last();
                if (!lastStep.IsAsync)
                {
                    var step = new TransformBlock<TInput, TOutput>(stepFunc);
                    var targetBlock = (lastStep.Block as ISourceBlock<TInput>);

                    if (targetBlock == null)
                    {
                        throw new InvalidOperationException("Last step could not be found, new step cannot be added.", new NullReferenceException("targetBlock is null"));
                    }

                    targetBlock.LinkTo(step, new DataflowLinkOptions() { PropagateCompletion = true });
                    _steps.Add(new DataflowSandStep(step));
                }
                else
                {
                    var step = new TransformBlock<Task<TInput>, TOutput>(async (input) => stepFunc(await input));
                    var targetBlock = (lastStep.Block as ISourceBlock<Task<TInput>>);

                    if (targetBlock == null)
                    {
                        throw new InvalidOperationException("Last step could not be found, new step cannot be added.", new NullReferenceException("targetBlock is null"));
                    }

                    targetBlock.LinkTo(step, new DataflowLinkOptions() { PropagateCompletion = true });
                    _steps.Add(new DataflowSandStep(step));
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="stepFunc"></param>
        public void AddStepAsync<TInput, TOutput>(Func<TInput, Task<TOutput>> stepFunc)
        {
            if (_steps.Count == 0)
            {
                var step = new TransformBlock<TInput, Task<TOutput>>(async (input) => await stepFunc(input));
                _steps.Add(new DataflowSandStep(step, true));
            }
            else
            {
                var lastStep = _steps.Last();
                if (lastStep.IsAsync)
                {
                    var step = new TransformBlock<Task<TInput>, Task<TOutput>>(async (input) => await stepFunc(await input));
                    var targetBlock = (lastStep.Block as ISourceBlock<Task<TInput>>);

                    if (targetBlock == null)
                    {
                        throw new InvalidOperationException("Last step could not be found, new step cannot be added.", new NullReferenceException("targetBlock is null"));
                    }

                    targetBlock.LinkTo(step, new DataflowLinkOptions() { PropagateCompletion = true });
                    _steps.Add(new DataflowSandStep(step, true));
                }
                else
                {
                    var step = new TransformBlock<TInput, Task<TOutput>>(async (input) => await stepFunc(input));
                    var targetBlock = (lastStep.Block as ISourceBlock<TInput>);

                    if (targetBlock == null)
                    {
                        throw new InvalidOperationException("Last step could not be found, new step cannot be added.", new NullReferenceException("targetBlock is null"));
                    }

                    targetBlock.LinkTo(step, new DataflowLinkOptions() { PropagateCompletion = true });
                    _steps.Add(new DataflowSandStep(step, true));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="resultCallback"></param>
        public Task CreatePipeline<TOutput>(Action<TOutput> resultCallback)
        {
            var lastStep = _steps.Last();

            if (lastStep == null)
            {
                throw new InvalidOperationException("Pipeline is empty and cannot add a result callback", new NullReferenceException("lastStep is null"));
            }

            if (lastStep.IsAsync)
            {
                var targetBlock = (lastStep.Block as ISourceBlock<Task<TOutput>>);

                if (targetBlock == null)
                {
                    throw new InvalidOperationException("Pipeline is empty and cannot add a result callback", new NullReferenceException("targetBlock is null"));
                }

                var callBackStep = new ActionBlock<Task<TOutput>>(async t => resultCallback(await t));
                targetBlock.LinkTo(callBackStep, new DataflowLinkOptions { PropagateCompletion = true });

                return callBackStep.Completion;
            }
            else
            {
                var targetBlock = (lastStep.Block as ISourceBlock<TOutput>);

                if (targetBlock == null)
                {
                    throw new InvalidOperationException("Pipeline is empty and cannot add a result callback", new NullReferenceException("targetBlock is null"));
                }

                var callBackStep = new ActionBlock<TOutput>(t => resultCallback(t));

                targetBlock.LinkTo(callBackStep,new DataflowLinkOptions { PropagateCompletion = true });

                return callBackStep.Completion;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <param name="input"></param>
        public void Execute<TInput>(TInput input)
        {
            var firstStep = _steps[0].Block as ITargetBlock<TInput>;

            if (firstStep == null)
            {
                throw new InvalidOperationException("Pipeline is empty and cannot be executed", new NullReferenceException("firstStep is null"));
            }

            firstStep.SendAsync(input);
        }

        /// <summary>
        /// Completes the first step to signalize an stop of data input
        /// </summary>
        public void CompleteFirstStep()
        {
            _steps[0].Block.Complete();
        }
    }
}
