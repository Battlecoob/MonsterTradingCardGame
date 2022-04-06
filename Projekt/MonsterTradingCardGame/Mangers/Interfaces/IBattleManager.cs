using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Managers
{
    public interface IBattleManager
    {
        BattleLog StartBattle();
        List<Card> CardSetup(Deck deck);
        //double CalculateElementMultiplicator(Element element);
        void CardFight(Player player, Player enemy, ref Round round);
        Specialities CalculateSpeciality(Card card1, Card card2);
        void EnemyWinsRound(Player player, Player enemy, ref Round round);
        void PlayerWinsRound(Player player, Player enemy, ref Round round);
        void ChangeCards(ref List<Card> winner, ref List<Card> loser, Card loserCard);
        void SetRoundEnding(Player winner, Player loser, bool draw, ref Round round);
    }
}
