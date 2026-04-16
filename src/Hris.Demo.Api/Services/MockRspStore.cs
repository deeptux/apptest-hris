using Hris.Demo.Shared;
using Hris.Demo.Shared.Dtos;

namespace Hris.Demo.Api.Services;

public sealed class MockRspStore
{
    private readonly List<OrganizationUnitDto> _organizationUnits;
    private readonly List<QualificationStandardRefDto> _qualificationStandards;
    private readonly List<PositionItemDto> _positionItems;
    private readonly List<PersonProfileDto> _personProfiles;
    private readonly List<ManpowerRequestDto> _manpowerRequests;
    private readonly List<VacancyDto> _vacancies;
    private readonly List<ApplicantDto> _applicants;
    private readonly List<AppointmentPackageDto> _appointments;
    private readonly List<AuditEventDto> _auditEvents;

    public MockRspStore()
    {
        var ouNursing = Guid.Parse("11111111-1111-4111-8111-111111111111");
        var ouRecords = Guid.Parse("22222222-2222-4222-8222-222222222222");
        var ouHr = Guid.Parse("33333333-3333-4333-8333-333333333333");

        var qsNurse = Guid.Parse("44444444-4444-4444-8444-444444444444");
        var qsRecords = Guid.Parse("55555555-5555-4555-8555-555555555555");
        var qsHr = Guid.Parse("66666666-6666-4666-8666-666666666666");

        var piNurse = Guid.Parse("77777777-7777-4777-8777-777777777777");
        var piRecords = Guid.Parse("88888888-8888-4888-8888-888888888888");
        var piHr = Guid.Parse("99999999-9999-4999-8999-999999999999");

        var profileAna = Guid.Parse("aaaaaaa1-aaaa-4aaa-8aaa-aaaaaaaaaaa1");
        var profileSam = Guid.Parse("aaaaaaa2-aaaa-4aaa-8aaa-aaaaaaaaaaa2");

        _organizationUnits =
        [
            new(ouNursing, "NSD", "Nursing Services", true, null),
            new(ouRecords, "MRD", "Medical Records", true, null),
            new(ouHr, "HRMO", "Human Resource Management Office", true, null)
        ];

        _qualificationStandards =
        [
            new(qsNurse, "QS-SN2", "Staff Nurse II", "Handles bedside care and patient support under nursing protocols.", "BS Nursing", "8 hours nursing updates", "1 year relevant experience", "RA 1080", true, null),
            new(qsRecords, "QS-RO1", "Records Officer I", "Maintains records integrity and document lifecycle controls.", "Bachelor's Degree", "4 hours records management", "None required", "Career Service Professional", true, null),
            new(qsHr, "QS-HRS1", "HR Specialist I", "Supports recruitment, placement, and HR policy administration.", "Bachelor's Degree", "8 hours HR training", "1 year HR experience", "Career Service Professional", true, null)
        ];

        _positionItems =
        [
            new(piNurse, "ITM-NSD-014", "PLT-2026-014", "Staff Nurse II", "SG-15", ouNursing, qsNurse, true, null),
            new(piRecords, "ITM-MRD-018", "PLT-2026-018", "Records Officer I", "SG-12", ouRecords, qsRecords, true, null),
            new(piHr, "ITM-HRMO-021", "PLT-2026-021", "HR Specialist I", "SG-16", ouHr, qsHr, true, null)
        ];

        _personProfiles =
        [
            new(profileAna, "Ana Reyes", "ana.reyes@demo.local", null, false, true, null),
            new(profileSam, "Sam Villarin", "sam.villarin@demo.local", null, false, true, null)
        ];

        _manpowerRequests =
        [
            new(Guid.Parse("a1000000-0000-4000-8000-000000000001"), "MR-2026-014", "Nursing Services", "Staff Nurse II", ouNursing, piNurse, "Approved", new DateOnly(2026, 3, 2)),
            new(Guid.Parse("a1000000-0000-4000-8000-000000000002"), "MR-2026-018", "Medical Records", "Records Officer I", ouRecords, piRecords, "Submitted", new DateOnly(2026, 3, 18)),
            new(Guid.Parse("a1000000-0000-4000-8000-000000000003"), "MR-2026-021", "HRMO", "HR Specialist I", ouHr, piHr, "Draft", new DateOnly(2026, 4, 1))
        ];

        _vacancies =
        [
            new(Guid.Parse("b2000000-0000-4000-8000-000000000001"), "Staff Nurse II", "SG-15", piNurse, new DateOnly(2026, 3, 10), new DateOnly(2026, 4, 10), "Published"),
            new(Guid.Parse("b2000000-0000-4000-8000-000000000002"), "Records Officer I", "SG-12", piRecords, new DateOnly(2026, 3, 20), new DateOnly(2026, 4, 20), "Unpublished"),
            new(Guid.Parse("b2000000-0000-4000-8000-000000000003"), "HR Specialist I", "SG-16", piHr, new DateOnly(2026, 4, 5), new DateOnly(2026, 5, 5), "Unpublished")
        ];

        _applicants =
        [
            new(Guid.Parse("c3000000-0000-4000-8000-000000000001"), "Ana Reyes", "Staff Nurse II", profileAna, "Qualified", 88),
            new(Guid.Parse("c3000000-0000-4000-8000-000000000002"), "Jordan Cruz", "Staff Nurse II", null, "Pending", 72),
            new(Guid.Parse("c3000000-0000-4000-8000-000000000003"), "Sam Villarin", "Records Officer I", profileSam, "Qualified", 81)
        ];

        _appointments =
        [
            new(Guid.Parse("d4000000-0000-4000-8000-000000000001"), "Ana Reyes", Guid.Parse("c3000000-0000-4000-8000-000000000001"), "Staff Nurse II", piNurse, null, "Package assembly", null),
            new(Guid.Parse("d4000000-0000-4000-8000-000000000002"), "Sam Villarin", Guid.Parse("c3000000-0000-4000-8000-000000000003"), "Records Officer I", piRecords, null, "Ready for approval", new DateOnly(2026, 5, 1))
        ];

        _auditEvents =
        [
            new(Guid.Parse("e5000000-0000-4000-8000-000000000001"), DateTimeOffset.Parse("2026-04-02T09:12:00+08:00"), "HR", "Submitted", "ManpowerRequest", Guid.Parse("a1000000-0000-4000-8000-000000000002"), "Manpower request MR-2026-018"),
            new(Guid.Parse("e5000000-0000-4000-8000-000000000002"), DateTimeOffset.Parse("2026-04-02T10:05:00+08:00"), "Approver", "Approved", "ManpowerRequest", Guid.Parse("a1000000-0000-4000-8000-000000000001"), "Manpower request MR-2026-014"),
            new(Guid.Parse("e5000000-0000-4000-8000-000000000003"), DateTimeOffset.Parse("2026-04-03T14:22:00+08:00"), "HR", "Published", "Vacancy", Guid.Parse("b2000000-0000-4000-8000-000000000001"), "Vacancy Staff Nurse II")
        ];
    }

