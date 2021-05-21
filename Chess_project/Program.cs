using System;
using Chess;
using ChessBoard;
using ChessBoard.Enums;
using ChessBoard.Exceptions;

namespace Chess_project
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                MatchChess partida = new MatchChess();

                while (!partida.terminada)
                {
                    try
                    {
                        Console.Clear();
                        Screem.imprimirPartida(partida);

                        Console.WriteLine();
                        Console.Write("Origem: ");
                        Position origem = Screem.lerPosicaoXadrez().toPosicao();
                        partida.validarPosicaoDeOrigem(origem);

                        bool[,] posicoesPossiveis = partida.tab.peca(origem).movimentosPossiveis();

                        Console.Clear();
                        Screem.imprimirTabuleiro(partida.tab, posicoesPossiveis);

                        Console.WriteLine();
                        Console.Write("Destino: ");
                        Position destino = Screem.lerPosicaoXadrez().toPosicao();
                        partida.validarPosicaoDeDestino(origem, destino);

                        partida.realizaJogada(origem, destino);
                    }
                    catch (BoardExceptions e)
                    {
                        Console.WriteLine(e.Message);
                        Console.ReadLine();
                    }
                }

                Screem.imprimirTabuleiro(partida.tab);
            }
            catch (BoardExceptions e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}
