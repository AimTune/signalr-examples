using Microsoft.AspNetCore.SignalR;

namespace backend.Hubs
{
    public class DrawingHub : Hub<IDrawingHubClient>
    {
        public void DrawColor(string color, int x, int y, int lineWidth = 3)
        {
            Clients.Others.DrawColor(color, x, y, lineWidth);
        }

        public void Erase(int x, int y)
        {
            Clients.Others.Erase(x, y);
        }
    }
}
