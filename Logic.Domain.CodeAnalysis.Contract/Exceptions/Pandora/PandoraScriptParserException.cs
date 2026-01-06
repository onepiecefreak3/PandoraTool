namespace Logic.Domain.CodeAnalysis.Contract.Exceptions.Pandora;

public class PandoraScriptParserException : Exception
{
    public int Line { get; }
    public int Column { get; }

    public PandoraScriptParserException()
    {
    }

    public PandoraScriptParserException(string message) : base(message)
    {
    }

    public PandoraScriptParserException(string message, Exception inner) : base(message, inner)
    {
    }

    public PandoraScriptParserException(string message, int line, int column) : base(message)
    {
        Line = line;
        Column = column;
    }
}