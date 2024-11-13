namespace Fukicycle.Tool.Firebase.Realtime.Database.Extention.Models
{
    internal sealed class SubscribeResponse<T>
    {
        public SubscribeResponse(
            string path,
            T @object)
        {
            Path = path;
            Object = @object;
        }
        public string Path { get; }
        public T Object { get; }
    }
}
