using System;
using System.Collections.Generic;
using System.Linq;

namespace CheckersGame
{
    public class Player
    {
        private string m_Name;

        public ePlayerColor Color { get; internal set; }
        public List<Piece> Pieces { get; set; }
        internal Piece AllowedPiece { get; set; }
        public int Score { get; set; }
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                if (value.Length > 20 || value.Contains(' '))
                {
                    throw new ArgumentException(
                        "Name must be with a maximum of 20 characters and no spaces.");
                }

                m_Name = value;
            }
        }

        public Player(string i_PlayerName)
        {
            Name = i_PlayerName;
            Pieces = new List<Piece>();
        }
        public Player(Player i_Player)
        {
            this.Name = i_Player.Name;
            this.Color = i_Player.Color;
            this.Score = i_Player.Score;
            this.Pieces = Pieces = new List<Piece>();
        }

        public List<Move> GetAvailableMoves(Board i_Board)
        {
            List<Move> availableMoves = new List<Move>();
            if(AllowedPiece == null)
            {
                foreach (Piece piece in Pieces)
                {
                    availableMoves.AddRange(piece.GetAvailableMoves(i_Board, this));
                }
            }
            else
            {
                availableMoves.AddRange(AllowedPiece.GetAvailableMoves(i_Board, this));
            }

            if (availableMoves.Count == 0)
            {
                return availableMoves;
            }

            int maxDistance = availableMoves.Max(move => move.Distance);
            List<Move> movesWithMaxDistance = availableMoves.Where(move => move.Distance == maxDistance).ToList();

            return movesWithMaxDistance;
        }

        internal bool IsJumpAvailable(Board i_Board)
        {
            foreach(Piece currentPiece in this.Pieces)
            {
                if(currentPiece.CanJump(i_Board) == true)
                {
                    return true;
                }
            }

            return false;
        }

        internal virtual Move DecideMove(Board i_Board, Game game = null)
        {
            return null;
        }

        public void RemovePiece(Piece piece)
        {
            Pieces.Remove(piece);
        }

        public void AddPiece(Piece piece)
        {
            Pieces.Add(piece);
        }
    }

    public class CPUPlayer : Player
    {
        public CPUPlayer() : base("CPUPlayer") { }

        internal Move GetRandomValidMove(Board i_Board)
        {
            List<Move> validMoves = this.GetAvailableMoves(i_Board);
            Random random = new Random();

            return validMoves[random.Next(validMoves.Count)];
        }

        internal override Move DecideMove(Board i_Board, Game game = null)
        {
            return GetRandomValidMove(i_Board);
        }
    }

    public class AIPlayer : Player
    {
        private readonly int r_MaxDepth = 10;

        public AIPlayer() : base("AIPlayer") { }

        public AIPlayer(AIPlayer i_AIPlayer) : base(i_AIPlayer)
        {
            this.r_MaxDepth = i_AIPlayer.r_MaxDepth;
        }

        internal override Move DecideMove(Board i_Board, Game i_Game)
        {
            return findBestMove(i_Game);
        }

        private Move findBestMove(Game i_Game)
        {
            int bestScore = int.MinValue;
            List<Move> bestMoves = new List<Move>();

            List<Move> availableMoves = this.GetAvailableMoves(i_Game.Board);

            foreach (Move move in availableMoves)
            {
                Game simulatedGame = new Game(i_Game);
                simulateMove(move, simulatedGame);
                bool thisPlayer = i_Game.CurrentPlayer == simulatedGame.CurrentPlayer;
                int moveScore = alphaBeta(simulatedGame, r_MaxDepth, int.MinValue, int.MaxValue, thisPlayer);
                if (moveScore > bestScore)
                {
                    bestScore = moveScore;
                    bestMoves.Clear();
                    bestMoves.Add(move);
                }
                else if (moveScore == bestScore)
                {
                    bestMoves.Add(move);
                }
            }
            if (bestMoves.Count > 0)
            {
                Random random = new Random();
                return bestMoves[random.Next(bestMoves.Count)];
            }

            return null;
        }

        private static void simulateMove(Move i_Move, Game i_Game)
        {
            i_Game.MakeMove(i_Move);
        }

        private int alphaBeta(Game i_Game, int i_Depth, int i_Alpha, int i_Beta, bool i_MaximizingPlayer)
        {
            if (i_Depth == 0 || i_Game.Winner == null)
            {
                return evaluateBoard(i_Game);
            }

            List<Move> moves = null;
            moves = i_Game.CurrentPlayer.GetAvailableMoves(i_Game.Board);
            if (moves.Count == 0)
            {
                return evaluateBoard(i_Game);
            }

            if (i_MaximizingPlayer)
            {
                int maxEval = int.MinValue;
                foreach (Move move in moves)
                {
                    Game simulatedGame = new Game(i_Game);
                    simulateMove(move, simulatedGame);
                    bool thisPlayer = i_Game.CurrentPlayer == simulatedGame.CurrentPlayer;
                    int eval = alphaBeta(simulatedGame, i_Depth - 1, i_Alpha, i_Beta, thisPlayer);
                    maxEval = Math.Max(maxEval, eval);
                    i_Alpha = Math.Max(i_Alpha, eval);
                    if (i_Beta <= i_Alpha)
                    {
                        break;
                    }
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                foreach (Move move in moves)
                {
                    Game simulatedGame = new Game(i_Game);
                    simulateMove(move, simulatedGame);
                    bool thisPlayer = i_Game.CurrentPlayer == simulatedGame.CurrentPlayer;
                    int eval = alphaBeta(simulatedGame, i_Depth - 1, i_Alpha, i_Beta, !thisPlayer);
                    minEval = Math.Min(minEval, eval);
                    i_Beta = Math.Min(i_Beta, eval);
                    if (i_Beta <= i_Alpha)
                    {
                        break;
                    }
                }
                return minEval;
            }
        }
        private int evaluateBoard(Game i_Game)
        {
            int score = 0;
            Board board = i_Game.Board;

            for (int i = 0; i < board.Size; i++)
            {
                for (int j = 0; j < board.Size; j++)
                {
                    Piece piece = board[i, j];

                    if (piece != null)
                    {
                        int pieceValue = piece.Color == this.Color ? piece.Points : -piece.Points;
                        score += pieceValue;

                        int safetyValue = evaluateSafety(piece, i_Game);
                        score += safetyValue;
                    }
                }
            }

            return score;
        }

        private static int evaluateSafety(Piece piece, Game i_Game)
        {
            if (isPieceUnderThreat(piece, i_Game))
            {
                return -2;
            }

            return 0;
        }

        private static bool isPieceUnderThreat(Piece piece, Game i_Game)
        {
            Player opponent = i_Game.CurrentPlayer;
            List<Move> opponentMoves = opponent.GetAvailableMoves(i_Game.Board);

            foreach (Move move in opponentMoves)
            {
                if (i_Game.Board.GetJumpedPiece(move) == piece)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
