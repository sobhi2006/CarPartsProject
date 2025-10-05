using CarPartsProject.DTOs.Requests.CarParts;
using CarPartsProject.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace CarPartsProject.Validations.CarParts;

public class CarPartsValidator : AbstractValidator<CarPartRequest>
{
    public CarPartsValidator()
    {       
        RuleFor(p => p.Name)
                .NotEmpty()
                .WithMessage("Name can't be empty.")
                .Length(3, 100)
                .WithMessage("Length of Name can't be less 3 character and over 100 character.");

        RuleFor(p => p.Brand)
                .NotEmpty()
                .WithMessage("Brand can't be empty.")
                .Length(3, 50)
                .WithMessage("Length of Brand can't be less 3 character and over 50 character.");

        RuleFor(p => p.CountryPart)
                .NotNull()
                .WithMessage("CountryPart is required.")
                .NotEmpty()
                .WithMessage("CountryPart can't be empty.")
                .Length(3, 50)
                .WithMessage("Length of CountryPart can't be less 3 character and over 50 character.");

        RuleFor(p => p.Description)
                .NotNull()
                .WithMessage("Description is required.")
                .NotEmpty()
                .WithMessage("Description can't be empty.")
                .Length(3, 1000)
                .WithMessage("Length of Description can't be less 3 character and over 1000 character.");

        RuleFor(p => p.StatusPart)
                .NotNull()
                .WithMessage("StatusPart is required.")
                .IsInEnum()
                .WithMessage($"StatusPart must be in {CarPartStatus.New},or {CarPartStatus.Used}");

        RuleFor(p => p.ImageFile)
                .Must(HaveValidImageCount)
                .WithMessage("can you upload 5 images maximum");

        RuleForEach(p => p.ImageFile)
                .NotNull()
                .WithMessage("ImageFile is required.")
                .Must(BeValidImage)
                .WithMessage("you upload invalid image, you can upload image with extension {.jpg, .jpeg, .png, .gif, .bmp}")
                .Must(BeUnderMaxSize)
                .WithMessage("Max size for each image is 5 MB");
        }
    private bool HaveValidImageCount(List<IFormFile> files)
    {
        return files is not null && files.Count <= 5;
    }

    private bool BeValidImage(IFormFile file)
    {
        if (file is null) return false;
        var AllowExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        var extension = Path.GetExtension(file.FileName);
        return AllowExtensions.Contains(extension);
    }

    private bool BeUnderMaxSize(IFormFile file)
    {
        return file is not null && file.Length <= 5 * 1024 * 1024;
    }
}