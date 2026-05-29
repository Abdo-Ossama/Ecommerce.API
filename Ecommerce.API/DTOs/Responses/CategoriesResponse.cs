namespace EcommerceProject.ViewModel
{
    public class CategoriesResponse
    {
        public List<Category> categoriesQuery { get; set; }
        public List<Category> categories { get; set; }
        public double totalPages { get; set; }
        public double currentPage { get; set; }
        public double pageSize { get; set; }
    
         
    }
}