    public IReadOnlyList<RspJourneyStepDto> JourneySteps => RspJourney.Steps;
    public IReadOnlyList<ManpowerRequestDto> ManpowerRequests => _manpowerRequests;
    public IReadOnlyList<VacancyDto> Vacancies => _vacancies;
    public IReadOnlyList<ApplicantDto> Applicants => _applicants;
    public IReadOnlyList<AppointmentPackageDto> Appointments => _appointments;
    public IReadOnlyList<AuditEventDto> AuditEvents => _auditEvents;

    public IReadOnlyList<OrganizationUnitDto> GetOrganizationUnits(bool includeInactive = false) =>
        includeInactive ? _organizationUnits : _organizationUnits.Where(x => x.IsActive).ToList();

    public OrganizationUnitDto CreateOrganizationUnit(OrganizationUnitUpsertDto model)
    {
        var created = new OrganizationUnitDto(Guid.NewGuid(), model.Code.Trim(), model.Name.Trim(), true, null);
        _organizationUnits.Add(created);
        return created;
    }

    public OrganizationUnitDto? UpdateOrganizationUnit(Guid id, OrganizationUnitUpsertDto model)
    {
        var current = _organizationUnits.FirstOrDefault(x => x.Id == id);
        if (current is null)
        {
            return null;
        }

        var updated = current with { Code = model.Code.Trim(), Name = model.Name.Trim() };
        Replace(_organizationUnits, id, updated);
        return updated;
    }

    public OrganizationUnitDto? DeactivateOrganizationUnit(Guid id)
    {
        var current = _organizationUnits.FirstOrDefault(x => x.Id == id);
        if (current is null)
        {
            return null;
        }

        var updated = current with { IsActive = false, DeactivatedAt = DateTimeOffset.UtcNow };
        Replace(_organizationUnits, id, updated);
        return updated;
    }

