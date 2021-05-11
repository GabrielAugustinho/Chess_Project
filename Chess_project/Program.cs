using System;
using ChessBoard;

namespace Chess_project
{
    class Program
    {
        static void Main(string[] args)
        {
            Board tab = new Board(8, 8);

            Screem.imprimirTabuleiro(tab);

            Console.ReadLine();
        }
    }
}
