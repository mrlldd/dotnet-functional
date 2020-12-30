using System;

namespace Functional.Tests.Core.Internal.Extensions
{
    internal static class TestsCoreLevelObjectExtensions
    {
        public static T SideEffect<T>(this T source, Action<T> effect)
        {
            effect(source);
            return source;
        }

        public static T SideEffects<T>(this T source, params Action<T>[] effects)
        {
            foreach (var effect in effects)
            {
                effect(source);
            }

            return source;
        }

        public static TMapped Map<T, TMapped>(this T source, Func<T, TMapped> mapper) 
            => mapper(source);
    }
}