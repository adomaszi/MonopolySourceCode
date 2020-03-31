
using MonopolyAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Monopoly
{
    public class Board
    {
        public DataCollector dataCollector;
        private static Card[] ChanceCards = new Card[] {
            new GetOutOfJailFreeCard(),
            new GoToJailCard()
        };
        public void AddPlayer(Player p) => players.Add(p);
        private Random rng = new Random();
        public int RollDice()
        {
            return RandomInt(1, 6);
        }

        public int RandomInt(int min, int max)
        {
            return rng.Next(min, max + 1);
        }

        public Card PickChanceCard()
        {
            return ChanceCards[rng.Next(ChanceCards.Length)];
        }
        public BoardSpace[] boardSpaces;
        internal List<Player> players;

        public IReadOnlyList<Player> Players => players;

        public int JailSpace = 10;

        public int CurrentPlayer = 0;
        public int Turn = 1;

        public int roundLimit;
        public int playerLimit;

        public IPlayerInteracter playerInteracter;

        public Board(IPlayerInteracter playerInteracter, int player, int round, DataCollector dataCollector )
        {
            this.dataCollector = dataCollector;
            roundLimit = round;
            playerLimit = player;
            this.playerInteracter = playerInteracter;
            playerInteracter.Board = this;
            players = new List<Player>();
            boardSpaces = new BoardSpace[]
            {
                null,
                // first row
                new Property(this, "Vine Street", PropertyFamily.Brown, 60m, 30m, 50m, new decimal[] { 2m,  10m, 30m, 90m, 160m, 250m}),
                new CommunityChestSpace(),
                new Property(this,  "Coventry Street", PropertyFamily.Brown, 60m, 30m, 50m, new decimal[] { 4m, 20m,  60m, 180m, 360m, 450m}),
                new TaxSpace("Income Tax", 200m),
                new StationSpace("Marylebone Station"),
                new Property(this, "Leicester Square", PropertyFamily.LightBlue, 100m, 50m, 50m, new decimal[]{ 6m, 30m, 90m, 270m, 400m, 550m }),
                new ChanceSpace(),

                new Property(this, "Bow Street", PropertyFamily.LightBlue, 100m, 50m, 50m, new decimal[]{ 6m, 30m, 90m, 270m, 400m, 550m }),

                new Property(this, "Whitechapel Road", PropertyFamily.LightBlue, 100m, 50m, 50m, new decimal[]{ 8m, 40m, 100m, 300m, 450m, 600m }),
               
                // second row
                new JailSpace(),
                new Property(this, "The Angel Islington", PropertyFamily.Pink, 140m, 100m, 70m, new decimal[]{ 10m, 50m, 150m, 450m, 625m, 700m }),
                new Utility("Electric Company"),
                new Property(this, "Trafalgar Square", PropertyFamily.Pink, 140m, 100m, 70m, new decimal[] {10m, 50m, 150m, 450m, 625m, 700m}),
                new Property(this, "Northumberland Avenue", PropertyFamily.Pink, 160m, 100m, 80m, new decimal[] { 12m, 60m, 180m, 500m, 700m, 900m }),
                new StationSpace("Fenchurech St Station"),
                new Property(this, "Marlborough Street", PropertyFamily.Orange, 180m, 100m, 90m, new decimal[] { 14m, 70m, 200m, 550m, 750m, 950m }),
                new CommunityChestSpace(),
                new Property(this, "Fleet Street", PropertyFamily.Orange, 180m, 100m, 90m, new decimal[] { 14m, 70m, 200m, 550m, 750m, 950m }),

                new Property(this, "Old Knet Road", PropertyFamily.Orange, 200m, 100m, 100m, new decimal[] { 16m, 80m, 220m, 600m, 800m, 1000m}),
                
                
                // third row
                new Property(this, "Whitehall", PropertyFamily.Red, 220m, 150m, 110, new decimal[] { 18m, 90m, 250m, 700m, 875m, 1050m }),
                new ChanceSpace(),
                new Property(this, "Pentonville Road", PropertyFamily.Red, 220m, 150m, 110, new decimal[] { 18m, 90m, 250m, 700m, 875m, 1050m }),
                new Property(this, "Pall Mall", PropertyFamily.Red, 240m, 150m, 120m, new decimal[] { 20m, 100m, 300m, 750m, 925m, 1100m }),
                new StationSpace("Kings Cross Station"),
                new Property(this, "Bond Street", PropertyFamily.Yellow, 150m, 150m, 130m, new decimal[] { 22m, 110m, 330m, 800m, 975m, 1150m }),
                new Property(this, "Strand", PropertyFamily.Yellow, 150m, 150m, 130m, new decimal[] { 22m, 110m, 330m, 800m, 975m, 1150m }),
                new Utility("Water Company"),
                new Property(this, "Regent Street", PropertyFamily.Yellow, 150m, 150m, 140m, new decimal[] { 24m, 120m, 360m, 850m, 1025m, 1200m }),

                // fourth row
                new GotoJailSpace(),
                new Property(this, "Euston Road", PropertyFamily.Green, 200m, 200m, 150m, new decimal[] { 26m, 130m, 390m, 900m, 1100m, 1275m }),
                new Property(this, "Picadilly", PropertyFamily.Green, 200m, 200m, 150m, new decimal[] { 26m, 130m, 390m, 900m, 1100m, 1275m }),
                new CommunityChestSpace(),
                new Property(this, "Oxford Street", PropertyFamily.Green, 200m, 200m, 160m, new decimal[] { 28m, 150m, 450m, 1000m, 1200m, 1400m }),
                new StationSpace("Liverpool Station"),
                new ChanceSpace(),
                new Property(this, "Park Lane", PropertyFamily.DarkBlue, 350m, 200m, 175m, new decimal[] { 35m, 175m, 500m, 1100m, 1300m, 1500m }),
                new TaxSpace("Super Tax", 400m),
                new Property(this, "Mayfair", PropertyFamily.DarkBlue, 400m, 200m, 200m, new decimal[] { 50m, 200m, 600m, 1400m, 1700m, 2000m }),
            };
        }

        public void Setup()
        {
            playerInteracter.Setup(this);
        }

        public int DoTurn()
        {
            if (Turn >= roundLimit)
            {
                return -1;
            }


            playerInteracter.BeforeTurn();
            foreach (var player in players)
                player.Roll();
            Turn++;
            return Turn;
        }
        public void PassedGo(Player player)
        {
            player.Gain(200m);
        }
    }

    public interface IPlayerInteracter
    {
        Board Board { get; set; }
        void Setup(Board board);
        void ShowPlayerLanded(Player player, BoardSpace space);
        void BeforeTurn();
        void PassedGo(Player player);
        void ShowPlayerPaidRent(Player player, Player owner, BoardSpace space, decimal amount);
        bool CheckPlayerBuy(Player player, BoardSpace property, decimal cost);
        bool CheckPlayerUseGetOutOfJailCard(Player player);
        bool CheckPlayerPayOutOfJail(Player player);
        void ShowTaxed(Player player, TaxSpace space);
        void ShowSentToJail(Player player);
        void ShowFreedFromJail(Player player, EscapeJailCause cause);
        void ShowStillInJail(Player player);
        void ShowPlayerRolled(Player player, int dice1, int dice2, bool doubles);
        void ShowPlayerMoney(Player player);
        void ShowPlayerPickedCard(Player player, Card card);
        void ShowPlayerAddedHouse(Player owner, Property prop, bool isHotel);
    }

    public class ConsolePlayerInteracter : IPlayerInteracter
    {
        public ConsolePlayerInteracter()
        {
        }

        public Board Board { get; set; }


        static T PromptMemberOf<T>(string prompt, IReadOnlyList<T> list)
        {
            Console.Write($"{prompt}: ");
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {list[i]}");
            }
            var index = Prompt<int>("Please select an index") - 1;
            return list[index];

        }
        static bool PromptBool(string promptMsg)
        {
            do
            {
                Console.Write($"{promptMsg} (y/n): ");
                var line = Console.ReadLine();
                if (line.StartsWith("y", StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
                if (line.StartsWith("n", StringComparison.CurrentCultureIgnoreCase))
                {
                    return false;
                }
                Console.WriteLine("Expected a boolean (y / n) input");
            } while (true);
        }
        static T Prompt<T>(string promptMsg)
        {
            bool wasValid = true;
            string line;
            T value = default(T);
            do
            {
                if (!wasValid)
                {
                    Console.WriteLine($"Expected {typeof(T)}");
                }
                wasValid = false;
                Console.Write($"{promptMsg}: ");
                line = Console.ReadLine();
                try
                {
                    value = (T)Convert.ChangeType(line, typeof(T));
                    wasValid = true;
                }
                catch (FormatException)
                {
                    wasValid = false;
                }
            } while (!wasValid);
            return value;
        }

        private Player PromptPlayer(int index)
        {
            var name = Prompt<string>($"Enter name for player {index}");
            var isAuto = PromptBool("CPU control this player?");
            return new Player(Board, name) { Auto = isAuto };
        }

        public void PassedGo(Player player)
        {
            Console.WriteLine($"{player} passed go and collected 200");
        }

        public void Setup(Board board)
        {
            Board = board;
            // var numPlayers = Prompt<int>("Enter number of players to create");
            for (int i = 0; i < board.playerLimit; i++)
            {
                // board.players.Add(PromptPlayer(i + 1));
                board.players.Add(new Player(Board, $"Player {i + 1}") { Auto = true });
            }
        }

        public void ShowPlayerLanded(Player player, BoardSpace space)
        {
            Console.WriteLine($"{player} moved by {player.LastSpacesMoved}: landed on {space.Name}!");
        }

        public void ShowPlayerPaidRent(Player player, Player owner, BoardSpace space, decimal amount)
        {
            Console.WriteLine($"{player} gave {space.Name} owner {owner} {amount:C}!");
        }

        public void ShowPlayerMoney(Player player)
        {
            Console.WriteLine($"{player} now has {player.Money:C}");
        }

        public bool CheckPlayerBuy(Player player, BoardSpace property, decimal cost)
        {
            if (player.Auto)
            {
                return cost < player.Money;
            }
            var shouldBuy = PromptBool($"Does {player} ({player.Money:C}) want to buy property {property.Name} {cost:C}? (true / false)\n");
            if (shouldBuy)
            {
                var props = player.OwnedProperties.Count == 0 ? "no" : string.Join(", ", player.OwnedProperties.Select(p => p.Name));
                Console.WriteLine($"{player} bought {property.Name}. Now has {player.Money - cost:C} and owns {props}");
            }
            return shouldBuy;
        }
        public void BeforeTurn()
        {
            Console.WriteLine($"Starting turn: {Board.Turn}");

            foreach (var player in Board.Players)
            {
                if (player.OwnedProperties.Count == 0)
                {
                    Console.WriteLine($"{player} does not own any properties.");
                    continue;
                }

                var props = string.Join(", ", player.OwnedProperties.Select(p => p.Name));

                Console.WriteLine($"{player} has properties: {props}");
                if (player.Auto)
                    continue;
                var doAction = PromptBool($"Does {player} want to do an action on any owned property?");

                if (doAction)
                {
                    var prop = PromptMemberOf("Pick a property to do an action on", player.OwnedProperties);
                    ShowActions(player, prop);
                }
            }
        }

        private void ShowActions(Player player, BoardSpace space)
        {
            if (space is Property prop)
            {
                var options = new List<string>();
                var action = PromptMemberOf($"Enter action for {prop.Name}", new List<string>
                {
                    "None",
                    "Buy a house"
                });
                if (action == "Buy a house")
                {
                    if (prop.CanAddHouse())
                    {
                        player.Charge(prop.HouseCost);
                        player.NumHouses++;
                        prop.NumHouses++;
                    }
                    else
                    {
                        Console.WriteLine("Can't add a house!");
                    }
                }
            }
        }

        public void ShowTaxed(Player player, TaxSpace space)
        {
            Console.WriteLine($"{player} charged by ${space.Tax:C} at ${space.Name}");
        }
        public void ShowSentToJail(Player player)
        {
            Console.WriteLine($"{player} sent to jail");
        }

        public void ShowFreedFromJail(Player player, EscapeJailCause cause)
        {
            switch (cause)
            {
                case EscapeJailCause.Doubles:
                    Console.WriteLine($"{player} rolled doubles to get out of jail");
                    break;
                case EscapeJailCause.GetOutOfJailCard:
                    Console.WriteLine($"{player} used a 'Get out of Jail free' card");
                    break;
                case EscapeJailCause.Paid:
                    Console.WriteLine($"{player} paid {50:C} to get out of jail");
                    break;
            }

        }

        public void ShowStillInJail(Player player)
        {
            Console.WriteLine($"{player} is still in jail!");
        }

        public void ShowPlayerRolled(Player player, int dice1, int dice2, bool doubles)
        {
            Console.WriteLine($"{player} rolled {dice1} and {dice2}. doubles: {doubles}");
        }

        public bool CheckPlayerPayOutOfJail(Player player)
        {
            return PromptBool($"Will {player} pay {50:C} to get out of jail?");
        }

        public bool CheckPlayerUseGetOutOfJailCard(Player player)
        {
            return PromptBool($"Will  {player} use their get out of jail free card?");
        }
        public void ShowPlayerPickedCard(Player player, Card card)
        {
            Console.WriteLine($"{player} picked card {card}");
        }

        public void ShowPlayerAddedHouse(Player owner, Property prop, bool isHotel)
        {
            Console.WriteLine($"{owner} added {(isHotel ? "hotel" : "house")} to {prop.Name}");
        }
    }
}
