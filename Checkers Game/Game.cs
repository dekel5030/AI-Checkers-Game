using System;
using System.Collections.Generic;

namespace CheckersGame
{
    public class Game
    {
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Board Board { get; }
        public Player CurrentPlayer { get; private set; }
        public Player Winner { get; private set; }
        public Player Loser { get; private set; }

        public Game(Board i_Board, Player i_Player1, Player i_Player2)
        {
            Player1 = i_Player1;
            Player2 = i_Player2;
            Player1.Color = ePlayerColor.White;
            Player2.Color = ePlayerColor.Black;
            Board = i_Board;
            ResetGame();
        }

        public Game(Game i_Game)
        {
            this.Player1 = new Player(i_Game.Player1);
            this.Player2 = new Player(i_Game.Player2);
            this.Board = new Board(i_Game.Board);
            resetPlayerPieces();
            this.CurrentPlayer = i_Game.CurrentPlayer == i_Game.Player1 ? this.Player1 : this.Player2;
            this.Winner = i_Game.Winner;
            this.Loser = i_Game.Loser;
        }

        private void resetPlayerPieces()
        {
            foreach(Piece piece in Board.Matrix)
            {
                if(piece != null)
                {
                    if(piece.Color == Player1.Color)
                    {
                        Player1.AddPiece(piece);
                    }
                    else
                    {
                        Player2.AddPiece(piece);
                    }
                }
            }
        }

        public Move MakeMove(Move i_Move = null)
        {
            if(i_Move == null)
            {
                i_Move = CurrentPlayer.DecideMove(Board, this);
            }
            else
            {
                if(validateMove(i_Move) == false)
                {
                    throw new ArgumentException("The move isn't valid.");
                }
            }

            Piece currentPiece = Board[i_Move.SrcRow, i_Move.SrcCol];
            Board.MovePiece(currentPiece, i_Move.DestRow, i_Move.DestCol);
            if (i_Move.Distance == 2)
            {
                Piece jumpedPiece = Board.GetJumpedPiece(i_Move);
                removePiece(jumpedPiece);
                if(currentPiece.CanJump(Board))
                {
                    CurrentPlayer.AllowedPiece = currentPiece;
                    return i_Move;
                }
            }

            if (isThereWinner() == true)
            {
                Board.CalculatePoints(Player1, Player2);
            }

            switchTurn();

            return i_Move;
        }

        private bool validateMove(Move i_Move)
        {
            if (Move.ValidateMove(i_Move, Board, CurrentPlayer) == false)
            {
                return false;
            }
            if (i_Move.Distance == 1 && CurrentPlayer.IsJumpAvailable(Board) == true)
            {
                return false;
            }

            return true;
        }

        private void removePiece(Piece i_Piece)
        {
            Board.RemovePiece(i_Piece);
            Player opponent = GetOpponent();
            opponent.RemovePiece(i_Piece);
        }

        private bool isThereWinner()
        {
            Player opponent = GetOpponent();
            List<Move> moves = opponent.GetAvailableMoves(Board);
            if (moves.Count == 0)
            {
                setWinner();
                return true;
            }
            return false;
        }

        private void switchTurn()
        {
            CurrentPlayer.AllowedPiece = null;
            CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
        }

        public void Surrender()
        {
            setLoser();
            Board.CalculatePoints(Player1, Player2);
        }

        private void setWinner()
        {
            Winner = CurrentPlayer;
            Loser = GetOpponent();
        }

        private void setLoser()
        {
            Loser = CurrentPlayer;
            Winner = GetOpponent();
        }

        internal Player GetOpponent()
        {
            return CurrentPlayer == Player1 ? Player2 : Player1;
        }

        public void ResetGame()
        {
            Loser = null;
            Winner = null;
            Player1.AllowedPiece = null;
            Player2.AllowedPiece = null;
            Board.InitializeBoard();
            Player1.Pieces.Clear();
            Player2.Pieces.Clear();
            setPlayersPieces();
            CurrentPlayer = Player1;
        }

        private void setPlayersPieces()
        {
            for(int i = 0; i < Board.Size; i++)
            {
                for(int j = 0; j < Board.Size; j++)
                {
                    Piece currentPiece = Board[i, j];
                    if(currentPiece != null)
                    {
                        if(currentPiece.Color == Player1.Color)
                        {
                            Player1.AddPiece(currentPiece);
                        }
                        else
                        {
                            Player2.AddPiece(currentPiece);
                        }
                    }
                }
            }
        }
    }
}
