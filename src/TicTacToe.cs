//using System.Net;
namespace TicTacToe
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string title = "TicTacToe";
            string[] options = new string[]
            {
                "Host a new game",
                "Join existing game"
            };

            Menu menu = new Menu(title, options);
            int[] selection = menu.GetSelection();

            string clientType;
            if (selection[1] == 0)
            {
                clientType = "host";
            }
            else
            {
                clientType = "peer";
            }

            Game game = new Game(clientType);
            bool winner = game.Run();

            Console.WriteLine(winner ? "CONGRATS" : "SUCK IT NOOB");
        }
    }


}