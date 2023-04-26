using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyBikeApp.Models;

namespace MyBikeApp.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
		public DbSet<MyBikeApp.Models.Journey>? Journey { get; set; }
		public DbSet<MyBikeApp.Models.Station>? Station { get; set; }
        public DbSet<MyBikeApp.Models.LoginViewModel>? LoginViewModel { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

		}

		
	}
}