using System.Collections.Generic;
using ChessBoard;
using ChessBoard.Enums;
using ChessBoard.Exceptions;

namespace Chess
{
    class MatchChess
    {
        public Board tab { get; private set; }
        public int turno { get; private set; }
        public Color jogadorAtual { get; private set; }
        public bool terminada { get; private set; }
        public bool xeque { get; private set; }
        public Piace vulneravelEnPassant { get; private set; }

        private HashSet<Piace> pecas;
        private HashSet<Piace> capturadas;

        public MatchChess()
        {
            tab = new Board(8, 8);
            turno = 1;
            jogadorAtual = Color.Branca;
            terminada = false;
            xeque = false;
            vulneravelEnPassant = null;
            pecas = new HashSet<Piace>();
            capturadas = new HashSet<Piace>();
            colocarPecas();
        }

        public Piace executaMovimento(Position origem, Position destino)
        {
            Piace p = tab.retirarPeca(origem);

            p.incrementarQteMovimentos();
            Piace pecaCapturada = tab.retirarPeca(destino);
            tab.colocarPeca(p, destino);
            if (pecaCapturada != null)
            {
                capturadas.Add(pecaCapturada);
            }

            // #jogadaespecial roque pequeno
            if (p is King && destino.coluna == origem.coluna + 2)
            {
                Position origemT = new Position(origem.linha, origem.coluna + 3);
                Position destinoT = new Position(origem.linha, origem.coluna + 1);
                Piace T = tab.retirarPeca(origemT);
                T.incrementarQteMovimentos();
                tab.colocarPeca(T, destinoT);
            }

            // #jogadaespecial roque grande
            if (p is King && destino.coluna == origem.coluna - 2)
            {
                Position origemT = new Position(origem.linha, origem.coluna - 4);
                Position destinoT = new Position(origem.linha, origem.coluna - 1);
                Piace T = tab.retirarPeca(origemT);
                T.incrementarQteMovimentos();
                tab.colocarPeca(T, destinoT);
            }

            // #jogadaespecial en Passant
            if (p is Peon)
            {
                if (origem.coluna != destino.coluna && pecaCapturada == null)
                {
                    Position posP;
                    if (p.cor == Color.Branca)
                    {
                        posP = new Position(destino.linha + 1, destino.coluna);
                    }
                    else
                    {
                        posP = new Position(destino.linha - 1, destino.coluna);
                    }
                    pecaCapturada = tab.retirarPeca(posP);
                    capturadas.Add(pecaCapturada);
                }
            }

            return pecaCapturada;
        }

        public void desfazMovimento(Position origem, Position destino, Piace pecaCapturada)
        {
            Piace p = tab.retirarPeca(destino);

            p.decrementarQteMovimentos();
            if (pecaCapturada != null)
            {
                tab.colocarPeca(pecaCapturada, destino);
                capturadas.Remove(pecaCapturada);
            }
            tab.colocarPeca(p, origem);

            // #jogadaespecial roque pequeno
            if (p is King && destino.coluna == origem.coluna + 2)
            {
                Position origemT = new Position(origem.linha, origem.coluna + 3);
                Position destinoT = new Position(origem.linha, origem.coluna + 1);
                Piace T = tab.retirarPeca(destinoT);
                T.decrementarQteMovimentos();
                tab.colocarPeca(T, origemT);
            }

            // #jogadaespecial roque grande
            if (p is King && destino.coluna == origem.coluna - 2)
            {
                Position origemT = new Position(origem.linha, origem.coluna - 4);
                Position destinoT = new Position(origem.linha, origem.coluna - 1);
                Piace T = tab.retirarPeca(destinoT);
                T.incrementarQteMovimentos();
                tab.colocarPeca(T, origemT);
            }

            // #jogadaespecial en Passant
            if (p is Peon)
            {
                if (origem.coluna != destino.coluna && pecaCapturada == vulneravelEnPassant)
                {
                    Piace peao = tab.retirarPeca(destino);
                    Position posP;
                    if (p.cor == Color.Branca)
                    {
                        posP = new Position(3, destino.coluna);
                    }
                    else
                    {
                        posP = new Position(4, destino.coluna);
                    }
                    tab.colocarPeca(peao, posP);
                }
            }
        }

