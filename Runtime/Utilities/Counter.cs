namespace MoonriseGames.Connect.Utilities
{
    internal class Counter
    {
        public ulong Value { get; private set; }
        public ulong StartValue { get; }

        public Counter(ulong startValue = 0)
        {
            StartValue = startValue;
            Value = startValue;
        }

        public ulong ReadAndIncrease() => Value++;

        public void Reset() => Value = StartValue;
    }
}
