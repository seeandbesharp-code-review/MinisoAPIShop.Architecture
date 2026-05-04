using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record UserLoginDTO
    (
        [Required, EmailAddress]
        string Email,

        [Required]
        string Password

    );


    public record UserRegisterDTO
    (
        [Required, EmailAddress]
        string Email,

        [Required]
        string FirstName,

        [Required]
        string LastName,

        [Required]
        string Password,

        string? Phone,

        string? City,

        string? Street

    );

    public record UserUpdateDTO
    (
        [Required]
        string FirstName,

        [Required]
        string LastName,

        string? Phone,

        string? City,

        string? Street
    );

    public record UserReadDTO
    (
         int UserId,

         [Required, EmailAddress] 
        string Email,

         [Required]
        string  Role,

        [Required]
         string FirstName,

        [Required]
         string LastName,

        string? Phone,

        string? City,

        string? Street
    );


}
