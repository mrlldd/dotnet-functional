﻿using System;
using mrlldd.Functional.Result.Exceptions;

namespace mrlldd.Functional.Result
{
    public abstract class Result<T>
    {
        public abstract bool Successful { get; }

        public static implicit operator T(Result<T> result)
            => result.Successful
                ? ((Success<T>) result).Value
                : throw new ResultUnwrapException("Can't extract value from result as it's not successful.");

        public static implicit operator Exception(Result<T> result)
            => result.Successful
                ? throw new ResultUnwrapException("Can't extract exception from result as it's successful.")
                : ((Fail<T>) result).Exception;

        public static implicit operator Result<T>(Exception exception)
            => new Fail<T>(exception);

        public static implicit operator Result<T>(T value)
            => new Success<T>(value);
    }
}