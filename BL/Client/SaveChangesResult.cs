using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public enum SaveChangesResultType
    {
        Ok, Error, NeedUpdate
    }

    public struct SaveChangesResult
    {
        public SaveChangesResult(bool result, Exception exception, SaveChangesResultType resultType)
        {
            Result = result;
            Exception = exception;
            ResultType = resultType;
        }

        public bool Result { get; }

        public Exception Exception { get; }

        public SaveChangesResultType ResultType { get; }
    }
}
