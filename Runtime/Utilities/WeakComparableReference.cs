using System;

namespace MoonriseGames.Connect.Utilities
{
    internal class WeakComparableReference<T>
        where T : class
    {
        private WeakReference Reference { get; }

        public T Target => Reference.Target as T;

        public WeakComparableReference(T target) => Reference = new WeakReference(target);

        public override bool Equals(object obj)
        {
            if (obj is WeakComparableReference<T> reference)
                obj = reference.Target;
            return obj?.Equals(Target) ?? Target == null;
        }

        public override int GetHashCode() => Target?.GetHashCode() ?? 0;

        public static implicit operator WeakComparableReference<T>(T target) => new(target);

        public static implicit operator T(WeakComparableReference<T> reference) => reference.Target;
    }
}
