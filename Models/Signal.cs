namespace Models
{
    public record class Signal(string SignalR, string ClientId, string FileName)
    {
        public override string ToString()
        {
            return $"{SignalR};{ClientId};{FileName}";
        }
    };
}