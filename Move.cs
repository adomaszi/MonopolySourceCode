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
        private BuyableSpace _property { get; }
        private decimal _moneyPaid { get; set; }
        private int _numberRolled { get; }

        public Move(Player player, BuyableSpace property, decimal moneyPaid, int numberRolled)
        {
            _player = player;
            _property = property;
            _moneyPaid = moneyPaid;
            _numberRolled = numberRolled;
        }
    }
}
