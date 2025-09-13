# BBCode.WPF

Lightweight BBCode rendering control for WPF (.NET Framework 4.6.2, .NET 8/9 Windows). Provides a `BBCodeBlock` control that parses a practical subset of BBCode into WPF inline elements.

---

## Installation
Add the package (once published) via NuGet:
```
Install-Package BBCode.WPF
```
Or reference the project directly.

---

## XAML Namespace
Using XmlnsDefinition (recommended):
```xaml
xmlns:bbcode="https://github.com/ChrisPulman/BBCode.WPF"
```
Fallback (direct CLR):
```xaml
xmlns:bbcode="clr-namespace:CP.BBCode.WPF;assembly=BBCode.WPF"
```

---

## Basic Usage
```xaml
<bbcode:BBCodeBlock BBCode="Hello [b]World[/b][br]Visit [url=https://example.com]Example[/url]" />
```
The text is parsed on load and whenever the `BBCode` property changes.

---

## Supported Tags
| Tag | Syntax | Description |
|-----|--------|-------------|
| Line Break | `[br]` | Inserts a line break. |
| Bold | `[b]text[/b]` | Bold text. |
| Italic | `[i]text[/i]` | Italic text. |
| Underline | `[u]text[/u]` | Underlined text. |
| Strikethrough | `[s]text[/s]` | Strikethrough. |
| Font Size | `[size=18]text[/size]` | Sets font size (double). |
| Font Family | `[font=Consolas]text[/font]` | Sets font family. |
| Color | `[color=Red]text[/color]` or `[color=#FF336699]text[/color]` | Sets foreground color. |
| Link (named) | `[url=https://site]caption[/url]` | Clickable hyperlink. |
| Link (auto) | `[url]https://site[/url]` | URL used as text + target. |
| Command Link | `[url=cmd:Parameter]Execute[/url]` | Executes bound `ICommand` with `Parameter`. |
| Email | `[email]user@host.com[/email]` or `[email=user@host.com]Mail me[/email]` | Creates `mailto:` link. |
| Image | `[img=https://host/image.png]caption[/img]` | Displays image (caption optional). |
| Image (sized) | `[img=https://img,width=200,height=120]caption[/img]` | Width/Height optional (any order). |
| Quote | `[quote]text[/quote]` | Italic, gray quoted block. |
| Quote (with author) | `[quote=Alice]text[/quote]` | Adds an “Alice wrote:” header. |
| Code | `[code]var x = 42;[/code]` | Monospaced with light background. |
| List | `[list][*]Item 1[*]Item 2[/list]` | Bullet list (simple). |

---

## Lists
Minimal bullet support (no nesting / numbering):
```bbcode
[list]
[*] First item
[*] Second item
[*] Third item
[/list]
```

---

## Images
```bbcode
[img=https://domain/image.png]Optional caption[/img]
[img=https://domain/image.png,width=150,height=90]Scaled image[/img]
```
Order of `width` / `height` does not matter. Omit either to leave it unset.

---

## Links & Commands
Hyperlinks open via `Process.Start` (shell execute). Suppress failures silently.

Command links:
```xaml
<bbcode:BBCodeBlock BBCode="Click [url=cmd:Refresh]here[/url] to refresh" Command="{Binding RefreshCommand}"/>
```
When clicked: parameter = `Refresh` (substring after `cmd:`). Command receives string via `CommandParameter`.

---

## Email
```bbcode
[email]user@host.com[/email]
[email=user@host.com]Contact us[/email]
```
Generates `mailto:` links.

---
## Quotes
Generic quote:
```bbcode
[quote]This is a quoted text.[/quote]
```
Quote with author:
```bbcode
[quote=Author]This is a quoted text with an author.[/quote]
```

---

## Code
```bbcode
[code]
for (int i = 0; i < 3; i++)
{
    Console.WriteLine(i);
}
[/code]
```
Renders in Consolas (if available) with light gray background.

---

## MVVM Example
```xaml
<bbcode:BBCodeBlock
    BBCode="Reload data: [url=cmd:Reload]Reload[/url]"
    Command="{Binding ReloadCommand}" />
```
```csharp
public ICommand ReloadCommand => new RelayCommand(p => Reload(p));
private void Reload(object? parameter)
{
    // parameter == "Reload"
}
```

---

## Dynamic Updates
Bind `BBCode` to a view model property; control re-parses on change.
```xaml
<bbcode:BBCodeBlock BBCode="{Binding PreviewText}" />
```

---

## Error Handling
If parsing fails, the raw BBCode string is displayed as plain text.

---

## Limitations / Notes
- No nested lists or ordered lists yet.
- Quote / code blocks are inline-based (no full block layout containers).
- Color names and hex codes rely on `ColorConverter` (system-defined names only).
- Parsing is lightweight; not a full spec implementation.
- Silent failures for invalid tags / images that cannot load.

---

## Roadmap Ideas
- Numbered lists `[list=1]`.
- Nested list support.
- Table tags.
- Spoiler / collapse blocks.
- Custom tag extensibility.

---

## Contributing
PRs / issues welcome. Keep additions lightweight and safe for partial trust UI scenarios.

---

## License
MIT License. See LICENSE file.

---

## Quick Reference (Cheat Sheet)
```
[b]bold[/b] [i]italic[/i] [u]underline[/u] [s]strike[/s]
[size=20]big[/size] [font=Consolas]mono[/font]
[color=#FF8800]orange[/color]
[url=https://example.com]Example[/url]
[url]https://example.com[/url]
[email]user@host.com[/email]
[img=https://host/x.png,width=100,height=60]caption[/img]
[quote=Alice]Hi[/quote]
[code]int x = 1;[/code]
[list][*]One[*]Two[*]Three[/list]

---

**BBCode.Wpf** - By Chris Pulman - Empowering Industrial Automation with Reactive Technology ⚡🏭
