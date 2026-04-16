namespace Hris.Demo.Shared.Dtos;

public sealed class OrganizationUnitUpsertDto
{
    public OrganizationUnitUpsertDto()
    {
    }

    public OrganizationUnitUpsertDto(string code, string name)
    {
        Code = code;
        Name = name;
    }

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public sealed class PositionItemUpsertDto
{
    public PositionItemUpsertDto()
    {
    }

    public PositionItemUpsertDto(string itemNumber, string plantillaNumber, string title, string salaryGrade, Guid organizationUnitId, Guid qualificationStandardRefId)
    {
        ItemNumber = itemNumber;
        PlantillaNumber = plantillaNumber;
        Title = title;
        SalaryGrade = salaryGrade;
        OrganizationUnitId = organizationUnitId;
        QualificationStandardRefId = qualificationStandardRefId;
    }

    public string ItemNumber { get; set; } = string.Empty;
    public string PlantillaNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string SalaryGrade { get; set; } = string.Empty;
    public Guid OrganizationUnitId { get; set; }
    public Guid QualificationStandardRefId { get; set; }
}

public sealed class PersonProfileUpsertDto
{
    public PersonProfileUpsertDto()
    {
    }

    public PersonProfileUpsertDto(string fullName, string? emailAddress)
    {
        FullName = fullName;
        EmailAddress = emailAddress;
    }

    public string FullName { get; set; } = string.Empty;
    public string? EmailAddress { get; set; }
}

public sealed class QualificationStandardRefUpsertDto
{
    public QualificationStandardRefUpsertDto()
    {
    }

    public QualificationStandardRefUpsertDto(string code, string positionTitle, string description, string education, string training, string experience, string eligibility)
    {
        Code = code;
        PositionTitle = positionTitle;
        Description = description;
        Education = education;
        Training = training;
        Experience = experience;
        Eligibility = eligibility;
    }

    public string Code { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public string Training { get; set; } = string.Empty;
    public string Experience { get; set; } = string.Empty;
    public string Eligibility { get; set; } = string.Empty;
}
