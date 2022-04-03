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
        //double CalculateElementMultiplicator(Element element);
        void CardFight(Player player, Player enemy, Round round);
        Specialities CalculateSpeciality(Card card1, Card card2);
        void EnemyWinsRound(Player player, Player enemy, Round round);
        void PlayerWinsRound(Player player, Player enemy, Round round);
        void ChangeCards(List<Card> winner, List<Card> loser, Card loserCard);
        void SetGameEnding(Player winner, Player loser, bool draw, Round round);
    }
}
