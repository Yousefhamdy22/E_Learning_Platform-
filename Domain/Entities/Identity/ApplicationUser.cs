using Domain.Entities.Students;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [MaxLength(100)]
        public string? FirstName { get; set; }
        [MaxLength(100)]
        public string? LastName { get; set; }
        public DateTime? CreatedDate { get; set; }

        // Navigation properties

        public Instructor? Instructor { get; set; }
        public Student? Student { get; set; }
        public ICollection<RefreshToken>? RefreshTokens { get; set; }



    }
}
