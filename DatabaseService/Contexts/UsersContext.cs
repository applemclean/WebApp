using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using DatabaseService.Models;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace DatabaseService.Contexts
{
    /// <summary>
    /// Context for the purpose of operating on users records
    /// </summary>
    internal class UsersContext : DbContext
    {
        /// <summary>
        /// Set of all available users.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Create context add pass a connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public UsersContext(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// Create <c>UsersContext</c>
        /// Database connection via web.config
        /// </summary>
        public UsersContext() : base("DefaultConnection")
        {
        }

        /// <summary>
        /// Overrides User validation.
        /// </summary>
        /// <param name="entityEntry">User entity</param>
        /// <param name="items">Items</param>
        /// <returns>User validation result</returns>
        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            var result = base.ValidateEntity(entityEntry, items);
            var userEntry = entityEntry.Entity as User;
                
            // Check email uniqueness
            if(Users.Where(x => x.EMail == userEntry.EMail && x.Id != userEntry.Id).FirstOrDefault() != default(User))
            {
                var eMailValidationError = new DbValidationError(nameof(User.EMail), "The EMail is not unique");
                result.ValidationErrors.Add(eMailValidationError);
                return new DbEntityValidationResult(entityEntry, result.ValidationErrors);
            }
            else
            {
                return result;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var userEntity = modelBuilder.Entity<User>();
            userEntity.HasKey(x => x.Id);
            var emailUniqueIndex = new IndexAnnotation(
                new IndexAttribute("IX_Email") { IsUnique = true }
            );

            userEntity.Property(x => x.EMail).IsRequired()
                      .HasMaxLength(300)
                      .HasColumnAnnotation(IndexAnnotation.AnnotationName, emailUniqueIndex);
            userEntity.Property(x => x.Name).IsRequired();
            userEntity.Property(x => x.EMail).IsRequired();
            userEntity.Property(x => x.Surname).IsRequired();
        }
    }
}