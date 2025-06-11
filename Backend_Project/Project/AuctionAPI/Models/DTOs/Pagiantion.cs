namespace AuctionAPI.Models.DTOs
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public PaginationMetadata Pagination { get; set; } = new PaginationMetadata();
    }

    public class PaginationMetadata
    {
        public int TotalRecords { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}