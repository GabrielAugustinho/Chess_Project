using System;
using ChessBoard.Exceptions;

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

        public void colocarPeca(Piace p, Position pos)
        {
            if (existePeca(pos))
            {
                throw new BoardExceptions("Já existe uma peça nesta posição");
            }
            pecas[pos.linha, pos.coluna] = p;
            p.posicao = pos;
        }

        public Piace retirarPeca(Position pos)
        {
            if (peca(pos) == null)
            {
                return null;
            }
            Piace aux = peca(pos);
            aux.posicao = null;
            pecas[pos.linha, pos.coluna] = null;
            return aux;
        }

        public Piace peca(int linha, int coluna)
        {
            return pecas[linha, coluna];
        }

        public Piace peca(Position pos)
        {
            return pecas[pos.linha, pos.coluna];
        }

        public bool existePeca(Position pos)
        {
            validarPosicao(pos); // Varifica se existe a posição informada.
            return peca(pos) != null; // SeVerifica na matriz se existe uma peça na posição informada.
        }

        public bool posicaoValida(Position pos)
        {
            if (pos.linha < 0 || pos.linha >= linhas || pos.coluna < 0 || pos.coluna >= colunas) // Varifica se a posição é válida
            {
                return false;
            }
            return true;
        }

        public void validarPosicao(Position pos)
        {
            if (!posicaoValida(pos)) // Se a posição informada for inválida ele avisa o usuário
            {
                throw new BoardExceptions("Posição inválida");
            }
        }
    }
}
