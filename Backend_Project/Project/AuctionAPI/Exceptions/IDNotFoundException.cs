namespace AuctionAPI.Exceptions
{
    public class IdNotFoundException : Exception
    {
        public string _message = "No entitiy found with given ID";

        public IdNotFoundException(string msg)
        {
            _message = msg;
        }
        public override string Message => _message;
    }
}