        public void realizaJogada(Position origem, Position destino)
        {
            Piace pecaCapturada = executaMovimento(origem, destino);

            if (estaEmXeque(jogadorAtual))
            {
                desfazMovimento(origem, destino, pecaCapturada);
                throw new BoardExceptions("Você não pode se colocar em xeque!");
            }

            Piace p = tab.peca(destino);

            // #jogadaespecial promocao
            if (p is Peon)
            {
                if ((p.cor == Color.Branca && destino.linha == 0) || (p.cor == Color.Preta && destino.linha == 7))
                {
                    p = tab.retirarPeca(destino);
                    pecas.Remove(p);
                    Piace dama = new Queen(tab, p.cor);
                    tab.colocarPeca(dama, destino);
                    pecas.Add(dama);
                }
            }

            if (estaEmXeque(adversaria(jogadorAtual)))
            {
                xeque = true;
            }
            else
            {
                xeque = false;
            }

            if (testeXequemate(adversaria(jogadorAtual)))
            {
                terminada = true;
            }
            else
            {
                turno++;
                mudaJogador();
            }

            // #jogadaespecial en passant
            if (p is Peon && (destino.linha == origem.linha - 2) || destino.linha == origem.linha + 2)
            {
                vulneravelEnPassant = p;
            }
            else
            {
                vulneravelEnPassant = null;
            }
        }

        public void validarPosicaoDeDestino(Position origem, Position destino)
        {
            if (!tab.peca(origem).MovimentoPossivel
                (destino))
            {
                throw new BoardExceptions("Posição de destino inválida");
            }
        }

        public void validarPosicaoDeOrigem(Position pos)
        {
            if (tab.peca(pos) == null)
            {
                throw new BoardExceptions("Não existe peça na posição de origem escolhida!");
            }
            if (jogadorAtual != tab.peca(pos).cor)
            {
                throw new BoardExceptions("A peça escolhida não é sua!");
            }
            if (!tab.peca(pos).existeMovimentosPossiveis())
            {
                throw new BoardExceptions("Não há movimentos possíveis para a peça de origem escolhida!");
            }
        }

        private void mudaJogador()
        {
            if (jogadorAtual == Color.Branca)
            {
                jogadorAtual = Color.Preta;
            }
            else
            {
                jogadorAtual = Color.Branca;
            }
        }

        public HashSet<Piace> pecasCapturadas(Color cor)
        {
            HashSet<Piace> aux = new HashSet<Piace>();
            foreach (Piace x in capturadas)
            {
                if (x.cor == cor)
                {
                    aux.Add(x);
                }
            }
            return aux;
        }

        public HashSet<Piace> pecasEmJogo(Color cor)
        {
            HashSet<Piace> aux = new HashSet<Piace>();
            foreach (Piace x in pecas)
            {
                if (x.cor == cor)
                {
                    aux.Add(x);
                }
            }
            aux.ExceptWith(pecasCapturadas(cor));
            return aux;
        }

        private Color adversaria(Color cor)
        {
            if (cor == Color.Branca)
            {
                return Color.Preta;
            }
            else
            {
                return Color.Branca;
            }
        }

        private Piace rei(Color cor)
        {
            foreach (Piace x in pecasEmJogo(cor))
            {
                if (x is King)
                {
                    return x;
                }
            }
            return null;
        }

