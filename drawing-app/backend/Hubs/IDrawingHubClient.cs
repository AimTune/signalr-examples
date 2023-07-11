namespace backend.Hubs
{
    public interface IDrawingHubClient
    {
        Task DrawColor(string color, int x, int y, int lineWidth);
        Task Erase(int x, int y);
        Task EraseAllScreen();
    }
}
