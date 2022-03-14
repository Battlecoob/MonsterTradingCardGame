using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Mangers
{
    public class BattleManager : IBattleManager
    {
        private BattleLog _log;
        private string _enemyName;
        private string _playerName;
        private readonly IRepoManager _repoManager;
        private List<Card> _deckEnemy = new List<Card>();
        private List<Card> _deckPlayer = new List<Card>();

        public BattleManager(IRepoManager repoManager, Deck deckPlayer, string playerName, Deck deckEnemy, string enemyName)
        {
            _enemyName = enemyName;
            _playerName = playerName;
            _repoManager = repoManager;
            _deckEnemy = CardSetup(deckEnemy);
            _deckPlayer = CardSetup(deckPlayer);

            _log = new BattleLog
            {
                Decks = new List<Deck>(),
                Rounds = new List<Round>()
            };

            _log.Decks.Add(deckEnemy);
            _log.Decks.Add(deckPlayer);
        }

        public BattleLog StartBattle()
        {
            throw new NotImplementedException();
        }

        public List<Card> CardSetup(Deck deck)
        {
            var cards = new List<Card>();

            foreach (string cardId in deck.CardIds)
                cards.Add(_repoManager.GetCardByIdToken(cardId, deck.Token));

            return cards;
        }

        public MonsterSpecialities CalculateSpeciality(Card card1, Card card2)
        {
            switch (card1.Species)
            {
                case Species.elf: // can evade dragons
                    if (card1.Element == Element.fire && card2.Species == Species.dragon)
                        return MonsterSpecialities.friend;
                    break;

                case Species.ork: // controlled by wizzards
                    if (card2.Species == Species.wizzard)
                        return MonsterSpecialities.controlled;
                    break;

                //case Species.dragon:
                //    break;

                case Species.goblin: // afraid of dragons
                    if (card2.Species == Species.dragon)
                        return MonsterSpecialities.afraid;
                    break;

                case Species.knight: // drown
                    if(card2.CardType == CardType.spell && card2.Element == Element.water)
                        return MonsterSpecialities.drowned;
                    break;

                case Species.kraken: // immune against spells
                    if (card2.CardType == CardType.spell)
                        return MonsterSpecialities.immune;
                    break;
                //case Species.wizzard:
                //    break;
            }

            return MonsterSpecialities.none;
        }

        public double CalculateElementMultiplicator(Element element)
        {
            double multiplier = 1;

            switch(element)
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

        private double CompareElementEffectiveness(Element effective, Element ineffective, Element element)
        {
            if (element == effective) 
                return 2;
            else if (element == ineffective) 
                return 0.5;
            else 
                return 1;
        }
    }
}