    public IReadOnlyList<QualificationStandardRefDto> GetQualificationStandards(bool includeInactive = false) =>
        includeInactive ? _qualificationStandards : _qualificationStandards.Where(x => x.IsActive).ToList();

    public QualificationStandardRefDto CreateQualificationStandard(QualificationStandardRefUpsertDto model)
    {
        var created = new QualificationStandardRefDto(
            Guid.NewGuid(),
            model.Code.Trim(),
            model.PositionTitle.Trim(),
            model.Description.Trim(),
            model.Education.Trim(),
            model.Training.Trim(),
            model.Experience.Trim(),
            model.Eligibility.Trim(),
            true,
            null);
        _qualificationStandards.Add(created);
        return created;
    }

    public QualificationStandardRefDto? UpdateQualificationStandard(Guid id, QualificationStandardRefUpsertDto model)
    {
        var current = _qualificationStandards.FirstOrDefault(x => x.Id == id);
        if (current is null)
        {
            return null;
        }

        var updated = current with
        {
            Code = model.Code.Trim(),
            PositionTitle = model.PositionTitle.Trim(),
            Description = model.Description.Trim(),
            Education = model.Education.Trim(),
            Training = model.Training.Trim(),
            Experience = model.Experience.Trim(),
            Eligibility = model.Eligibility.Trim()
        };
        Replace(_qualificationStandards, id, updated);
        return updated;
    }

    public QualificationStandardRefDto? DeactivateQualificationStandard(Guid id)
    {
        var current = _qualificationStandards.FirstOrDefault(x => x.Id == id);
        if (current is null)
        {
            return null;
        }

        var updated = current with { IsActive = false, DeactivatedAt = DateTimeOffset.UtcNow };
        Replace(_qualificationStandards, id, updated);
        return updated;
    }

    public IReadOnlyList<PositionItemDto> GetPositionItems(bool includeInactive = false) =>
        includeInactive ? _positionItems : _positionItems.Where(x => x.IsActive).ToList();

    public PositionItemDto CreatePositionItem(PositionItemUpsertDto model)
    {
        var created = new PositionItemDto(
            Guid.NewGuid(),
            model.ItemNumber.Trim(),
            model.PlantillaNumber.Trim(),
            model.Title.Trim(),
            model.SalaryGrade.Trim(),
            model.OrganizationUnitId,
            model.QualificationStandardRefId,
            true,
            null);
        _positionItems.Add(created);
        return created;
    }

    public PositionItemDto? UpdatePositionItem(Guid id, PositionItemUpsertDto model)
    {
        var current = _positionItems.FirstOrDefault(x => x.Id == id);
        if (current is null)
        {
            return null;
        }

        var updated = current with
        {
            ItemNumber = model.ItemNumber.Trim(),
            PlantillaNumber = model.PlantillaNumber.Trim(),
            Title = model.Title.Trim(),
            SalaryGrade = model.SalaryGrade.Trim(),
            OrganizationUnitId = model.OrganizationUnitId,
            QualificationStandardRefId = model.QualificationStandardRefId
        };
        Replace(_positionItems, id, updated);
        return updated;
    }

    public PositionItemDto? DeactivatePositionItem(Guid id)
    {
        var current = _positionItems.FirstOrDefault(x => x.Id == id);
        if (current is null)
        {
            return null;
        }

        var updated = current with { IsActive = false, DeactivatedAt = DateTimeOffset.UtcNow };
        Replace(_positionItems, id, updated);
        return updated;
    }

    public IReadOnlyList<PersonProfileDto> GetPersonProfiles(bool includeInactive = false) =>
        includeInactive ? _personProfiles : _personProfiles.Where(x => x.IsActive).ToList();

    public PersonProfileDto CreatePersonProfile(PersonProfileUpsertDto model)
    {
        var created = new PersonProfileDto(Guid.NewGuid(), model.FullName.Trim(), model.EmailAddress?.Trim(), null, false, true, null);
        _personProfiles.Add(created);
        return created;
    }

    public PersonProfileDto? UpdatePersonProfile(Guid id, PersonProfileUpsertDto model)
    {
        var current = _personProfiles.FirstOrDefault(x => x.Id == id);
        if (current is null)
        {
            return null;
        }

        var updated = current with { FullName = model.FullName.Trim(), EmailAddress = model.EmailAddress?.Trim() };
        Replace(_personProfiles, id, updated);
        return updated;
    }

