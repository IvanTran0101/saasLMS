# Design System: SaaS E-Learning LMS (Tech & Modern Concept)
*Inspired by the clean, trustworthy, and tech-forward architecture of Kraken.*

## 1. Visual Theme & Atmosphere

The design system for this E-learning SaaS LMS is a **sharp, tech-forward, and highly focused learning environment**. It uses stark white backgrounds combined with a commanding **Knowledge Purple** (`#7132f5`) to create a professional, "deep-work" identity. This style is perfect for technical subjects, coding bootcamps, and professional certifications.

Instead of overly playful rounded shapes, this system uses structured `12px` border radii, giving course cards and buttons a precise, architectural feel. The typography relies on modern, geometric sans-serifs (like `IBM Plex Sans` or `Inter`) to ensure maximum legibility for long-form reading and code snippets. Shadows are kept to an absolute whisper (`0px 4px 24px` at `3%` opacity), making the UI feel incredibly flat, modern, and fast.

**Key Characteristics:**
- **Knowledge Purple** (`#7132f5`) as the primary brand and CTA anchor (themeable per tenant).
- **Tech-Geometric Typography:** `Plus Jakarta Sans` or `IBM Plex Sans` for display headings (tight tracking), and `Inter` for UI and lesson text.
- **Cool Neutral Scale:** Near-black (`#101114`) text with cool blue-gray borders, preventing the harshness of pure black-on-white.
- **Architectural Radii:** 12px radius for buttons and course cards (strictly rounded rectangles, never full-pill).
- **Whisper Shadows:** Flat UI design with barely-there shadows (`rgba(0,0,0,0.03)`).
- **Semantic Green:** (`#149e61`) exclusively for positive learning outcomes (Passed quizzes, 100% completion).

## 2. Color Palette & Roles

### Primary (Tenant Themeable)
- **Knowledge Purple** (`#7132f5`): Primary CTA ("Enroll Now"), active lesson states, active tab underlines, and brand logos.
- **Purple Dark** (`#5741d8`): Hover states for primary buttons, outlined button borders.
- **Purple Subtle** (`rgba(133,91,251,0.16)`): Background washes for selected lessons in the curriculum list, or "Pro" tier badges.

### Neutral & Canvas
- **White Canvas** (`#ffffff`): The universal background for dashboards, course detail pages, and reading views.
- **Focus Black** (`#101114`): Primary text for course titles, headings, and lesson body text.
- **Silver Blue** (`#9497a9`): Secondary text, video duration metadata, last-updated timestamps.
- **Border Gray** (`#dedee5`): UI dividers, curriculum chapter borders, empty progress bar tracks.
- **Cool Gray** (`#686b82`): Muted UI elements, inactive icons.

### Semantic (Learning Feedback)
- **Success Green** (`#149e61`): Passed quizzes, correct answers, 100% completion progress bars.
- **Success Light** (`rgba(20,158,97,0.16)`): Background for "Completed" badges.
- **Warning/Pending** (`#fbbc05`): In-progress assignments or warnings.

## 3. Typography Rules

### Font Families
- **Display / Headings**: `Plus Jakarta Sans, IBM Plex Sans, Helvetica, sans-serif` — Geometrical, sharp, tech-oriented.
- **UI / Body / Reading**: `Inter, Helvetica Neue, Arial, sans-serif` — Highly legible for long paragraphs.
- **Code Snippets**: `Fira Code, JetBrains Mono, monospace` — For technical courses.

### Hierarchy

| Role | Font | Size | Weight | Line Height | Letter Spacing | Context |
|------|------|------|--------|-------------|----------------|---------|
| Display Hero | Display | 48px | 700 | 1.17 | -1px | Course Catalog / Landing Page |
| H1 (Course Title) | Display | 36px | 700 | 1.22 | -0.5px | Course Detail Page Header |
| H2 (Chapter) | Display | 28px | 700 | 1.29 | -0.5px | Curriculum section titles |
| Feature Title | UI (Inter) | 22px | 600 | 1.20 | normal | Module titles, Dashboard widgets |
| Body Large | UI (Inter) | 18px | 400 | 1.60 | normal | Main reading text in lessons |
| UI Medium | UI (Inter) | 16px | 500 | 1.38 | normal | Buttons, Navigation tabs |
| Metadata | UI (Inter) | 14px | 400–500 | 1.43 | normal | "2 hours left", "Instructor name" |
| Micro | UI (Inter) | 12px | 500 | 1.33 | normal | Quiz score labels, tooltips |

## 4. Component Stylings

### Buttons

**Primary Knowledge Button ("Enroll", "Start Lesson")**
- Background: `#7132f5`
- Text: `#ffffff`
- Padding: 13px 20px
- Radius: `12px` (Strictly 12px, never 50px pill)
- Font: UI 16px Medium (500)

