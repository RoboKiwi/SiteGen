namespace SiteGen.Core.Models.Hierarchy;

/// <summary>
/// Occurs when an operation involving a <see cref="ITreeEntity{T}"/> or
/// related classes fails.
/// </summary>
public class TreeEntityException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TreeEntityException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="args">The args.</param>
    public TreeEntityException(string message, params object[] args) : base( string.Format(message, args)) {}
}