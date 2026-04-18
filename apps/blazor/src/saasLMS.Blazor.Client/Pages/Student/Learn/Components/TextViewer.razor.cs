using Microsoft.AspNetCore.Components;
using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Materials.Dtos.Outputs;

namespace saasLMS.Blazor.Client.Pages.Student.Learn.Components;

public partial class TextViewer : ComponentBase
{
    // ── Parameter ─────────────────────────────────────────────────────────────

    /// <summary>
    /// The Text material. Content is read from <see cref="MaterialInLessonDto.Content"/>
    /// and rendered according to <see cref="MaterialInLessonDto.Format"/>.
    /// </summary>
    [Parameter, EditorRequired]
    public MaterialInLessonDto? Material { get; set; }

    // ── Derived state ─────────────────────────────────────────────────────────

    /// The content string to display. For HTML this is raw markup; for plain/markdown it is plain text.
    private string? _renderedContent;

    /// True when the content should be injected as raw HTML via <c>MarkupString</c>.
    private bool _isHtml;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    protected override void OnParametersSet()
    {
        var content = Material?.Content;
        var format  = Material?.Format ?? TextFormat.Plain;

        if (string.IsNullOrWhiteSpace(content))
        {
            _renderedContent = null;
            _isHtml          = false;
            return;
        }

        if (format == TextFormat.Html)
        {
            _renderedContent = content;
            _isHtml          = true;
        }
        else
        {
            // Plain or Markdown: render as pre-formatted text.
            // (Full Markdown rendering would require a third-party library.)
            _renderedContent = content;
            _isHtml          = false;
        }
    }
}
