using System;
using System.Collections.Generic;
using System.Text;

namespace Monopoly
{
    public abstract class Card
    {

        public string Name;
        public string Description;
        public abstract void DoCard(Player player);

        public Card(string name, string desc)
        {
            Name = name;
            Description = desc;
        }

        public override string ToString()
        {
            return $"{Name}: {Description}";
        }
    }

    public class GetOutOfJailFreeCard : Card
    {
        public GetOutOfJailFreeCard() : base("Get out of Jail Free", "Mr. Monopoly, in close-fitting one-piece prison stripes, is literally kicked out")
        {

        }
        public override void DoCard(Player player) => player.NumGetOutOfJailFreeCards++;
    }

    public class GoToJailCard : Card
    {
        public GoToJailCard() : base("Go to Jail. Go directly to Jail", "A truncheon-carrying policeman in a dark-colored uniform hauls a surprised Mr Monopoly along by the feet")
        {

        }
        public override void DoCard(Player player)
        {
            player.SendToJail();
        }
    }
}
