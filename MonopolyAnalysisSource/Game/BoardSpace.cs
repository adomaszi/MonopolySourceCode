using MonopolyAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monopoly
{
    public class BoardSpace
    {

        public string Name { get; private set; }

        public BoardSpace(string name)
        {
            Name = name;
        }

        public virtual void OnPlayerLanded(Player player, int numberRolled)
        {
            player.Interacter.ShowPlayerLanded(player, this);
        }
    }

    public class TaxSpace : BoardSpace
    {
        public decimal Tax;

        public TaxSpace(string name, decimal tax) : base(name)
        {
            Tax = tax;
        }

        public override void OnPlayerLanded(Player player, int numberRolled)
        {
            base.OnPlayerLanded(player, numberRolled);
            player.Charge(Tax);
            player.Interacter.ShowTaxed(player, this);
        }
    }

    public class GotoJailSpace : BoardSpace
    {
        public GotoJailSpace() : base("Go to Jail")
        { }
        public override void OnPlayerLanded(Player player, int numberRolled)
        {
            base.OnPlayerLanded(player, numberRolled);
            player.SendToJail();
        }
    }
    public class JailSpace : BoardSpace
    {
        public JailSpace() : base("Jail")
        { }
    }



    public abstract class BuyableSpace : BoardSpace
    {

        public Player Owner = null;
        abstract public decimal Cost { get; }
        abstract public decimal MortgageValue { get; }
        abstract public decimal CalculateRent(Player player);

        public BuyableSpace(string name) : base(name)
        {

        }

        public override void OnPlayerLanded(Player player, int numberRolled)
        {
            base.OnPlayerLanded(player, numberRolled);
            if (player != Owner && Owner != null && !Owner.InJail)
            {
                var rent = CalculateRent(player);
                player.Charge(rent);
                player.Board.dataCollector.registerMove(new Move(player, this, rent, numberRolled));
                Owner.Gain(rent);
                player.Interacter.ShowPlayerPaidRent(player, Owner, this, rent);
            }
            else if (Owner == null)
            {
                if (player.Interacter.CheckPlayerBuy(player, this, Cost))
                {
                    Owner = player;
                    player.AddProperty(this);
                }
            }
        }
    }

    public class Utility : BuyableSpace
    {

        public override decimal Cost => 150m;

        public override decimal MortgageValue => 75m;

        public Utility(string name) : base(name)
        {
        }

        public override decimal CalculateRent(Player player) =>
            4m * player.LastSpacesMoved;
    }

    public class StationSpace : BuyableSpace
    {
        public StationSpace(string name) : base(name)
        {
        }

        public override decimal Cost => 200m;

        public override decimal MortgageValue => 100m;

        public override decimal CalculateRent(Player player)
        {
            if (Owner == null)
                return 0m;
            switch (Owner.OwnedStations)
            {
                case 1: return 25m;
                case 2: return 50m;
                case 3: return 100m;
                default: return 200m;
            }
        }

    }

    public enum PropertyFamily
    {
        Brown,
        LightBlue,
        DarkBlue,
        Pink,
        Orange,
        Red
    }

    public class CommunityChestSpace : BoardSpace
    {
        public CommunityChestSpace() : base("Community Chest") { }
    }

    public class ChanceSpace : BoardSpace
    {
        public ChanceSpace() : base("Chance") { }
        public override void OnPlayerLanded(Player player, int numberRolled)
        {
            base.OnPlayerLanded(player, numberRolled);

            var card = player.Board.PickChanceCard();
            player.Interacter.ShowPlayerPickedCard(player, card);
            card.DoCard(player);
        }
    }

    public class Property : BuyableSpace
    {
        public Board Board;
        public decimal[] RentValues;
        public int NumHouses = 0;
        public decimal HouseCost { get; private set; }
        public override decimal Cost => cost;
        private readonly decimal cost;
        public override decimal MortgageValue => mortgageValue;
        private readonly decimal mortgageValue;
        public PropertyFamily Family { get; private set; }
        internal IPlayerInteracter Interacter => Board.playerInteracter;

        public Property(Board board, string name, PropertyFamily family, decimal cost, decimal houseCost, decimal mortgageValue, decimal[] rentValues) : base(name)
        {
            if (rentValues.Length != 6)
            {
                throw new Exception($"{name} rent values must be in array of: [rent, with 1 house, with 2 houses, with 3 houses, with 4 houses, with hotel]");
            }
            Board = board;
            RentValues = rentValues;
            Family = family;
            this.cost = cost;
            HouseCost = houseCost;
            this.mortgageValue = mortgageValue;
        }

        public void AddHouse()
        {
            if (!CanAddHouse())
                return;
            NumHouses++;
            Interacter.ShowPlayerAddedHouse(Owner, this, NumHouses == RentValues.Last());
        }

        public bool CanAddHouse()
            => (Owner.Money >= HouseCost) && (NumHouses + 1 < RentValues.Length) && (Owner.HasAll(Family));

        public override string ToString() => Name;

        public override decimal CalculateRent(Player player)
            => RentValues[NumHouses];
    }
}
