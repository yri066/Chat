namespace Chat.Domain.Interface.ConsoleWorker
{
    public interface IWorker
    {
        public string Name { get; }
        public Task Run();
    }
}
