using Microsoft.EntityFrameworkCore;
using MyWebApi.Models;

namespace MyWebApi.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbcontextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users {get; set;}
  }
}