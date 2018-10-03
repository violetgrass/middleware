namespace VioletGrass.Middleware
{
    public class Message
    {
        public Message(string routingKey, string body)
        {
            RoutingKey = routingKey ?? throw new System.ArgumentNullException(nameof(routingKey));
            Body = body ?? throw new System.ArgumentNullException(nameof(body));
        }

        public string RoutingKey { get; }
        public string Body { get; }
    }
}