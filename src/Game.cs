using System.Net;

namespace TicTacToe
{
    public class Game
    {
        GameSocket socket;
        string[,] board;
        string pSymbol { get { return p1 ? "X" : "O"; } }
        bool p1;
        bool p1Turn;
        bool playerTurn { get { return p1 == p1Turn; } }
        public Game(string clientType)
        {
            if (clientType == "host")
            {
                socket = new GameSocket();
                socket.ListenForConnection();
                Random random = new Random();
                p1 = random.Next(2) == 1;
                if (p1)
                {
                    socket.Send("p1Host");
                }
                else
                {
                    socket.Send("p2Host");
                }
            }
            else
            {
                Console.Write("Enter host IP address: ");
                string? ip = Console.ReadLine();
                if (ip == null) throw new Exception("IP can't be null");

                IPAddress remoteHostIP = IPAddress.Parse(ip);
                socket = new GameSocket(remoteHostIP);

                p1 = socket.Receive() == "p2Host";
            }

            board = new string[3, 3];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    board[i, j] = ".";

            p1Turn = true;
        }
        int[] getMove()
        {
            string menuText = "Make a selection";
            int[] move;
            bool validMove;
            do
            {
                Menu menu = new Menu(menuText, board);
                menuText = "Invalid selection, try again";
                move = menu.GetSelection();
                validMove = board[move[0], move[1]] == ".";
            } while (!validMove);

            return move;
        }
        void printBoard()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.Write($"{board[i, j]} ");
                }

                Console.WriteLine();
            }
        }

        string[,] parseBoard(string boardStr)
        {
            string[,] board = new string[3, 3];

            for (int i = 0; i < 9; i++)
            {
                board[i / 3, i % 3] = boardStr[i].ToString();
            }

            return board;
        }

        string encodeBoard()
        {
            string boardStr = "";
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    boardStr += board[i, j];

            return boardStr;
        }

        string? checkWinner()
        {
            // Check rows
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] != "." && board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2])
                {
                    return board[i, 0];
                }
            }

            // Check columns
            for (int j = 0; j < 3; j++)
            {
                if (board[0, j] != "." && board[0, j] == board[1, j] && board[1, j] == board[2, j])
                {
                    return board[0, j];
                }
            }

            // Check diagonals
            if (board[0, 0] != "." && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
            {
                return board[0, 0];
            }

            if (board[0, 2] != "." && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
            {
                return board[0, 2];
            }

            return null;
        }

        bool checkDraw()
        {
            // Check if any empty spaces are left
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == ".")
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        int? checkState()
        {
            int? score = null;
            string? winner = checkWinner();
            if (winner != null)
                score = winner == pSymbol ? 1 : -1;
            else if (checkDraw())
                score = 0;

            return score;
        }


        public int Run()
        {
            while (true)
            {
                if (playerTurn)
                {
                    int[] move = getMove();
                    board[move[0], move[1]] = pSymbol;

                    string boardStr = encodeBoard();
                    socket.Send(boardStr);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Waiting for other player to make a move");
                    printBoard();
                    string boardStr = socket.Receive();
                    board = parseBoard(boardStr);
                }

                int? score = checkState();

                if (score != null)
                {
                    string finalText;
                    switch (score)
                    {
                        case 1:
                            finalText = "YOU WIN!";
                            break;
                        case -1:
                            finalText = "YOU LOSE!";
                            break;
                        default:
                            finalText = "DRAW!";
                            break;
                    }
                    Console.Clear();
                    printBoard();
                    Console.WriteLine(finalText);

                    return (int)score;
                }

                p1Turn = !p1Turn;
            }
        }
    }
}