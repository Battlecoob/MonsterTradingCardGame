using MonsterTradingCardGame.Models.Enums;
using System.Collections.Generic;

namespace MonsterTradingCardGame.Models
{

    public class Card
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public double Damage { get; set; }
        public CardType CardType => NameToType[Name];
        public Element Element => NameToElement[Name];
        public Species Species => NameToSpecies[Name];

        static Dictionary<string, Element> NameToElement = new Dictionary<string, Element>(){
            {"Ork", Element.normal},
            {"Dragon", Element.fire},
            {"Kraken", Element.water},
            {"FireElf", Element.fire},
            {"Knight", Element.normal},
            {"Wizzard", Element.normal},
            {"FireSpell", Element.fire},
            {"WaterSpell", Element.water},
            {"WaterGoblin", Element.water},
            {"RegularSpell", Element.normal}
        };

        static Dictionary<string, CardType> NameToType = new Dictionary<string, CardType>(){
            {"Ork", CardType.monster},
            {"Dragon", CardType.monster},
            {"Kraken", CardType.monster},
            {"Knight", CardType.monster},
            {"FireElf", CardType.monster},
            {"Wizzard", CardType.monster},
            {"FireSpell", CardType.spell},
            {"WaterSpell", CardType.spell},
            {"RegularSpell", CardType.spell},
            {"WaterGoblin", CardType.monster}
        };

        static Dictionary<string, Species> NameToSpecies = new Dictionary<string, Species>(){
            {"Ork", Species.ork},
            {"FireElf", Species.elf},
            {"Dragon", Species.dragon},
            {"Knight", Species.knight},
            {"Kraken", Species.kraken},
            {"FireSpell", Species.none},
            {"Wizzard", Species.wizzard},
            {"WaterSpell", Species.none},
            {"RegularSpell", Species.none},
            {"WaterGoblin", Species.goblin},
        };

        public override string ToString() 
        { 
            return $"Id: {Id}, Name: {Name}, Damage: {Damage}; "; 
        }

        public float CalculateElementMultiplicator(Element element)
        {
            float multiplier = 1;

            switch (this.Element)
            {
                case Element.normal:
                    multiplier = CompareElementEffectiveness(Element.water, Element.fire, element);
                    break;
                case Element.fire:
                    multiplier = CompareElementEffectiveness(Element.normal, Element.water, element);
                    break;
                case Element.water:
                    multiplier = CompareElementEffectiveness(Element.fire, Element.normal, element);
                    break;
            }

            return multiplier;
        }

        private float CompareElementEffectiveness(Element effective, Element ineffective, Element element)
        {
            if (element == effective)
                return 2f;
            else if (element == ineffective)
                return 0.5f;
            else
                return 1f;
        }
    }
}