using System;
using ChessBoard.Enums;

namespace ChessBoard
{
    class Piace
    {
        public Position posicao { get; set; }
        public Color cor { get; protected set; }
        public int QteMovimento { get; protected set; }
        public Board tab { get; protected set; }

        public Piace(Position posicao, Color cor, Board tab)
        {
            this.posicao = posicao;
            this.cor = cor;
            this.tab = tab;
            QteMovimento = 0;
        }
    }
}
