using System;
using System.Collections.Generic;

namespace CheckersGame
{
    public class Piece
    {
        private int m_Points;
        private bool m_IsKing;

        public ePlayerColor Color { get; }
        public int Row { get; set; }
        public int Col { get; set; }
        public int Points { get; private set; }
        public bool IsKing
        {
            get
            {
                return m_IsKing;
            }
            set
            {
                Points = value == true ? 4 : 1;
                m_IsKing = value;
            }
        }

        public Piece(int i_Row,int i_Col, ePlayerColor i_Color)
        {
            this.Row = i_Row;
            this.Col = i_Col;
            this.Color = i_Color;
            this.IsKing = false;
        }

        public Piece(Piece i_Piece)
        {
            this.Row = i_Piece.Row;
            this.Col = i_Piece.Col;
            this.Color = i_Piece.Color;
            this.IsKing = i_Piece.IsKing;
        }

        public void Position(int i_Row, int i_Col)
        {
            Row = i_Row;
            Col = i_Col;
        }

        internal bool CanJump(Board i_Board)
        {
            int[] rowOffsets = { -2, -2, 2, 2 };
            int[] colOffsets = { -2, 2, -2, 2 };

            for (int i = 0; i < rowOffsets.Length; i++)
            {
                int destRow = this.Row + rowOffsets[i];
                int destCol = this.Col + colOffsets[i];
                Move currentJump = new Move(this.Row, this.Col, destRow, destCol);
                bool jumpIsValid = Move.ValidatePieceJump(currentJump, i_Board, this);
                if (jumpIsValid)
                {
                    return true;
                }
            }

            return false;
        }

        internal List<Move> GetAvailableMoves(Board i_Board, Player i_Player)
        {
            List<Move> availableJumpsList = new List<Move>();
            int[] rowOffsets = { -1, -1, 1, 1, -2, -2, 2, 2 };
            int[] colOffsets = { -1, 1, -1, 1, -2, 2, -2, 2 };

            for (int i = 0; i < rowOffsets.Length; i++)
            {
                int destRow = this.Row + rowOffsets[i];
                int destCol = this.Col + colOffsets[i];
                Move currentMove = new Move(this.Row, this.Col, destRow, destCol);
                if (Move.ValidateMove(currentMove, i_Board, i_Player))
                {
                    availableJumpsList.Add(currentMove);
                }
            }

            return availableJumpsList;
        }
    }
}
