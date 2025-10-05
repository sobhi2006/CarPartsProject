using CarPartsProject.DTOs.Requests.Users;
using CarPartsProject.Enums;
using FluentValidation;

namespace CarPartsProject.Validations.Users;
public class UserValidator : AbstractValidator<UserRequest>
{
    public UserValidator()
    {
        RuleFor(p => p.FName)
                .NotEmpty()
                .WithMessage("FName can't be empty.")
                .Length(3, 20)
                .WithMessage("Length of FName can't be less 3 character and over 20 character.");

        RuleFor(p => p.LName)
                .NotEmpty()
                .WithMessage("LName can't be empty.")
                .Length(3, 20)
                .WithMessage("Length of LName can't be less 3 character and over 20 character.");

        RuleFor(p => p.Email)
                .NotNull()
                .WithMessage("Email is required.")
                .NotEmpty()
                .WithMessage("Email can't be empty.")
                .EmailAddress()
                .WithMessage("Invalid Email set, try again...");

        RuleFor(p => p.Password)
                .NotNull()
                .WithMessage("Password is required.")
                .NotEmpty()
                .WithMessage("Password can't be empty.")
                .MinimumLength(8)
                .WithMessage("Password can't be less than 8 character.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at less one capital letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at less one small letter")
                .Matches(@"[0-9]").WithMessage("Password must contain at less one digit")
                .Matches(@"[!@#$%^&*()_+-=[\]{}<>?;':""]").WithMessage("Password must contain at less one punctuation");

        RuleFor(p => p.IsActive)
                .NotNull()
                .WithMessage("Is Active is required.")
                .Must(v => v || !v)
                .WithMessage("IsActive must be boolean.");

        RuleFor(p => p.Role)
                .NotNull()
                .WithMessage("Role is required.")
                .IsInEnum()
                .WithMessage($"Role must be in {UserRole.Admin},or {UserRole.Labor}");

        RuleFor(p => p.DateOfBirth)
                .NotNull()
                .WithMessage("DateOfBirth is required.")
                .Must(v =>DateTime.UtcNow.AddYears(-100) < v && DateTime.UtcNow.AddYears(-18) > v)
                .WithMessage("Age must be between (18 - 100).");
    }
    
}