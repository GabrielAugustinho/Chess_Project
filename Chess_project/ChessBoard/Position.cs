using System;

namespace ChessBoard
{
    class Position
    {
        public int linha { get; set; }
        public int coluna { get; set; }

        public Position(int line, int column)
        {
            this.linha = line;
            this.coluna = column;
        }

        public void definirValores(int linha, int coluna)
        {
            this.linha = linha;
            this.coluna = coluna;
        }

        public override string ToString()
        {
            return linha
                + ", "
                + coluna;
        }
    }
}
