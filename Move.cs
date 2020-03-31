using Monopoly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyAnalysis
{
    public class Move : EventArgs
    {
        public Player _player { get; }
        public BoardSpace _property { get; }
        public decimal _moneyPaid { get; set; }
        public int _numberRolled { get; }

        public Move(Player player, BoardSpace property, decimal moneyPaid, int numberRolled)
        {
            _player = player;
            _property = property;
            _moneyPaid = moneyPaid;
            _numberRolled = numberRolled;
        }

        public Move(Player player, int numberRolled)
        {
            _player = player;
            _numberRolled = numberRolled;
        }
    }
}
