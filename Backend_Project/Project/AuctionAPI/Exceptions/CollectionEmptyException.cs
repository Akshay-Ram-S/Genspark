namespace AuctionAPI.Exceptions
{
    public class CollectionEmptyException : Exception
    {
        public string _message = "No entities found in database";

        public CollectionEmptyException(string msg)
        {
            _message = msg;
        }
        public override string Message => _message;
    }
}