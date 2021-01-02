using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Functional.Result.Internal.Utilities
{
    internal static class Execute
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result Safely(Action effect)
        {
            try
            {
                effect();
                return Result.Success;
            }
            catch (Exception e)
            {
                return e;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result Safely<T>(Action<T> effect, T argument)
        {
            try
            {
                effect(argument);
                return Result.Success;
            }
            catch (Exception e)
            {
                return e;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result Safely<T>(Action<T, CancellationToken> effect, T argument, CancellationToken cancellationToken)
        {
            try
            {
                effect(argument, cancellationToken);
                return Result.Success;
            }
            catch (Exception e)
            {
                return e;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Safely<T>(Func<T> factory)
        {
            try
            {
                return factory();
            }
            catch (Exception e)
            {
                return e;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Safely<T>(Func<CancellationToken, T> factory, CancellationToken cancellationToken)
        {
            try
            {
                return factory(cancellationToken);
            }
            catch (Exception e)
            {
                return e;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TMapped> Safely<T, TMapped>(T source, Func<T, CancellationToken, TMapped> mapper,
            CancellationToken cancellationToken)
        {
            try
            {
                return mapper(source, cancellationToken);
            }
            catch (Exception e)
            {
                return e;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TMapped> Safely<T, TMapped>(T source, Func<T, TMapped> mapper)
        {
            try
            {
                return mapper(source);
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}