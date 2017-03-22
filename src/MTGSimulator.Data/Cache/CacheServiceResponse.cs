namespace MTGSimulator.Data.Cache
{
    public class CacheServiceResponse<T>
    {
        public bool Hit { get; set; }
        public T Value { get; set; }
    }
}