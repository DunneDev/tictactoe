namespace TicTacToe
{
    public class Menu
    {
        string title;
        string[,] options;
        int[] selectionIndex;
        int colLength { get { return options.GetLength(0); } }
        int rowLength { get { return options.GetLength(1); } }
        public Menu(string title)
        {
            this.title = title;
            this.options = new string[,] { };

            selectionIndex = new int[] { 0, 0 };
        }

        public Menu(string title, string[,] options) : this(title)
        {
            this.options = options;
        }

        public Menu(string title, string[] options) : this(title)
        {
            this.options = new string[1, options.Length];

            for (int i = 0; i < options.Length; i++)
            {
                this.options[0, i] = options[i];
            }
        }


        void printMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(title);

            for (int i = 0; i < colLength; i++)
            {
                for (int j = 0; j < rowLength; j++)
                {
                    if (i == selectionIndex[0] && j == selectionIndex[1])
                        Console.ForegroundColor = ConsoleColor.Red;
                    else
                        Console.ForegroundColor = ConsoleColor.White;

                    Console.Write($"{options[i, j]} ");
                }

                Console.WriteLine();
            }
        }


        bool getInput()
        {
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.UpArrow)
            {
                if (selectionIndex[0] > 0)
                    selectionIndex[0]--;
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                if (selectionIndex[0] < colLength - 1)
                    selectionIndex[0]++;
            }
            else if (key.Key == ConsoleKey.LeftArrow)
            {
                if (selectionIndex[1] > 0)
                    selectionIndex[1]--;
            }
            else if (key.Key == ConsoleKey.RightArrow)
            {
                if (selectionIndex[1] < rowLength - 1)
                    selectionIndex[1]++;
            }
            else if (key.Key == ConsoleKey.Enter)
            {
                Console.ForegroundColor = ConsoleColor.White;
                return true;
            }

            return false;
        }

        public int[] GetSelection()
        {
            while (true)
            {
                printMenu();
                if (getInput())
                {
                    return selectionIndex;
                }
            }
        }
    }
}