    public PersonProfileDto? DeactivatePersonProfile(Guid id)
    {
        var current = _personProfiles.FirstOrDefault(x => x.Id == id);
        if (current is null)
        {
            return null;
        }

        var updated = current with { IsActive = false, DeactivatedAt = DateTimeOffset.UtcNow };
        Replace(_personProfiles, id, updated);
        return updated;
    }

    public ManpowerRequestDto? ToggleManpowerSubmitApprove(Guid id, string actorRole)
    {
        var current = _manpowerRequests.FirstOrDefault(x => x.Id == id);
        if (current is null)
        {
            return null;
        }

        var nextStatus = current.Status switch
        {
            "Draft" => "Submitted",
            "Submitted" => "Approved",
            _ => "Submitted"
        };
        var updated = current with { Status = nextStatus };
        Replace(_manpowerRequests, id, updated);
        AddAudit(actorRole, nextStatus, "ManpowerRequest", id, $"{updated.ReferenceCode} moved to {nextStatus}");
        return updated;
    }

    public VacancyDto? ToggleVacancyPublish(Guid id, string actorRole)
    {
        var current = _vacancies.FirstOrDefault(x => x.Id == id);
        if (current is null)
        {
            return null;
        }

        var publish = !string.Equals(current.PublicationStatus, "Published", StringComparison.OrdinalIgnoreCase);
        var nextStatus = publish ? "Published" : "Unpublished";
        var updated = current with { PublicationStatus = nextStatus };
        Replace(_vacancies, id, updated);
        AddAudit(actorRole, publish ? "Published" : "Unpublished", "Vacancy", id, $"{updated.Title} is now {nextStatus}");
        return updated;
    }

    public ApplicantDto? SetApplicantOutcome(Guid id, string actorRole, string outcome)
    {
        var current = _applicants.FirstOrDefault(x => x.Id == id);
        if (current is null)
        {
            return null;
        }

        var normalized = outcome.Trim();
        var updated = current with { EligibilityFlag = normalized };
        Replace(_applicants, id, updated);
        AddAudit(actorRole, "ScreeningOutcomeSet", "Applicant", id, $"{updated.FullName} marked {normalized}");
        return updated;
    }

    public AppointmentPackageDto? MarkAsAppointed(Guid id, string actorRole, DateOnly? effectivity)
    {
        var current = _appointments.FirstOrDefault(x => x.Id == id);
        if (current is null)
        {
            return null;
        }

        var appointeeProfileId = current.AppointeePersonProfileId;
        var profile = _personProfiles.FirstOrDefault(x => x.Id == appointeeProfileId)
            ?? _personProfiles.FirstOrDefault(x => string.Equals(x.FullName, current.ApplicantName, StringComparison.OrdinalIgnoreCase));

        if (profile is null)
        {
            profile = new PersonProfileDto(Guid.NewGuid(), current.ApplicantName, null, null, false, true, null);
            _personProfiles.Add(profile);
        }

        var employeeNo = profile.EmployeeNumber;
        if (string.IsNullOrWhiteSpace(employeeNo))
        {
            employeeNo = $"EMP-{DateTime.UtcNow:yyyy}-{(_personProfiles.Count + 100):0000}";
        }

        var employeeProfile = profile with { IsEmployee = true, EmployeeNumber = employeeNo };
        Replace(_personProfiles, employeeProfile.Id, employeeProfile);

        var updated = current with
        {
            AppointeePersonProfileId = employeeProfile.Id,
            PackageStatus = "Appointed",
            Effectivity = effectivity ?? DateOnly.FromDateTime(DateTime.UtcNow)
        };
        Replace(_appointments, id, updated);
        AddAudit(actorRole, "MarkAsAppointed", "AppointmentPackage", id, $"{updated.ApplicantName} appointed ({employeeNo})");
        return updated;
    }

    public void AddAudit(string actorRole, string action, string entityType, Guid entityId, string summary)
    {
        _auditEvents.Add(new AuditEventDto(Guid.NewGuid(), DateTimeOffset.UtcNow, actorRole, action, entityType, entityId, summary));
    }

    private static void Replace<T>(List<T> items, Guid id, T updated) where T : class
    {
        var index = items.FindIndex(x => ((dynamic)x).Id == id);
        if (index >= 0)
        {
            items[index] = updated;
        }
    }
}
