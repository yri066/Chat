namespace Chat.ConsoleWorker.Interface
{
    public interface IWorker
    {
        public string Name { get; }
        public Task Run();
    }
}
