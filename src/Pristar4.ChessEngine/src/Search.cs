using Rudzoft.ChessLib;
using Rudzoft.ChessLib.Enums;
using Rudzoft.ChessLib.MoveGeneration;
using Rudzoft.ChessLib.Types;

namespace Pristar4.ChessEngine
{
    public class Search
    {
        private const int MinScore = -int.MaxValue;
        private const int MaxScore = int.MaxValue;

        public ExtMove Think(IGame game)
        {
            Random random = new Random();
            var legalMoves = game.Pos.GenerateMoves();
            var shuffledMoves = legalMoves.ToArray();
            random.Shuffle( shuffledMoves);


        
            int depth = 2;
            ExtMove bestMove = ExtMove.Empty;
            bestMove.Score = MinScore;

            foreach (var move in shuffledMoves)
            {
                var newExtMove = move;
                game.Pos.MakeMove(move.Move, new State());
                newExtMove.Score = -(int)NegaMax(game.Pos, depth - 1).Raw;
                game.Pos.TakeMove(move.Move);

                // update best value and best move

                if (newExtMove.Score > bestMove.Score)
                {
                    bestMove.Score = newExtMove.Score;
                    bestMove = move.Move;
                }

            }

            return bestMove;
        }


        public static Value NegaMax(IPosition position, int depth)
        {

            ExtMove bestMove = ExtMove.Empty;
            // bestMove.Move -> Move
            // bestMove.Score -> int

            var score = MinScore;


            return score;
        }


        public int MiniMax(IPosition position, int depth, int alpha, int beta)
        {
            if (position.IsMate || position.IsDraw(position.Ply))
            {
                return Evaluate(position);
            }

            if (depth == 0)
            {
                return Evaluate(position);
            }

            int bestValue = MinScore;
var Random = new Random();
            var legalMoves = position.GenerateMoves();
            var shuffledMoves = legalMoves.ToArray();
            Random.Shuffle(shuffledMoves);
            
            


            foreach (var move in shuffledMoves)
            {
                ExtMove extMove = move;
                position.MakeMove(extMove.Move, new State());
                extMove.Score = -MiniMax(position, depth - 1, -beta, -alpha);
                position.TakeMove(extMove.Move);

                // update best value and best move

                if (extMove.Score > bestValue)
                {
                    bestValue = extMove.Score;
                }
            }

            return bestValue;
        }

        private int Evaluate(IPosition position)
        {
            var who2Move = position.SideToMove == Player.White ? 1 : -1;


            int score;

            if (position.IsMate)
            {
                score = -32000 * who2Move;
            }
            else if (!position.IsDraw(position.Ply))
            {
                score = EvaluateMaterial(position) * who2Move;
            }
            else
            {
                score = 0;
            }


            return score;
        }

        private int EvaluateMaterial(IPosition position)
        {
            // something like
            var score = 0;

            // enumerate through all the pieces that are player white

            // we have to go through each square and check if there is a piece on it and add the value of the piece to the score

            for (int i = 0; i < 64; i++)
            {
                var piece = position.GetPiece((Square)i);
                var pieceType = piece.Type();

                score += piece.IsWhite ? GetScore(pieceType) : -GetScore(pieceType);
            }

            return score;
        }


        private int GetScore(PieceTypes piece)
        {
            var score = 0;
            var pieceValue = new PieceValue();


            switch (piece)
            {
                case PieceTypes.Pawn:
                    score = (int)pieceValue.GetPieceValue(PieceTypes.Pawn, Phases.Mg);
                    break;
                case PieceTypes.Knight:
                    score = (int)pieceValue.GetPieceValue(PieceTypes.Knight, Phases.Mg);


                    break;
                case PieceTypes.Bishop:
                    score = (int)pieceValue.GetPieceValue(PieceTypes.Bishop, Phases.Mg);
                    break;
                case PieceTypes.Rook:
                    score = (int)pieceValue.GetPieceValue(PieceTypes.Rook, Phases.Mg);
                    break;
                case PieceTypes.Queen:
                    score = (int)pieceValue.GetPieceValue(PieceTypes.Queen, Phases.Mg);
                    break;
            }


            return score;
        }
    }
}