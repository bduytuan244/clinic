﻿using clinic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace clinic.Controllers
{
    public class ReviewController : Controller
    {
        dbDClinicDataContext db = new dbDClinicDataContext();
        // GET: Review
        public ActionResult Index(int? page, int? reviewId)
        {
            try
            {
                var customers = db.customers.ToList();
                var comments = db.comments.ToList();
                var reviews = db.reviews.OrderByDescending(r => r.created_at).ToList();

                var reviewList = reviews.Select(r => new ReviewViewModel
                {
                    ReviewId = r.review_id,
                    CustomerId = r.patient_id ?? 0,
                    CustomerName = customers.FirstOrDefault(c => c.patient_id == r.patient_id)?.full_name ?? "Ẩn danh",
                    Title = r.title ?? "Không có tiêu đề",  // Thêm dòng này
                    Rating = r.rating,
                    ReviewText = r.comment,
                    ReviewDate = r.created_at ?? DateTime.MinValue,
                    Comments = comments
        .Where(c => c.review_id == r.review_id)
        .OrderBy(c => c.created_at)
        .Select(c => new CommnentViewModel
        {
            CommentId = c.comment_id,
            ReviewId = c.review_id,
            CustomerId = c.patient_id,
            CustomerName = customers.FirstOrDefault(cus => cus.patient_id == c.patient_id)?.full_name ?? "Ẩn danh",
            CommentText = c.comment_text,
            CreatedAt = c.created_at ?? DateTime.MinValue
        })
        .ToList()
                }).ToList();

                ViewBag.CurrentReviewId = reviewId;

                return View(reviewList);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi tải danh sách đánh giá!";
                return RedirectToAction("Index");
            }
        }


        public ActionResult Create()
        {
            if (Session["CustomerId"] == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để đánh giá!";
                return RedirectToAction("Login", "Account");
            }




            ViewBag.CustomerId = Session["CustomerId"];
            ViewBag.CustomerName = Session["CustomerName"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReviewViewModel model)
        {
            try
            {
                if (Session["CustomerId"] == null)
                {
                    TempData["ErrorMessage"] = "Bạn cần đăng nhập để đánh giá!";
                    return RedirectToAction("Login", "Account");
                }

                int customerId = (int)Session["CustomerId"];

                var newReview = new review
                {
                    patient_id = customerId,
                    title = model.Title,  // Lưu tiêu đề vào database
                    rating = model.Rating ?? 0,
                    comment = model.ReviewText ?? "",
                    created_at = DateTime.Now
                };

                db.reviews.InsertOnSubmit(newReview);
                db.SubmitChanges();

                TempData["SuccessMessage"] = "Đánh giá của bạn đã được gửi!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi: " + ex.Message);
                TempData["ErrorMessage"] = "Có lỗi xảy ra. Vui lòng thử lại!";
                return View(model);
            }
        }

        public ActionResult AddComment(int reviewId)
        {

            ViewBag.ReviewId = reviewId;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(int reviewId, string commentText)
        {
            try
            {
                if (Session["CustomerId"] == null)
                {
                    TempData["ErrorMessage"] = "Bạn cần đăng nhập để bình luận!";
                    return RedirectToAction("Login", "Account");
                }

                if (string.IsNullOrWhiteSpace(commentText) || commentText.Length > 500)
                {
                    TempData["ErrorMessage"] = "Bình luận không được để trống và tối đa 500 ký tự!";
                    return RedirectToAction("Detail", new { reviewId = reviewId });
                }

                int customerId = (int)Session["CustomerId"];

                var reviewExists = db.reviews.Any(r => r.review_id == reviewId);
                if (!reviewExists)
                {
                    TempData["ErrorMessage"] = "Bài đánh giá không tồn tại!";
                    return RedirectToAction("Index");
                }

                var newComment = new comment
                {
                    review_id = reviewId,
                    patient_id = customerId,
                    comment_text = commentText.Trim(),
                    created_at = DateTime.Now
                };

                db.comments.InsertOnSubmit(newComment);
                db.SubmitChanges();

                TempData["SuccessMessage"] = "Bình luận của bạn đã được đăng!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi đăng bình luận!";
            }

            return RedirectToAction("Detail", new { reviewId = reviewId });
        }
        public ActionResult Detail(int reviewId)
        {
            try
            {
                var customers = db.customers.ToList();
                var comments = db.comments.ToList();
                var review = db.reviews.FirstOrDefault(r => r.review_id == reviewId);

                if (review == null)
                {
                    TempData["ErrorMessage"] = "Bài đánh giá không tồn tại!";
                    return RedirectToAction("Index");
                }

                var reviewDetail = new ReviewViewModel
                {
                    ReviewId = review.review_id,
                    CustomerId = review.patient_id ?? 0,
                    CustomerName = customers.FirstOrDefault(c => c.patient_id == review.patient_id)?.full_name ?? "Ẩn danh",
                    Title = review.title ?? "Không có tiêu đề",  // Thêm dòng này
                    Rating = review.rating,
                    ReviewText = review.comment,
                    ReviewDate = review.created_at ?? DateTime.MinValue,
                    Comments = comments
         .Where(c => c.review_id == review.review_id)
         .OrderBy(c => c.created_at)
         .Select(c => new CommnentViewModel
         {
             CommentId = c.comment_id,
             ReviewId = c.review_id,
             CustomerId = c.patient_id,
             CustomerName = customers.FirstOrDefault(cus => cus.patient_id == c.patient_id)?.full_name ?? "Ẩn danh",
             CommentText = c.comment_text,
             CreatedAt = c.created_at ?? DateTime.MinValue
         })
         .ToList()
                };

                return View(reviewDetail);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi tải chi tiết đánh giá!";
                return RedirectToAction("Index");
            }
        }
    }
}