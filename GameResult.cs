using Monopoly;
using MonopolyAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonolpolyAnalysis
{
    class GameResult
    {
        private List<Move> _moves;
        private Board _board;

        public GameResult(List<Move> moves, Board board)
        {
            _moves = moves;
            _board = board;
        }

        public List<Move> Moves { get => _moves; set => _moves = value; }
        public Board Board { get => _board; set => _board = value; }
    }
}
