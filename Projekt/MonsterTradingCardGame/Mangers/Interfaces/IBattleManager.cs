using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Mangers
{
    public interface IBattleManager
    {
        BattleLog StartBattle();
        List<Card> CardSetup(Deck deck);
        double CalculateElementMultiplicator(Element element);
        MonsterSpecialities CalculateSpeciality(Card card1, Card card2);
    }
}
