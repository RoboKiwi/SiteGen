# Syntax highlighting

Syntax highlighting is applied to fenced code blocks in Markdown files.

````markdown
```javascript
console.log("Hello, world!");
```
````

You can also apply additional arguments to the fenced code block:

## Line numbers

Enable or disable line numbers

## Line highlighting

Highlight specific lines in the code block

## Theme

Override the default theme for the code block

## Highlighting engine

Configure the specific syntax highlighting engine to use for the code block

# Themes and styles

Your site must bundle (recommended) or dynamically the CSS for the syntax highlighting engine you choose.

You can extract the styles using the SiteGen CLI tool.

Try to include both dark and light themes, and use the `prefers-color-scheme` media query to switch between them.

# Supported syntax highlighting engines

Be careful configuring more than one syntax highlighting engine, as they will conflict with each other.

## Pygments

Pygments is written in Python.

You need to have the `pygmentize` command available on your system to use it.

TODO: Rewrite to use `pygments` directly via isolated Python environment.

## Prism.js

```csharp
services.ConfigurePrism();
```

[Prism.js](https://prismjs.com/)

## Monaco Editor

```csharp
services.ConfigureMonaco();
```

The [Monaco Editor](https://microsoft.github.io/monaco-editor/) is the code editor that powers Visual Studio Code.

### Themes

- `vs` Visual Studio Light (default)
- `vs-dark` Visual Studio Dark
- `hc-black` High Contrast Black
- `hc-light` High Contrast Light

## Pending

- [Highlight.js](https://highlightjs.org/)