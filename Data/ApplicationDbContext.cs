﻿using Microsoft.EntityFrameworkCore;

namespace Chat
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Message> Messages { get; set; }
    }
}
