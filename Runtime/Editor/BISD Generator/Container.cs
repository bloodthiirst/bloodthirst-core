namespace Bloodthirst.Core.BISD.CodeGeneration
{
    public class Container<T>
    {
        public string ModelName { get; set; }
        public T Behaviour { get; set; }
        public T Instance { get; set; }
        public T State { get; set; }
        public T Data { get; set; }
    }
}
