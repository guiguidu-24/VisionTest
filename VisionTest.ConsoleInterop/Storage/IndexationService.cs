using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Globalization;
using VisionTest.Core.Models;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace VisionTest.ConsoleInterop.Storage;

public class IndexationService(string enumFilePath, string projectDirectory)
{
    private readonly string _enumFilePath = enumFilePath;
    private readonly string _projectDirectory = projectDirectory;

    public async Task RemoveElementFromIndexAsync(string id)
    {
        if (File.Exists(_enumFilePath))
        {
            var lines = (await File.ReadAllLinesAsync(_enumFilePath)).ToList();
            lines.RemoveAll(line => line.Contains("public const " + id + " "));

            await File.WriteAllLinesAsync(_enumFilePath, lines);
        }
        else
        {
            throw new FileNotFoundException($"Enum file '{_enumFilePath}' does not exist.");
        }
    }


    public async Task AddElementToIndexAsync(ScreenElement screenElement)
    {
        var parts = screenElement.Id.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
        var classNames = GetClassNames(parts);
        var constField = BuildConstField(parts.Last(), screenElement.Id);

        if (!File.Exists(_enumFilePath))
            await WriteBootstrapFileAsync(classNames, constField);
        else
            await UpdateExistingFileAsync(classNames, constField);
    }

    private static List<string> GetClassNames(string[] parts) =>
        parts.Length > 1
            ? parts.Take(parts.Length - 1)
                   .Select(p => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(p))
                   .ToList()
            : new List<string>();

    private static FieldDeclarationSyntax BuildConstField(string name, string value) =>
        FieldDeclaration(
            VariableDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)))
            .AddVariables(
                VariableDeclarator(name)
                    .WithInitializer(
                        EqualsValueClause(
                            LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                Literal(value.Replace('\\', '/')))))))
        .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.ConstKeyword))
        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

    /// <summary>
    /// Helper to create the root ScreenElements class with all fields.
    /// </summary>
    private static ClassDeclarationSyntax CreateRootScreenElementsClass(IEnumerable<(List<string> classNames, FieldDeclarationSyntax constField)> fields)
    {
        var rootClass = ClassDeclaration("ScreenElements")
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
            .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
            .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));

        var tree = rootClass;
        foreach (var (classNames, constField) in fields)
        {
            tree = AddMembersRecursively(tree, classNames, constField);
        }
        return tree;
    }

    private async Task WriteBootstrapFileAsync(List<string> classNames, FieldDeclarationSyntax constField)
    {
        var fields = new List<(List<string>, FieldDeclarationSyntax)> { (classNames, constField) };
        var screenElementsClass = CreateRootScreenElementsClass(fields);
        var @namespace = NamespaceDeclaration(
                             IdentifierName(Path.GetFileName(
                                 _projectDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))))
                         .AddMembers(screenElementsClass);
        var compilationUnit = CompilationUnit()
            .AddMembers(@namespace)
            .NormalizeWhitespace();
        await File.WriteAllTextAsync(_enumFilePath, compilationUnit.ToFullString());
    }

    private async Task UpdateExistingFileAsync(List<string> classNames, FieldDeclarationSyntax constField)
    {
        var text = await File.ReadAllTextAsync(_enumFilePath);
        var root = (CompilationUnitSyntax)CSharpSyntaxTree.ParseText(text).GetRoot();
        var ns = root.Members.OfType<BaseNamespaceDeclarationSyntax>().First();
        var old = ns.Members.OfType<ClassDeclarationSyntax>()
                    .First(cd => cd.Identifier.ValueText == "ScreenElements");

        var updated = AddMembersRecursively(old, classNames, constField);
        var newNs = ns.ReplaceNode(old, updated);
        var newRoot = root.ReplaceNode(ns, newNs).NormalizeWhitespace();

        await File.WriteAllTextAsync(_enumFilePath, newRoot.ToFullString());
    }

    private static ClassDeclarationSyntax BuildNestedClassChain(string rootName,
    List<string> nestedNames, FieldDeclarationSyntax constField)
    {
        // build bottom-up: innermost first
        ClassDeclarationSyntax inner = ClassDeclaration(rootName)
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
            .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
            .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken));

        if (!nestedNames.Any())
            return inner.AddMembers(constField);

        // innermost with const
        inner = ClassDeclaration(nestedNames.Last())
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
            .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
            .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken))
            .AddMembers(constField);

        // wrap outer levels
        foreach (var name in nestedNames.Take(nestedNames.Count - 1).Reverse())
        {
            inner = ClassDeclaration(name)
                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
                .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken))
                .AddMembers(inner);
        }

        // attach chain under root
        return ClassDeclaration(rootName)
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
            .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
            .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken))
            .AddMembers(inner);
    }

    private static ClassDeclarationSyntax AddMembersRecursively(
     ClassDeclarationSyntax rootClass,
     List<string> nestedNames,
     FieldDeclarationSyntax constField)
    {
        // No nesting → just add the const directly
        if (!nestedNames.Any())
            return rootClass.AddMembers(constField);

        // Look at the first level
        var level1 = nestedNames[0];
        var existing = rootClass.Members
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault(c => c.Identifier.ValueText == level1);

        ClassDeclarationSyntax updatedNested;
        if (existing != null)
        {
            // Recurse into the existing class
            updatedNested = AddMembersRecursively(
                existing,
                nestedNames.Skip(1).ToList(),
                constField);
        }
        else
        {
            // Build a fresh chain for all remaining levels
            updatedNested = BuildNestedClassChain(level1, nestedNames.Skip(1).ToList(), constField);
        }

        // Replace or add at this level
        return existing != null
            ? rootClass.ReplaceNode(existing, updatedNested)
            : rootClass.AddMembers(updatedNested);
    }

    /// <summary>
    /// Updates the ScreenElements.cs file with all existing screen elements,
    /// organizing them into nested static classes based on their directory segments.
    /// </summary>
    public async Task RebuildIndexAsync(IEnumerable<string> allIds)
    {
        // 2. Delete existing file so we start fresh
        if (File.Exists(_enumFilePath))
            File.Delete(_enumFilePath);

        // 3. Build the tree using Roslyn syntax
        var fields = new List<(List<string> classNames, FieldDeclarationSyntax constField)>();
        foreach (var id in allIds)
        {
            var parts = id.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            var classNames = GetClassNames(parts);
            var constField = BuildConstField(parts.Last(), id);
            fields.Add((classNames, constField));
        }

        // Use the helper to create the root class
        var tree = CreateRootScreenElementsClass(fields);

        // Compose the namespace and compilation unit
        var ns = Path.GetFileName(_projectDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        var @namespace = NamespaceDeclaration(IdentifierName(ns)).AddMembers(tree);
        var compilationUnit = CompilationUnit().AddMembers(@namespace).NormalizeWhitespace();

        // 4. Write the file
        await File.WriteAllTextAsync(_enumFilePath, compilationUnit.ToFullString());
    }
}