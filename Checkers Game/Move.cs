using System;

namespace CheckersGame
{
    public class Move
    {
        public int SrcRow { get; set; }
        public int SrcCol { get; set; }
        public int DestRow { get; set; }
        public int DestCol { get; set; }
        public int Distance { get; set; }

        public Move(int i_SrcRow, int i_SrcCol, int i_DestRow, int i_DestCol)
        {
            SrcRow = i_SrcRow;
            SrcCol = i_SrcCol;
            DestRow = i_DestRow;
            DestCol = i_DestCol;
            Distance = Math.Abs(i_DestRow - i_SrcRow);
        }

        public static bool ValidateMove(Move i_Move, Board i_Board, Player i_Player)
        {
            if (i_Move == null)
            {
                return false;
            }

            if (!i_Board.IsWithinBounds(i_Move.SrcRow, i_Move.SrcCol) || !i_Board.IsWithinBounds(i_Move.DestRow, i_Move.DestCol))
            {
                return false;
            }

            if (i_Player.AllowedPiece != null)
            {
                if (i_Move.SrcRow != i_Player.AllowedPiece.Row || i_Move.SrcCol != i_Player.AllowedPiece.Col)
                {
                    return false;
                }
            }

            Piece startPiece = i_Board.Matrix[i_Move.SrcRow, i_Move.SrcCol];
            if (startPiece == null || startPiece.Color != i_Player.Color)
            {
                return false;
            }

            if (i_Board[i_Move.DestRow, i_Move.DestCol] != null)
            {
                return false;
            }

            int rowDistance = Math.Abs(i_Move.DestRow - i_Move.SrcRow);
            int colDistance = Math.Abs(i_Move.DestCol - i_Move.SrcCol);
            if (isForwardMove(i_Move.SrcRow, i_Move.DestRow, startPiece) == false)
            {
                return false;
            }

            if (rowDistance == 2 && colDistance == 2)
            {
                int middleX = (i_Move.SrcRow + i_Move.DestRow) / 2;
                int middleY = (i_Move.SrcCol + i_Move.DestCol) / 2;

                Piece middlePiece = i_Board.Matrix[middleX, middleY];
                return middlePiece != null && middlePiece.Color != i_Player.Color;
            }
            else if (rowDistance == 1 && colDistance == 1)
            {
                return true;
            }

            return false;
        }

        internal static bool ValidatePieceJump(Move i_Move, Board i_Board, Piece i_Piece)
        {
            if (!i_Board.IsWithinBounds(i_Move.DestRow, i_Move.DestCol))
            {
                return false;
            }

            Piece destPiece = i_Board[i_Move.DestRow, i_Move.DestCol];
            if (destPiece != null)
            {
                return false;
            }

            if (!Move.isForwardMove(i_Move.SrcRow, i_Move.DestRow, i_Piece))
            {
                return false;
            }

            Piece jumpedPiece = i_Board.GetJumpedPiece(i_Move);
            if (jumpedPiece == null || jumpedPiece.Color == i_Piece.Color)
            {
                return false;
            }

            return true;
        }

        internal static bool isForwardMove(int i_SrcRow, int i_DestRow, Piece i_Piece)
        {
            if (i_Piece == null)
            {
                return false;
            }

            if (i_Piece.IsKing)
            {
                return true;
            }

            if (i_Piece.Color == ePlayerColor.White)
            {
                return i_DestRow < i_SrcRow;
            }

            else
            {
                return i_DestRow > i_SrcRow;
            }
        }
    }
}
