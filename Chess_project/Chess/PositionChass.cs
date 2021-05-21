using ChessBoard;

namespace Chess
{
    class PositionChass
    {
        public char coluna { get; set; }
        public int linha { get; set; }

        public PositionChass(char coluna, int linha)
        {
            this.coluna = coluna;
            this.linha = linha;
        }

        public Position toPosicao()
        {
            return new Position(8 - linha, coluna - 'a');
        }

        public override string ToString()
        {
            return "" + coluna + linha;
        }
    }
}
