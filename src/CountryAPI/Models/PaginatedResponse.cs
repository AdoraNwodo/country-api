namespace CountryAPI
{
    public class PaginatedResponse<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public bool HasNext { get; set; }
        public IEnumerable<T>? Data { get; set; }
    }
}

