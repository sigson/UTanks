using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

namespace SecuredSpace.ClientControl.JobWrapper
{
    public class ProxyJob 
    {
        JobTaskObject jobHandler;
        public delegate void JobCallbackDelegate<T>(NativeArray<T> data) where T : struct;
    }

    public interface JobTaskObject
    {
        public enum State
        {
            Created,
            Working,
            Executed
        }

        public State JobState { get; set; }
        public void OnFinish();
    }

    public interface StructAction<T> where T : struct { public void Execute(NativeArray<T> data); }
    public interface StructFunction<T, T1> where T : struct where T1 : struct { public NativeArray<T> Execute(NativeArray<T1> data); }

    [BurstCompile]
    public struct JobTask<Tin, Tout> : JobTaskObject, IJob where Tin : struct where Tout : struct
    {
        [WriteOnly]
        public NativeArray<Tout> resultArray;
        [ReadOnly]
        public NativeArray<Tin> inputArray;
        [ReadOnly]
        public StructFunction<Tout, Tin> task;
        public FunctionPointer<ProxyJob.JobCallbackDelegate<Tout>> taskCallbackOnExecuted;

        private JobTaskObject.State local_JobState;
        public JobTaskObject.State JobState { get => local_JobState; set => local_JobState = value; }

        public void Execute()
        {
            resultArray = task.Execute(inputArray);
        }

        public void OnFinish()
        {
            taskCallbackOnExecuted.Invoke(resultArray);
        }

        public void Setup()
        {
            resultArray = task.Execute(inputArray);
        }
    }
}
