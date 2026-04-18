using System;
using Microsoft.AspNetCore.Components;
using saasLMS.CourseCatalogService.Materials.Dtos.Outputs;

namespace saasLMS.Blazor.Client.Pages.Student.Learn.Components;

public partial class VideoViewer : ComponentBase
{
    // ── Parameter ─────────────────────────────────────────────────────────────

    /// <summary>
    /// The VideoLink material. Its <see cref="MaterialInLessonDto.ExternalUrl"/> is
    /// the YouTube/video URL that the Instructor saved via CreateVideoLinkMaterialAsync.
    /// </summary>
    [Parameter, EditorRequired]
    public MaterialInLessonDto? Material { get; set; }

    // ── Derived state ─────────────────────────────────────────────────────────

    /// Final embed URL shown in the template (YouTube embed or direct URL).
    private string? _embedUrl;

    /// True when the URL should be rendered as an iframe (YouTube / embeddable).
    private bool _isIframe;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    protected override void OnParametersSet()
    {
        var raw = Material?.ExternalUrl;

        if (string.IsNullOrWhiteSpace(raw))
        {
            _embedUrl = null;
            _isIframe = false;
            return;
        }

        if (IsYouTubeUrl(raw))
        {
            _embedUrl = BuildYouTubeEmbedUrl(raw);
            _isIframe = true;
        }
        else
        {
            // Generic direct video URL — let the browser's <video> element handle it
            _embedUrl = raw;
            _isIframe = false;
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static bool IsYouTubeUrl(string url)
        => url.Contains("youtube.com", StringComparison.OrdinalIgnoreCase)
        || url.Contains("youtu.be",    StringComparison.OrdinalIgnoreCase);

    /// Converts any YouTube watch/share URL to the embeddable https://www.youtube.com/embed/{id} form.
    private static string BuildYouTubeEmbedUrl(string url)
    {
        // Short share link: https://youtu.be/VIDEO_ID
        if (url.Contains("youtu.be/", StringComparison.OrdinalIgnoreCase))
        {
            var afterSlash = url.Substring(url.LastIndexOf("youtu.be/", StringComparison.OrdinalIgnoreCase) + 9);
            var id = afterSlash.Split('?', '&', '#')[0];
            return $"https://www.youtube.com/embed/{id}";
        }

        // Watch URL: https://www.youtube.com/watch?v=VIDEO_ID[&...]
        var qIdx = url.IndexOf('?');
        if (qIdx >= 0)
        {
            foreach (var pair in url.Substring(qIdx + 1).Split('&'))
            {
                var eqIdx = pair.IndexOf('=');
                if (eqIdx > 0 && pair.Substring(0, eqIdx) == "v")
                    return $"https://www.youtube.com/embed/{pair.Substring(eqIdx + 1)}";
            }
        }

        // Embed URL already (https://www.youtube.com/embed/...)
        return url;
    }
}
