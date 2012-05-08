#pragma warning disable 169 // ReSharper disable StaticFieldInGenericType, InconsistentNaming, CheckNamespace
using System;

namespace Machine.Specifications
{
    public abstract class ContextAndResult<TSubject, TResult>
    {
        protected static Exception the_exception;
        protected static TSubject subject;
        protected static TResult result;

        /// <summary> Use like <code>Because it = ShouldFail(() => subject.do(something));</code> </summary>
        protected static Because ShouldFail(Action throwingCase)
        {
            return () => { the_exception = Catch.Exception(throwingCase); };
        }
    }

    public abstract class ContextOf<TSubject> : ContextAndResult<TSubject, object> { }
}