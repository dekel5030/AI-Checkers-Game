using System;

namespace CheckersGame
{
    public class Board
    {
        private readonly int m_Size;

        public Piece[,] Matrix { get; }
        public int Size
        {
            get { return m_Size; }
        }
        public Piece this[int i_Row, int i_Col]
        {
            get { return Matrix[i_Row, i_Col]; }
            set { Matrix[i_Row, i_Col] = value; }
        }

        public Board(int i_Size)
        {
            if (i_Size != 6 && i_Size != 8 && i_Size != 10)
            {
                throw new ArgumentException("Board size must be 6, 8, or 10.");
            }

            m_Size = i_Size;
            Matrix = new Piece[Size, Size];
        }

        public Board(Board i_Board)
        {
            this.m_Size = i_Board.Size;
            Matrix = new Piece[Size, Size];
            for (int row = 0; row < i_Board.Size; row++)
            {
                for (int col = 0; col < i_Board.Size; col++)
                {
                    Piece originalPiece = i_Board[row, col];
                    if (originalPiece != null)
                    {
                        Matrix[row, col] = new Piece(originalPiece);
                    }
                }
            }
        }

        internal void InitializeBoard()
        {
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    if ((row + col) % 2 != 0)
                    {
                        if (row < Size / 2 - 1)
                        {
                            Matrix[row, col] = new Piece(row, col, ePlayerColor.Black);
                        }
                        else if (row > Size / 2)
                        {
                            Matrix[row, col] = new Piece(row, col, ePlayerColor.White);
                        }
                        else
                        {
                            Matrix[row, col] = null;
                        }
                    }
                }
            }
        }

        internal bool IsWithinBounds(int i_Row, int i_Col)
        {
            return i_Row >= 0 && i_Row < Size && i_Col >= 0 && i_Col < Size;
        }

        internal void RemovePiece(Piece i_Piece)
        {
            Matrix[i_Piece.Row, i_Piece.Col] = null;
        }

        internal void MovePiece(Piece i_Piece, int i_DestRow, int i_DestCol)
        {
            this[i_Piece.Row, i_Piece.Col] = null;
            i_Piece.Position(i_DestRow,i_DestCol);
            this[i_DestRow, i_DestCol] = i_Piece;
            if (i_Piece.Color == ePlayerColor.White && i_DestRow == 0)
            {
                i_Piece.IsKing = true;
            }
            else if (i_Piece.Color == ePlayerColor.Black && i_DestRow == this.Size - 1)
            {
                i_Piece.IsKing = true;
            }
        }

        public Piece GetJumpedPiece(Move i_Move)
        {
            if(i_Move.Distance != 2)
            {
                return null;
            }
            int jumpedRow = (i_Move.DestRow + i_Move.SrcRow) / 2;
            int jumpedCol = (i_Move.DestCol + i_Move.SrcCol) / 2;

            return this[jumpedRow, jumpedCol];
        }

        public void CalculatePoints(Player io_Player1, Player io_Player2)
        {
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    if ((row + col) % 2 != 0)
                    {
                        Piece currentPiece = Matrix[row, col];
                        if (currentPiece == null)
                        {
                            continue;
                        }

                        if (currentPiece.Color == io_Player1.Color)
                        {
                            io_Player1.Score += currentPiece.Points;
                        }
                        else
                        {
                            io_Player2.Score += currentPiece.Points;
                        }
                    }
                }
            }
        }
    }
}
