using System;

namespace ChessBoard
{
    class Board
    {
        public int linhas { get; set; }
        public int colunas { get; set; }
        private Piace[,] pecas;

        public Board(int linhas, int colunas)
        {
            this.linhas = linhas;
            this.colunas = colunas;
            pecas = new Piace[linhas, colunas];
        }

        public Piace peca(int linha, int coluna)
        {
            return pecas[linha, coluna];
        }
    }
}
