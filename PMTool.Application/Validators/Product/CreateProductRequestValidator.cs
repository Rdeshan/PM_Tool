using FluentValidation;
using PMTool.Application.DTOs.Product;

namespace PMTool.Application.Validators.Product;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required");

        RuleFor(x => x.VersionName)
            .NotEmpty().WithMessage("Version name is required")
            .Matches(@"^\d+\.\d+(\.\d+)?$").WithMessage("Version name must follow semantic versioning (e.g., 1.0, 1.0.0)")
            .MaximumLength(50).WithMessage("Version name cannot exceed 50 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.PlannedReleaseDate)
            .NotEmpty().WithMessage("Planned release date is required")
            .GreaterThan(DateTime.UtcNow).WithMessage("Planned release date must be in the future");

        RuleFor(x => x.ReleaseType)
            .InclusiveBetween(1, 4).WithMessage("Invalid release type");
    }
}
