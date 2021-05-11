using System;
using ChessBoard.Enums;

namespace ChessBoard
{
    class Piace
    {
        public Position posicao { get; set; }
        public Color cor { get; protected set; }
        public int QteMovimento { get; protected set; }
        public Board tab { get; set; }
    }
}
