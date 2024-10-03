namespace oneSpanDemo.Models
{
    public interface Iedocument
    {
        public Task<DocumentsResult> GetOneSpanDocument(string transaction);
    }
}
