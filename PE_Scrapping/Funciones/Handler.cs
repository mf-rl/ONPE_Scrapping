using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;


namespace PE_Scrapping.Funciones
{
    public static class Handler
    {
        public static Action DoNothing() { return () => { }; }
        public static void ExecuteAction(Action action)
        {
            try { action(); }
            catch (Exception excepcion)
            {
                bool reThrow = ExceptionPolicy.HandleException(excepcion, Constants.PROPAGATE_POLICY_NAME);
                if (reThrow) throw;
            }
        }
        public static void ExecuteActionIf(Action action, Func<bool> condition, Action actionElse = null)
        {
            try {
                if (condition())
                    action();
                else actionElse?.Invoke();
            }
            catch (Exception excepcion)
            {
                bool reThrow = ExceptionPolicy.HandleException(excepcion, Constants.PROPAGATE_POLICY_NAME);
                if (reThrow) throw;
            }
        }
        public static void ExecuteAction(Action action, Action<Exception> actionException)
        {
            try { action(); }
            catch (Exception exception)
            {
                actionException(exception);
            }
        }
        public static T ExecuteFunction<T>(Func<T> function)
        {
            T functionValue = default;
            try { functionValue = function(); }
            catch (Exception exception)
            {
                bool reThrow = ExceptionPolicy.HandleException(exception, Constants.PROPAGATE_POLICY_NAME);
                if (reThrow) throw;
            }
            return functionValue;
        }
        public static void ExecuteActions(List<Action> actions)
        {
            try
            {
                foreach (Action action in actions) { action(); }
            }
            catch (Exception exception)
            {
                bool reThrow = ExceptionPolicy.HandleException(exception, Constants.PROPAGATE_POLICY_NAME);
                if (reThrow) throw;
            }
        }
        public static void ExecuteActionsIf(List<Action> actions, Func<bool> condition)
        {
            try
            {
                foreach (Action action in actions)
                {
                    action();
                    if (!condition()) break;
                }
            }
            catch (Exception exception)
            {
                bool reThrow = ExceptionPolicy.HandleException(exception, Constants.PROPAGATE_POLICY_NAME);
                if (reThrow) throw;
            }
        }
        public static void RepeatActionIf(Action action, Func<bool> condition)
        {
            try
            {
                while (condition())
                {
                    action();
                }
            }
            catch(Exception exception)
            {
                bool reThrow = ExceptionPolicy.HandleException(exception, Constants.PROPAGATE_POLICY_NAME);
                if (reThrow) throw;
            }
        }
        public static void ExecuteParallelActions(List<Action> actions)
        {
            OrderablePartitioner<Action> partition = Partitioner.Create(actions);
            ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
            Parallel.ForEach(partition, parallelOptions,
                (action, loopState) => {
                    try { action(); }
                    catch (Exception) { loopState.Break(); throw; }
                });
        }
        public static void ExecuteParallelActions(List<Action> actions, Action<AggregateException> actionException)
        {
            try
            {
                Task[] tasks =
                    actions.Select(action =>
                        new Task(action)).ToArray();
                foreach (Task task in tasks) task.Start();
                Task.WhenAll(tasks);
            }
            catch (AggregateException exception)
            {
                actionException(exception);
            }
        }
        public static string GetUserInput(string messageTitle, string inputRequired = "", string currentValue = "")
        {
            Console.WriteLine(messageTitle);
            return (
                (string.IsNullOrEmpty(inputRequired) ? Console.ReadLine() :
                    (Console.ReadLine() == inputRequired ? inputRequired : currentValue)
                )
            );
        }
        public static void WriteLines(string[] messageList)
        {
            messageList.ToList().ForEach(message => { Console.WriteLine(message); });
        }
        public static string FormatFileName(string file_name)
        {
            Path.GetInvalidFileNameChars().ToList().ForEach(c =>
            {
                file_name = file_name.Replace(c, '-');
            });
            return file_name.Trim();
        }
    }
}