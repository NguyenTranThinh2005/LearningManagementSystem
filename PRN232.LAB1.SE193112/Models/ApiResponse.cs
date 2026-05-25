namespace PRN232.LAB1.SE193112.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ApiResponse<T> Ok(T data, string message = "Request processed successfully")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = new()
            };
        }

        public static ApiResponse<T> Created(T data, string message = "Resource created successfully")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = new()
            };
        }

        public static ApiResponse<T> BadRequest(string error)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = "Bad Request",
                Data = default,
                Errors = new() { error }
            };
        }

        public static ApiResponse<T> NotFound(string error)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = "Not Found",
                Data = default,
                Errors = new() { error }
            };
        }

        public static ApiResponse<T> InternalError(string error)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = "Internal Server Error",
                Data = default,
                Errors = new() { error }
            };
        }
    }

    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public PaginationMetadata Pagination { get; set; }
    }

    public class PaginationMetadata
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }

    public class QueryParameters
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = string.Empty;
        public bool Ascending { get; set; } = true;
        public string Search { get; set; } = string.Empty;
        public string Fields { get; set; } = string.Empty; // CSV of fields to select
        public string Expand { get; set; } = string.Empty; // CSV of related entities to include

        public QueryParameters()
        {
            // Validate page and pagesize
            if (Page < 1) Page = 1;
            if (PageSize < 1) PageSize = 10;
            if (PageSize > 100) PageSize = 100; // Max 100 items per page
        }
    }
}
