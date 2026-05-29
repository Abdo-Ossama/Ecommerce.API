using Ecommerce.API.DTOs.Requests;
using Ecommerce.API.Helpers;
using Ecommerce.API.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IFileHandler _fileHandler;
        private readonly IRepository<UserReview> _userReview;

        public ReviewService(IFileHandler fileHandler, IRepository<UserReview> userReview)
        {
            _fileHandler = fileHandler;
            _userReview = userReview;
        }

        public async Task<IEnumerable<UserReview>> GetAsync(int productId)
        {
            var reviews = await _userReview.GetAsync(e => e.ProductId == productId);

            return reviews;
        }

        public async Task<UserReview> CreateAsync([FromForm] ReviewCreateRequest reviewCreateRequest)
        {
            // TODO: check if user and product exist

            var imgFileName = await _fileHandler.CreateFileAsync(reviewCreateRequest.Img, FileType.ReviewImg);    

            var review = new UserReview
            {
                ProductId = reviewCreateRequest.ProductId,
                ApplicationUserId = reviewCreateRequest.ApplicationUserId,
                Comment = reviewCreateRequest.Comment,
                Rate = reviewCreateRequest.Rate,
                Img = imgFileName
            };

            await _userReview.CreateAsync(review);
            await _userReview.CommitAsync();

            return review;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var review = await _userReview.GetOneAsync(e => e.Id == id);

            if (review is null)
            {
                throw new Exception("Review not found");
            }

            _fileHandler.DeleteOldFile(review.Img, FileType.ReviewImg);

            _userReview.delete(review);
            await _userReview.CommitAsync();

            return true;
        }
       

        public async Task<UserReview> UpdateAsync(int id, [FromForm] ReviewUpdateRequest reviewUpdateRequest)
        {
            var review = await _userReview.GetOneAsync(e => e.Id == id);

            if(review is null)
            {
                throw new Exception("Review not found");
            }

            if (reviewUpdateRequest.Img is not null)
            {
                _fileHandler.DeleteOldFile(review.Img, FileType.ReviewImg);
                var imgFileName = await _fileHandler.CreateFileAsync(reviewUpdateRequest.Img, FileType.ReviewImg);
                review.Img = imgFileName;
            }

            //review.ApplicationUserId = reviewUpdateRequest.ApplicationUserId;
            //review.ProductId = reviewUpdateRequest.ProductId;
            review.Comment = reviewUpdateRequest.Comment;
            review.Rate = reviewUpdateRequest.Rate;

            _userReview.Update(review);
            await _userReview.CommitAsync();

            return review;
        }
    }
}
