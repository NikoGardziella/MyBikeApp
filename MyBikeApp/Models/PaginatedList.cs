using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;

namespace MyBikeApp.Models
{
    public class PaginatedList <T> : List<T>
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            pageIndex = PageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }
        public bool PreviousPage
        {
            get
            {
                return(PageIndex > 1);
            }
        }
        public bool NextPage
        {
            get
            {
                return (PageIndex > TotalPages);
            }
        }
        public static async Task<PaginatedList<T>> CreateASync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items,count, pageIndex, pageSize);
        }

    }
}
