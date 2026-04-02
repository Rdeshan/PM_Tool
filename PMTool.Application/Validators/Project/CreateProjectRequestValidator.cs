using FluentValidation;
using PMTool.Application.DTOs.Project;

namespace PMTool.Application.Validators.Project;

public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required")
            .MaximumLength(200).WithMessage("Project name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.ClientName)
            .NotEmpty().WithMessage("Client name is required")
            .MaximumLength(200).WithMessage("Client name cannot exceed 200 characters");

        RuleFor(x => x.ProjectCode)
            .NotEmpty().WithMessage("Project code is required")
            .Matches(@"^[A-Z0-9]{3,20}$").WithMessage("Project code must be 3-20 uppercase letters/numbers");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .LessThanOrEqualTo(x => x.ExpectedEndDate).WithMessage("Start date must be before end date");

        RuleFor(x => x.ExpectedEndDate)
            .NotEmpty().WithMessage("Expected end date is required")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date");

        RuleFor(x => x.ColourCode)
            .Matches(@"^#[0-9A-Fa-f]{6}$").When(x => !string.IsNullOrEmpty(x.ColourCode))
            .WithMessage("Colour code must be a valid hex colour (e.g., #FF5733)");
    }
}