        public bool estaEmXeque(Color cor)
        {
            Piace R = rei(cor);
            if (R == null)
            {
                throw new BoardExceptions("Não tem rei da cor " + cor + " no tabuleiro");
            }

            foreach (Piace x in pecasEmJogo(adversaria(cor)))
            {
                bool[,] mat = x.movimentosPossiveis();
                if (mat[R.posicao.linha, R.posicao.coluna])
                {
                    return true;
                }
            }
            return false;
        }

        public bool testeXequemate(Color cor)
        {
            if (!estaEmXeque(cor))
            {
                return false;
            }
            foreach (Piace x in pecasEmJogo(cor))
            {
                bool[,] mat = x.movimentosPossiveis();
                for (int i = 0; i < tab.linhas; i++)
                {
                    for (int j = 0; j < tab.colunas; j++)
                    {
                        if (mat[i, j])
                        {
                            Position origem = x.posicao;
                            Position destino = new Position(i, j);
                            Piace pecaCapturada = executaMovimento(origem, destino);
                            bool testeXeque = estaEmXeque(cor);
                            desfazMovimento(origem, destino, pecaCapturada);
                            if (!testeXeque)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public void colocarNovaPeca(char coluna, int linha, Piace peca)
        {
            tab.colocarPeca(peca, new PositionChass(coluna, linha).toPosicao());
            pecas.Add(peca);
        }

        private void colocarPecas()
        {
            colocarNovaPeca('a', 1, new Tower(tab, Color.Branca));
            colocarNovaPeca('b', 1, new Hourse(tab, Color.Branca));
            colocarNovaPeca('c', 1, new Bishop(tab, Color.Branca));
            colocarNovaPeca('d', 1, new Queen(tab, Color.Branca));
            colocarNovaPeca('e', 1, new King(tab, Color.Branca, this));
            colocarNovaPeca('f', 1, new Bishop(tab, Color.Branca));
            colocarNovaPeca('g', 1, new Hourse(tab, Color.Branca));
            colocarNovaPeca('h', 1, new Peon(tab, Color.Branca, this));
            colocarNovaPeca('a', 2, new Peon(tab, Color.Branca, this));
            colocarNovaPeca('b', 2, new Peon(tab, Color.Branca, this));
            colocarNovaPeca('c', 2, new Peon(tab, Color.Branca, this));
            colocarNovaPeca('d', 2, new Peon(tab, Color.Branca, this));
            colocarNovaPeca('e', 2, new Peon(tab, Color.Branca, this));
            colocarNovaPeca('f', 2, new Peon(tab, Color.Branca, this));
            colocarNovaPeca('g', 2, new Peon(tab, Color.Branca, this));
            colocarNovaPeca('h', 2, new Peon(tab, Color.Branca, this));

            colocarNovaPeca('a', 8, new Tower(tab, Color.Preta));
            colocarNovaPeca('b', 8, new Hourse(tab, Color.Preta));
            colocarNovaPeca('c', 8, new Bishop(tab, Color.Preta));
            colocarNovaPeca('d', 8, new Queen(tab, Color.Preta));
            colocarNovaPeca('e', 8, new King(tab, Color.Preta, this));
            colocarNovaPeca('f', 8, new Bishop(tab, Color.Preta));
            colocarNovaPeca('g', 8, new Hourse(tab, Color.Preta));
            colocarNovaPeca('h', 8, new Peon(tab, Color.Preta, this));
            colocarNovaPeca('a', 7, new Peon(tab, Color.Preta, this));
            colocarNovaPeca('b', 7, new Peon(tab, Color.Preta, this));
            colocarNovaPeca('c', 7, new Peon(tab, Color.Preta, this));
            colocarNovaPeca('d', 7, new Peon(tab, Color.Preta, this));
            colocarNovaPeca('e', 7, new Peon(tab, Color.Preta, this));
            colocarNovaPeca('f', 7, new Peon(tab, Color.Preta, this));
            colocarNovaPeca('g', 7, new Peon(tab, Color.Preta, this));
            colocarNovaPeca('h', 7, new Peon(tab, Color.Preta, this));
        }
    }
}
