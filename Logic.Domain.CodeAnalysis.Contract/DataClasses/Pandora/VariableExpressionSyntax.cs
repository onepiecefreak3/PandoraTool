namespace Logic.Domain.CodeAnalysis.Contract.DataClasses.Pandora;

public class VariableExpressionSyntax : ExpressionSyntax
{
    public SyntaxToken Vars { get; private set; }
    public SyntaxToken BracketOpen { get; private set; }
    public ExpressionSyntax Expression { get; private set; }
    public SyntaxToken BracketClose { get; private set; }

    public override SyntaxLocation Location => Vars.FullLocation;
    public override SyntaxSpan Span => new(Vars.FullSpan.Position, BracketClose.FullSpan.EndPosition);

    public VariableExpressionSyntax(SyntaxToken varsKeyword, SyntaxToken bracketOpen, ExpressionSyntax expression, SyntaxToken bracketClose)
    {
        varsKeyword.Parent = this;
        bracketOpen.Parent = this;
        expression.Parent = this;
        bracketClose.Parent = this;

        Vars = varsKeyword;
        BracketOpen = bracketOpen;
        Expression = expression;
        BracketClose = bracketClose;

        Root.Update();
    }

    public void SetVars(SyntaxToken varsKeyword, bool updatePositions = true)
    {
        varsKeyword.Parent = this;

        Vars = varsKeyword;

        if (updatePositions)
            Root.Update();
    }

    public void SetBracketOpen(SyntaxToken bracketOpen, bool updatePositions = true)
    {
        bracketOpen.Parent = this;

        BracketOpen = bracketOpen;

        if (updatePositions)
            Root.Update();
    }

    public void SetExpression(ExpressionSyntax expression, bool updatePositions = true)
    {
        expression.Parent = this;

        Expression = expression;

        if (updatePositions)
            Root.Update();
    }

    public void SetBracketClose(SyntaxToken bracketClose, bool updatePositions = true)
    {
        bracketClose.Parent = this;

        BracketClose = bracketClose;

        if (updatePositions)
            Root.Update();
    }

    internal override int UpdatePosition(int position, ref int line, ref int column)
    {
        SyntaxToken vars = Vars;
        SyntaxToken bracketOpen = BracketOpen;
        SyntaxToken bracketClose = BracketClose;

        position = vars.UpdatePosition(position, ref line, ref column);
        position = bracketOpen.UpdatePosition(position, ref line, ref column);
        position = Expression.UpdatePosition(position, ref line, ref column);
        position = bracketClose.UpdatePosition(position, ref line, ref column);

        Vars = vars;
        BracketOpen = bracketOpen;
        BracketClose = bracketClose;

        return position;
    }
}