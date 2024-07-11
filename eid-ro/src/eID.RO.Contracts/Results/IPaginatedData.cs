namespace eID.RO.Contracts.Results
{
    public interface IPaginatedData<T>
    {
        /// <summary>
        /// Page index
        /// </summary>
        public int PageIndex { get; }
        
        /// <summary>
        /// Total item
        /// </summary>
        public int TotalItems { get; }
        
        /// <summary>
        /// Return data
        /// </summary>
        public IEnumerable<T> Data { get; }
    }
}
