﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLayer.DTOs.Comments;
using CoreLayer.Utilities;
using DataLayer.Context;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreLayer.Services.Comments
{
    public interface ICommentService
    {
        OperationResult CreateComment(CreateCommentDto command);
        List<CommentDto> GetPostComments(int postId);
    }
    public class CommentService : ICommentService
    {
        private readonly BlogContext _context;

        public CommentService(BlogContext context)
        {
            _context = context;
        }

        public OperationResult CreateComment(CreateCommentDto command)
        {
            var comment = new PostComment()
            {
                PostId = command.PostId,
                Text = command.Text,
                UserId = command.UserId
            };
            _context.Add(comment);
            _context.SaveChanges();
            return OperationResult.Success();
        }

        public List<CommentDto> GetPostComments(int postId)
        {
            return _context.PostComments
                .Include(c => c.User)
                .Where(c => c.PostId == postId)
                .Select(comment => new CommentDto()
                {
                    PostId = comment.PostId,
                    Text = comment.Text,
                    UserFullName = comment.User.FullName,
                    CommentId = comment.Id,
                    CreationDate = comment.CreationDate
                }).ToList();
        }
    }
}