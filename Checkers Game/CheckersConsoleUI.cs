using System;
using System.Linq;

namespace CheckersGame
{
    public class CheckersGameConsoleUI
    {
        private Game m_Game;

        public CheckersGameConsoleUI()
        {
            Player player1 = createPlayer("Enter Player'1 name: ", eMode.Player);
            Board board = createBoard();
            eMode mode = getPlayMode();
            Player player2 = createPlayer("Enter Player'2 name: ", mode);
            m_Game = new Game(board, player1, player2);
        }

        public void StartGameSeries()
        {
            string input = "1";
            Move move = null;
            while (input == "1")
            {
                StartGame();
                displayScore();
                Console.WriteLine("Would you like to play another game? (1 - Yes) ");
                m_Game.ResetGame();
                input = Console.ReadLine();
            }
            Console.WriteLine("Thanks for playing");
            Console.ReadLine();
        }

        public void StartGame()
        {
            PrintBoard(m_Game.Board);
            while (m_Game.Winner == null)
            {
                try
                {
                    Move move;
                    if (m_Game.CurrentPlayer is CPUPlayer || m_Game.CurrentPlayer is AIPlayer)
                    {
                        Player currPlayer = m_Game.CurrentPlayer;
                        Console.WriteLine("Computer’s Turn (press ‘enter’ to see it’s move)");
                        Console.ReadLine();
                        move = m_Game.MakeMove();
                        PrintBoard(m_Game.Board);
                        string CPUMove = formatMoveToString(move);
                        Console.WriteLine($"{currPlayer.Name}: {CPUMove[0]}{CPUMove[1]}>{CPUMove[3]}{CPUMove[4]}");
                    }
                    else
                    {
                        move = getMove();
                        if (move != null)
                        {
                            m_Game.MakeMove(move);
                            PrintBoard(m_Game.Board);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private static Player createPlayer(string i_Prompt, eMode i_Mode)
        {
            try
            {
                Player player;
                switch (i_Mode)
                {
                    case eMode.CPUPlayer:
                        player = new CPUPlayer();
                        break;

                    case eMode.AIPlayer:
                        player = new AIPlayer();
                        break;

                    case eMode.Player:
                        Console.WriteLine(i_Prompt);
                        string playerName = Console.ReadLine();
                        player = new Player(playerName);
                        break;
                    default:
                        Console.WriteLine("Invalid input. Please try again.");
                        return createPlayer(i_Prompt, i_Mode);
                }

                return player;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return createPlayer(i_Prompt, i_Mode);
            }
        }

        private static Board createBoard()
        {
            try
            {
                int boardSize;
                Console.WriteLine("Please enter board size (6,8,10): ");
                int.TryParse(Console.ReadLine(), out boardSize);
                return new Board(boardSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return createBoard();
            }
        }

        private static eMode getPlayMode()
        {
            Console.WriteLine("Select a play mode:");
            Console.WriteLine("0 - Player");
            Console.WriteLine("1 - AI Player");
            //Console.WriteLine("2 - CPU Player");

            string input = Console.ReadLine();

            switch (input)
            {
                case "0":
                    return eMode.Player;
                case "1":
                    return eMode.AIPlayer;
                case "2":
                    return eMode.CPUPlayer;
                default:
                    Console.WriteLine("Invalid input. Please try again.");
                    return getPlayMode();
            }
        }

        private void displayScore()
        {
            Console.WriteLine(string.Format("The Winner is {0} ({1}):",
                m_Game.Winner.Name,
                m_Game.Winner.Color == ePlayerColor.White ? 'X' : 'O'));
            Console.WriteLine($"{m_Game.Player1.Name} Points: {m_Game.Player1.Score}");
            Console.WriteLine($"{m_Game.Player2.Name} Points: {m_Game.Player2.Score}");
            Console.ReadLine();
        }

        private Move getMove()
        {
            Console.WriteLine(string.Format("{0} ({1}) Enter your move:",
                m_Game.CurrentPlayer.Name,
                m_Game.CurrentPlayer.Color == ePlayerColor.White ? 'X' : 'O'));
            string input = Console.ReadLine();
            if (input == "Q")
            {
                m_Game.Surrender();
                return null;
            }
            if (!validateFormat(input))
            {
                throw new Exception("Invalid format. Please enter in the following format ROWcol>Rowcol (Fa>Fb).");
            }
            string lowerInput = input.ToLower();
            string result = lowerInput.Replace(">", "");
            int[] RowColValues = new int[4];
            for (int i = 0; i < result.Length; i++)
            {
                RowColValues[i] = (int)(result[i] - 'a');
            }

            Move move = new Move(RowColValues[0], RowColValues[1], RowColValues[2], RowColValues[3]);
            return move;
        }

        private static bool validateFormat(string i_Move)
        {
            if (i_Move.Length != 5)
            {
                return false;
            }
            if (i_Move[2] != '>')
            {
                return false;
            }
            string result = i_Move.Replace(">", "");

            if (result.Length != 4)
            {
                return false;
            }

            for (int i = 0; i < result.Length; i++)
            {
                if(i % 2 == 0)
                {
                    if(!char.IsUpper(result[i]))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!char.IsLower(result[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private static string formatMoveToString(Move i_Move)
        {
            char srcRowChar = (char)('A' + i_Move.SrcRow);
            char srcColChar = (char)('a' + i_Move.SrcCol);
            char destRowChar = (char)('A' + i_Move.DestRow);
            char destColChar = (char)('a' + i_Move.DestCol);
            return $"{srcRowChar}{srcColChar}>{destRowChar}{destColChar}";
        }
        public static void PrintBoard(Board i_Board)
        {
            Ex02.ConsoleUtils.Screen.Clear();
            printColumns(i_Board);

            string repeatedString = string.Concat(Enumerable.Repeat("=", i_Board.Size * 4 + 2));
            Console.Write("\n" + repeatedString + "\n");

            for (int i = 0; i < i_Board.Size; i++)
            {
                Console.Write((char)('A' + i));
                for (int j = 0; j < i_Board.Size; j++)
                {
                    Piece currPiece = i_Board.Matrix[i, j];
                    Console.Write($"| {getPieceSymbol(currPiece)} ");
                }
                Console.Write("|\n" + repeatedString + "\n");
            }
        }

        private static void printColumns(Board i_Board)
        {
            Console.Write("   ");
            for (int i = 0; i < i_Board.Size; i++)
            {
                Console.Write((char)('a' + i) + "   ");
            }
        }

        private static char getPieceSymbol(Piece piece)
        {
            if (piece == null)
            {
                return ' ';
            }

            char symbol;
            switch (piece.Color)
            {
                case ePlayerColor.Black when !piece.IsKing:
                    symbol = 'O';
                    break;
                case ePlayerColor.White when !piece.IsKing:
                    symbol = 'X';
                    break;
                case ePlayerColor.Black when piece.IsKing:
                    symbol = 'U';
                    break;
                case ePlayerColor.White when piece.IsKing:
                    symbol = 'K';
                    break;
                default:
                    symbol = ' ';
                    break;
            }

            return symbol;
        }
    }
}