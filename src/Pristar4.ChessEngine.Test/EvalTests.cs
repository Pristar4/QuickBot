using Rudzoft.ChessLib.Enums;
using Rudzoft.ChessLib.Factories;
using Rudzoft.ChessLib.Fen;
using Rudzoft.ChessLib.Types;

namespace Pristar4.ChessEngine.Test;

public sealed class EvalTests
{
    [Fact]
    public void CountStaticMaterial()
    {
        // construct game and start a new game
        var game = GameFactory.Create(Fen.StartPositionFen);
        var position = game.Pos;
        var whiteMaterial = 0;
        var blackMaterial = 0;
        // count piece count for each side and then compare the material value
        foreach (var piece in position)
        {
            if (piece.Type() == PieceTypes.King) continue;

            if (piece.IsWhite)
                whiteMaterial += (int)position.PieceValue.GetPieceValue(piece, Phases.Mg);
            else if (piece.IsBlack) blackMaterial += (int)position.PieceValue.GetPieceValue(piece, Phases.Mg);
        }

        var score = whiteMaterial - blackMaterial;
        Assert.Equal(0, score);
    }
}