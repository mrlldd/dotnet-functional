using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Functional.Result.Internal.Utilities
{
    internal partial class Execute
    {
        private Execute()
        {
        }
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result> SafelyAsync(Func<Task> asyncAction)
        {
            try
            {
                await asyncAction();
                return Result.Success;
            }
            catch (Exception e)
            {
                return e;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result> SafelyAsync(Func<CancellationToken, Task> asyncAction, CancellationToken token)
        {
            try
            {
                await asyncAction(token);
                return Result.Success;
            }
            catch (Exception e)
            {
                return e;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T>> SafelyAsync<T>(Func<Task<T>> asyncFactory)
        {
            try
            {
                var r = await asyncFactory();
                return r;
            }
            catch (Exception e)
            {
                return e;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T>> SafelyAsync<T>(Func<CancellationToken, Task<T>> asyncFactory,
            CancellationToken token)
        {
            try
            {
                var r = await asyncFactory(token);
                return r;
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}