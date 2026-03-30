using Riok.Mapperly.Abstractions;
using saasLMS.EnrollmentService.Enrollments;
using saasLMS.EnrollmentService.Enrollments.Dtos.Outputs;
using Volo.Abp.Mapperly;

namespace saasLMS.EnrollmentService;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class EnrollmentToEnrollmentDtoMapper : MapperBase<Enrollment, EnrollmentDto>
{
    public override partial EnrollmentDto Map(Enrollment source);
    public override partial void Map(Enrollment source, EnrollmentDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class EnrollmentToEnrollmentListItemDtoMapper : MapperBase<Enrollment, EnrollmentListItemDto>
{
    public override partial EnrollmentListItemDto Map(Enrollment source);
    public override partial void Map(Enrollment source, EnrollmentListItemDto destination);
}