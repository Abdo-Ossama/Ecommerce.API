using Ecommerce.API.DTOs.Requests;

namespace Ecommerce.API.Services.IServices
{
    public interface IReviewService
    {

        Task<IEnumerable<UserReview>> GetAsync(int productId);
        Task<UserReview> CreateAsync(ReviewCreateRequest reviewCreateRequest);
        Task<UserReview> UpdateAsync(int id, ReviewUpdateRequest reviewUpdateRequest);
        Task<bool> DeleteAsync(int id);
    }
}
