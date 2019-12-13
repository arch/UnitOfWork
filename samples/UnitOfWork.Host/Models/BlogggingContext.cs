using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Arch.EntityFrameworkCore.UnitOfWork.Host.Models
{
    public class BloggingContext : DbContext
    {
        public BloggingContext(DbContextOptions<BloggingContext> options)
            : base(options)
        { }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.EnableAutoHistory(null);
        }
    }

    public class Blog
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }

        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public List<Comment> Comments { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}