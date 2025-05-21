namespace WholeApplication2.Exceptions
{
    public class CollectionEmptyException : Exception
    {
        string _msg;
        public CollectionEmptyException(string msg) { 
            _msg = msg;
        }
        public override string ToString() => _msg;
    }
}
