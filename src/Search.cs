using Rudzoft.ChessLib;
using Rudzoft.ChessLib.MoveGeneration;
using Rudzoft.ChessLib.Types;

namespace QuickBot {
    public class Search {
        public Move Think(IGame game) {

            var bestMove = Move.EmptyMove;

            var bestValue = -32001;

            var legalMoves = game.Pos.GenerateMoves();

            foreach (var move in legalMoves) {
                game.Pos.MakeMove(move.Move, new State());
                var newValue = -NegaMax(game.Pos, 1, -32001, 32001);
                game.Pos.TakeMove(move.Move);

                // update best value and best move

                if (newValue > bestValue) {
                    bestValue = newValue;
                    bestMove = move.Move;
                }
                Console.WriteLine($"Move: {move.Move} Value: {newValue}");
            }

            return bestMove;
        }

        public int NegaMax(IPosition position, int depth, int alpha, int beta) {
            if (depth == 0) {
                return Evaluate(position);
            }

            int bestValue = -32001;


            var legalMoves = position.GenerateMoves();


            foreach (var move in legalMoves) {
                position.MakeMove(move.Move, new State());
                var newValue = -NegaMax(position, depth - 1, -beta, -alpha);
                position.TakeMove(move.Move);

                // update best value and best move

                if (newValue > bestValue) {
                    bestValue = newValue;
                }
            }

            return bestValue;
        }
        /// <summary>
        /// In order for negaMax to work,  Evaluation function must return a score relative to the side to being evaluated, e.g. the simplest score evaluation could be:
        /// score = materialWeight * (numWhitePieces - numBlackPieces) * who2move
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private int Evaluate(IPosition position) {
            var who2Move = position.SideToMove == Player.White ? 1 : -1;

            var score = 0;

            var materialWeight = 100;


            var numWhitePiece = 0;

            // count white pieces there are six piece types
            // PieceTypes.Bishop
            for (var i = 0; i < 6; i++) {
                switch (i) {
                    case 0:
                        numWhitePiece += position.Board.Pieces(Player.White, PieceTypes.Pawn).Count;
                        break;
                    case 1:
                        numWhitePiece += position.Board.Pieces(Player.White, PieceTypes.Knight)
                                .Count;
                        break;
                    case 2:
                        numWhitePiece += position.Board.Pieces(Player.White, PieceTypes.Bishop)
                                .Count;
                        break;
                    case 3:
                        numWhitePiece += position.Board.Pieces(Player.White, PieceTypes.Rook).Count;
                        break;
                    case 4:
                        numWhitePiece +=
                                position.Board.Pieces(Player.White, PieceTypes.Queen).Count;
                        break;
                    case 5:
                        numWhitePiece += position.Board.Pieces(Player.White, PieceTypes.King).Count;
                        break;
                }
            }

            var numBlackPieces = 0;

            for (var i = 0; i < 6; i++) {
                switch (i) {
                    case 0:
                        numBlackPieces +=
                                position.Board.Pieces(Player.Black, PieceTypes.Pawn).Count;
                        break;
                    case 1:
                        numBlackPieces += position.Board.Pieces(Player.Black, PieceTypes.Knight)
                                .Count;
                        break;
                    case 2:
                        numBlackPieces += position.Board.Pieces(Player.Black, PieceTypes.Bishop)
                                .Count;
                        break;
                    case 3:
                        numBlackPieces +=
                                position.Board.Pieces(Player.Black, PieceTypes.Rook).Count;
                        break;
                    case 4:
                        numBlackPieces +=
                                position.Board.Pieces(Player.Black, PieceTypes.Queen).Count;
                        break;
                    case 5:
                        numBlackPieces +=
                                position.Board.Pieces(Player.Black, PieceTypes.King).Count;
                        break;
                }
            }

            score = materialWeight * (numWhitePiece - numBlackPieces) * who2Move;

            return score;
        }
    }
}