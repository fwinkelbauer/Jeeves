namespace Jeeves.Client
{
    public class JeevesConfiguration<T>
    {
        public int Revision { get; set; }

        public T Data { get; set; }
    }
}
