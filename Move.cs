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
        private Player _player { get; }
        private BoardSpace _property { get; }
        private decimal _moneyPaid { get; set; }
        private int _numberRolled { get; }

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
