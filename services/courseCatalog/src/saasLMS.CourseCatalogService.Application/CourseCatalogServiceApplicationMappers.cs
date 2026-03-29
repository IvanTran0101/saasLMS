using Riok.Mapperly.Abstractions;
using saasLMS.CourseCatalogService.Chapters.Dtos.Outputs;
using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Courses.Dtos.Outputs;
using saasLMS.CourseCatalogService.Lessons.Dtos.Outputs;
using saasLMS.CourseCatalogService.Materials.Dtos.Outputs;
using Volo.Abp.Mapperly;

namespace saasLMS.CourseCatalogService;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CourseToCourseDtoMapper : MapperBase<Course, CourseDto>
{
    public override partial CourseDto Map(Course source);
    public override partial void Map(Course source, CourseDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CourseToCourseDetailDtoMapper : MapperBase<Course, CourseDetailDto>
{
    // Codex: CourseDetailDto.CourseId comes from Course.Id, Chapters are handled elsewhere.
    [MapperIgnoreTarget(nameof(CourseDetailDto.Chapters))]
    [MapProperty(nameof(Course.Id), nameof(CourseDetailDto.CourseId))]
    public override partial CourseDetailDto Map(Course source);
    // Codex: same mapping rules for Map(source, destination).
    [MapperIgnoreTarget(nameof(CourseDetailDto.Chapters))]
    [MapProperty(nameof(Course.Id), nameof(CourseDetailDto.CourseId))]
    public override partial void Map(Course source, CourseDetailDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CourseToCourseListItemDtoMapper : MapperBase<Course, CourseListItemDto>
{
    // Codex: CourseListItemDto.CourseId comes from Course.Id.
    [MapProperty(nameof(Course.Id), nameof(CourseListItemDto.CourseId))]
    public override partial CourseListItemDto Map(Course source);
    // Codex: same mapping rules for Map(source, destination).
    [MapProperty(nameof(Course.Id), nameof(CourseListItemDto.CourseId))]
    public override partial void Map(Course source, CourseListItemDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ChapterToChapterDtoMapper : MapperBase<Chapter, ChapterDto>
{
    // Codex: ChapterDto.Lessons is mapped via LessonInChapterDto elsewhere (ignore here).
    [MapperIgnoreTarget(nameof(ChapterDto.Lessons))]
    public override partial ChapterDto Map(Chapter source);
    // Codex: same mapping rules for Map(source, destination).
    [MapperIgnoreTarget(nameof(ChapterDto.Lessons))]
    public override partial void Map(Chapter source, ChapterDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class LessonToLessonDtoMapper : MapperBase<Lesson, LessonDto>
{
    // Codex: LessonDto.CourseId/ChapterId/Materials are enriched in AppService.
    [MapperIgnoreTarget(nameof(LessonDto.CourseId))]
    [MapperIgnoreTarget(nameof(LessonDto.ChapterId))]
    [MapperIgnoreTarget(nameof(LessonDto.Materials))]
    public override partial LessonDto Map(Lesson source);
    // Codex: same mapping rules for Map(source, destination).
    [MapperIgnoreTarget(nameof(LessonDto.CourseId))]
    [MapperIgnoreTarget(nameof(LessonDto.ChapterId))]
    [MapperIgnoreTarget(nameof(LessonDto.Materials))]
    public override partial void Map(Lesson source, LessonDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class MaterialToMaterialDtoMapper : MapperBase<Material, MaterialDto>
{
    // Codex: MaterialDto.CourseId/ChapterId are enriched in AppService.
    [MapperIgnoreTarget(nameof(MaterialDto.CourseId))]
    [MapperIgnoreTarget(nameof(MaterialDto.ChapterId))]
    [MapProperty(nameof(Material.TextContent), nameof(MaterialDto.Content))]
    [MapProperty(nameof(Material.TextFormat), nameof(MaterialDto.Format))]
    public override partial MaterialDto Map(Material source);
    // Codex: same mapping rules for Map(source, destination).
    [MapperIgnoreTarget(nameof(MaterialDto.CourseId))]
    [MapperIgnoreTarget(nameof(MaterialDto.ChapterId))]
    [MapProperty(nameof(Material.TextContent), nameof(MaterialDto.Content))]
    [MapProperty(nameof(Material.TextFormat), nameof(MaterialDto.Format))]
    public override partial void Map(Material source, MaterialDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class LessonToLessonInChapterDtoMapper : MapperBase<Lesson, LessonInChapterDto>
{
    // Codex: LessonInChapterDto.Materials are mapped via MaterialInLessonDto elsewhere (ignore here).
    [MapperIgnoreTarget(nameof(LessonInChapterDto.Materials))]
    public override partial LessonInChapterDto Map(Lesson source);
    // Codex: same mapping rules for Map(source, destination).
    [MapperIgnoreTarget(nameof(LessonInChapterDto.Materials))]
    public override partial void Map(Lesson source, LessonInChapterDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class MaterialToMaterialInLessonDtoMapper : MapperBase<Material, MaterialInLessonDto>
{
    // Codex: MaterialInLessonDto.LessonId is provided by parent context, not Material entity.
    [MapperIgnoreTarget(nameof(MaterialInLessonDto.LessonId))]
    [MapProperty(nameof(Material.TextContent), nameof(MaterialInLessonDto.Content))]
    [MapProperty(nameof(Material.TextFormat), nameof(MaterialInLessonDto.Format))]
    public override partial MaterialInLessonDto Map(Material source);
    // Codex: same mapping rules for Map(source, destination).
    [MapperIgnoreTarget(nameof(MaterialInLessonDto.LessonId))]
    [MapProperty(nameof(Material.TextContent), nameof(MaterialInLessonDto.Content))]
    [MapProperty(nameof(Material.TextFormat), nameof(MaterialInLessonDto.Format))]
    public override partial void Map(Material source, MaterialInLessonDto destination);
}
