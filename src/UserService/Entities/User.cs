using System.ComponentModel.DataAnnotations;
using Loft.Common.Enums;

namespace UserService.Entities;
public class User
{
    public long Id { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MinLength(6)]
    public string PasswordHash { get; set;} = string.Empty;
    public Role Role { get; set; }
    public bool CanSell { get; set; } = false;
    public string? AvatarUrl { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
}