**Outlined Secondary ("View Syllabus", "Resources")**
- Background: `#ffffff`
- Text: `#5741d8`
- Border: `1px solid #5741d8`
- Radius: `12px`

**Subtle Action ("Mark as Complete")**
- Background: `rgba(133,91,251,0.16)`
- Text: `#7132f5`
- Radius: `12px`

**Secondary Neutral ("Cancel", "Back")**
- Background: `rgba(148,151,169,0.08)`
- Text: `#101114`
- Radius: `12px`

### Course Cards & Containers

**Course Dashboard Card**
- Background: `#ffffff`
- Border: `1px solid #dedee5` (Often preferred over shadows for tech platforms)
- Radius: `12px`
- Shadow: `rgba(0,0,0,0.03) 0px 4px 24px` (Only on hover, otherwise flat)
- Structure:
    - 16:9 Image at top (top radii `12px`)
    - Content padding: `16px`
    - Title in Focus Black, 16px/600
    - Progress Bar: `4px` height, `#dedee5` track, `#7132f5` fill (or `#149e61` if 100%).

### Badges & Feedback

- **Completed/Passed**: `rgba(20,158,97,0.16)` bg, `#026b3f` text, `6px` radius.
- **Tenant/Category Tag**: `rgba(104,107,130,0.12)` bg, `#484b5e` text, `8px` radius.
- **Pro/Premium Tier**: `Purple Subtle` bg, `#7132f5` text.

## 5. Layout & Grid Principles

### Spacing Scale
The system relies on a precise, mathematical spacing scale to maintain structure:
`4px, 8px, 12px, 16px, 20px, 24px, 32px, 48px, 64px`

- Lesson content paragraphs gap: `24px`
- Course card internal padding: `16px` or `24px`
- Distance between curriculum chapters: `32px`

### Border Radius Hierarchy
- Checkboxes / Tags: `3px` or `6px`
- Alerts / Small panels: `8px`
- Buttons / Course Cards / Modals / Video Players: `12px` (The platform's signature radius)

## 6. Depth & Elevation
The platform feels "flat" and digital.
- **Surface Level 1 (Default)**: Flat white with `#dedee5` borders.
- **Surface Level 2 (Hover/Dropdowns)**: `rgba(16,24,40,0.04) 0px 1px 4px`
- **Surface Level 3 (Modals/Sticky Nav)**: `rgba(0,0,0,0.03) 0px 4px 24px`

## 7. Do's and Don'ts

### Do
- **Do** use Knowledge Purple (`#7132f5`) to guide the learner's eye to the most important action (Next Lesson, Submit Quiz).
- **Do** strictly apply the `12px` radius on all interactive containers to maintain the tech-focused architectural feel.
- **Do** use negative letter-spacing (`-0.5px` to `-1px`) on large display headings to make them look compact and modern.
- **Do** use Borders (`#dedee5`) instead of drop-shadows to separate content on the dashboard.

### Don't
- **Don't** use pill buttons (`50px` radius) — it makes the platform look like a consumer app rather than a professional tool.
- **Don't** use pure black (`#000000`); always use Focus Black (`#101114`) to reduce eye strain.
- **Don't** clutter the reading space. The white canvas must remain stark and clean.

## 8. Agent Prompt Guide (For UI Generation)

### Quick Reference
- Primary Brand/Action: `#7132f5` (Purple)
- Reading Text: `#101114` (Focus Black)
- Metadata/Muted: `#9497a9` (Silver Blue)
- Universal Background: `#ffffff` (White)
- Universal Element Radius: `12px`

### Example Component Prompts
1. **Course Hero Header:** "Create a course hero section. White canvas background. Use a Display font at 36px, weight 700, letter-spacing -0.5px in Focus Black (`#101114`) for the course title. Below it, metadata in Silver Blue (`#9497a9`). Add a primary action button 'Enroll Now' with `#7132f5` background, white text, 13px 20px padding, and strictly a `12px` border radius."

2. **Curriculum List Item:** "Design a curriculum lesson row. Flat white background, bottom border of `1px solid #dedee5`. Left side: Play icon in Cool Gray (`#686b82`), lesson title in Inter 16px/500 Focus Black. Right side: duration badge (12px text, Silver Blue). On hover, change background to `rgba(148,151,169,0.08)`."

3. **Quiz Success State:** "Create a quiz result card. White background, `1px solid #dedee5` border, `12px` radius. Top right badge: 'Passed' with `rgba(20,158,97,0.16)` background and `#026b3f` text (6px radius). Include a Primary button 'Continue to Next Module' in Knowledge Purple (`#7132f5`)."