namespace WholeApplication2.Exceptions
{
    public class DuplicateEntityException : Exception
    {
        string _msg;
        public DuplicateEntityException(string msg) {
            _msg = msg;
        }
        public override string ToString() => _msg;
    }
}
