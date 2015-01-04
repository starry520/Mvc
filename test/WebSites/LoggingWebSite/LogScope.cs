using System;
#if ASPNET50
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
#endif
using System.Threading;

namespace LoggingWebSite
{
    public class LogScope
    {
        public LogScope(object state)
        {
            State = state;
        }

        public object State { get; private set; }

        public LogScope ParentScope { get; set; }

        /// <summary>
        /// Representation of this scope as a node in the tree. 
        /// </summary>
        public ScopeNode ScopeNode { get; set; }

#if ASPNET50
        private static string FieldKey = typeof(LogScope).FullName + ".Value";

        public static LogScope CurrentScope
        {
            get
            {
                var handle = CallContext.LogicalGetData(FieldKey) as ObjectHandle;

                if (handle == null)
                {
                    return default(LogScope);
                }

                return (LogScope)handle.Unwrap();
            }
            set
            {
                CallContext.LogicalSetData(FieldKey, new ObjectHandle(value));
            }
        }
#else
        private static AsyncLocal<LogScope> _value = new AsyncLocal<LogScope>();
        public static LogScope CurrentScope
        {
            set
            {
                _value.Value = value;
            }
            get
            {
                return _value.Value;
            }
        }
#endif

        public static IDisposable Push(LogScope newScope, TestSink sink)
        {
            var oldScope = CurrentScope;
            CurrentScope = newScope;
            CurrentScope.ParentScope = oldScope;

            CurrentScope.ScopeNode = new ScopeNode()
            {
                State = CurrentScope.State,
                StateType = CurrentScope.State?.GetType()
            };

            if (CurrentScope.ParentScope != null)
            {
                CurrentScope.ParentScope.ScopeNode.Children.Add(CurrentScope.ScopeNode);
            }
            else
            {
                sink.LogEntries.Add(CurrentScope.ScopeNode);
            }

            return new DisposableAction(() =>
            {
                CurrentScope = CurrentScope.ParentScope;
            });
        }

        private class DisposableAction : IDisposable
        {
            private Action _action;

            public DisposableAction(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                if (_action != null)
                {
                    _action.Invoke();
                    _action = null;
                }
            }
        }
    }
}