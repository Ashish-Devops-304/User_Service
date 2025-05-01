using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository;
using System.Security.Cryptography;

namespace UserServiceTests
{
    public class TestHelper
    {
        /// <summary>
        /// Hash a password using PBKDF2 with SHA-256
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string HashPassword(string password)
        {
            // Generate a random salt

            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Compute the hash of the password and salt using PBKDF2 with SHA-256

            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                hash = pbkdf2.GetBytes(32);
            }

            // Concatenate the salt and hash and convert to a base64 string

            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Seeds the in memory database with records
        /// </summary>
        /// <returns></returns>
        public static RepositoryContext GetContextWithRecords()
        {
            var option = new DbContextOptionsBuilder<RepositoryContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            var context = new RepositoryContext(option);

            if (context != null)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            // User record 1

            context.Add(new User()
            {
                Id = Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"),
                FirstName = "Hariharan",
                LastName = "A S",
                Email = "iamhari@gmail.com",
                Phone = 9988774455,
                Role = "Admin"
            });

            context.Add(new UserSecret()
            {
                Id = Guid.Parse("66288e6c-bd36-4789-8b2e-b229de707203"),
                UserId = Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"),
                Password = HashPassword("passwordPassword@12#")
            });

            context.Add(new Address()
            {
                Id = Guid.Parse("5c9f4c63-f510-4ae1-95c1-4e5a43c42c07"),
                UserId = Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"),
                Name = "Hariharan",
                Type = "Home",
                Phone = 8877445566,
                Line1 = "4/576B",
                Line2 = "BMC Nagar",
                City = "Coimbatore",
                Pincode = 642126,
                State = "Tamil Nadu",
                Country = "India"
            });

            context.Add(new Payment()
            {
                Id = Guid.Parse("da2811ed-60a6-4984-bf61-083864cbdbf5"),
                UserId = Guid.Parse("95904722-c943-4716-aef0-feeea5ac8cce"),
                Type = "UPI",
                Name = "PayTM UPI",
                PaymentValue = "mypaytmupi@paytm"
            });

            // User record 2

            context.Add(new User()
            {
                Id = Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"),
                FirstName = "Sudharsan",
                LastName = "S",
                Email = "sudharsans@gmail.com",
                Phone = 6324789563,
                Role = "Customer"
            });

            context.Add(new UserSecret()
            {
                Id = Guid.Parse("e82810db-7e36-40a6-85cd-1db4b2854d60"),
                UserId = Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"),
                Password = HashPassword("MkleojnoMlpo@12#")
            });

            context.Add(new Address()
            {
                Id = Guid.Parse("4016bb62-3c83-4c76-bae3-ed8f414a8d64"),
                UserId = Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"),
                Name = "Sudharsan",
                Type = "Home",
                Phone = 9666993344,
                Line1 = "9, Green Park Layout",
                Line2 = "2nd street",
                City = "Chennai",
                Pincode = 600116,
                State = "Tamil Nadu",
                Country = "India"
            });

            context.Add(new Payment()
            {
                Id = Guid.Parse("a3d91c91-8afd-4e38-a881-8576547158cf"),
                UserId = Guid.Parse("8c5ac0ba-46eb-4e7d-8560-83dc932cb414"),
                Type = "UPI",
                Name = "GPay",
                PaymentValue = "mygpayupi@paytm"
            });

            // User record 3

            context.Add(new User()
            {
                Id = Guid.Parse("d3b0cbb1-b299-4314-8b75-b6a9f5d7f014"),
                FirstName = "Surya",
                LastName = "Kumar",
                Email = "suryakumar@gmail.com",
                Phone = 9874521036,
                Role = "Customer"
            });

            context.Add(new UserSecret()
            {
                Id = Guid.Parse("883ed5cb-d633-436f-8968-6a16c40e59f3"),
                UserId = Guid.Parse("d3b0cbb1-b299-4314-8b75-b6a9f5d7f014"),
                Password = HashPassword("MkleojnoMlpo@12#")
            });

            context.SaveChanges();

            return context;
        }

        /// <summary>
        /// Seeds the in memory database with no records
        /// </summary>
        /// <returns></returns>
        public static RepositoryContext GetEmptyContext()
        {
            var option = new DbContextOptionsBuilder<RepositoryContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            var context = new RepositoryContext(option);

            if (context != null)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            return context;
        }

    }
}
