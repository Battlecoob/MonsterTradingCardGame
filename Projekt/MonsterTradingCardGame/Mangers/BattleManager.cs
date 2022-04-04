using MonsterTradingCardGame.Exceptions;
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
            int roundCount = 0;
            var randomNr = new Random();

            for (; roundCount < 100; roundCount++)
            {
                if (_deckPlayer.Count <= 0 || _deckEnemy.Count <= 0)
                    break;

                // init round
                var round = new Round(roundCount + 1);

                // init player 1
                var tmpCard = _deckEnemy[randomNr.Next() % _deckEnemy.Count];
                var enemy = new Player(_enemyName, tmpCard);
                enemy.CardsLeft = _deckEnemy.Count;

                // init player 2
                tmpCard = _deckPlayer[randomNr.Next() % _deckPlayer.Count];
                var player = new Player(_playerName, tmpCard);
                player.CardsLeft = _deckPlayer.Count;

                if (player.Card.CardType == CardType.spell)
                {
                    // set element multiplier
                    player.ElementMultiplier = player.Card.CalculateElementMultiplicator(enemy.Card.Element);
                    // set total dmg
                    player.Damage = player.Card.Damage * player.ElementMultiplier;
                }
                else // monster
                {
                    // set default multiplier
                    player.ElementMultiplier = 1;
                    // set total dmg
                    player.Damage = player.Card.Damage;
                }

                if(enemy.Card.CardType == CardType.spell)
                {
                    // set element multiplier
                    enemy.ElementMultiplier = enemy.Card.CalculateElementMultiplicator(player.Card.Element);
                    // set total dmg
                    enemy.Damage = enemy.Card.Damage * enemy.ElementMultiplier;
                }
                else // monster
                {
                    // set default multiplier
                    enemy.ElementMultiplier = 1;
                    // set total dmg
                    enemy.Damage = enemy.Card.Damage;
                }

                // testen ob reihenfolge der karten param korrekt ist
                enemy.Speciality = CalculateSpeciality(player.Card, enemy.Card);
                player.Speciality = CalculateSpeciality(enemy.Card, player.Card);

                CardFight(player, enemy, round);

                // update both player's card data
                enemy.CardsLeft = _deckEnemy.Count;
                player.CardsLeft = _deckPlayer.Count;

                // add both players to the round log
                round.Players.Add(enemy);
                round.Players.Add(player);

                // log round
                _log.Rounds.Add(round);
            }

            _log.RoundCount = roundCount;

            if (_deckPlayer.Count <= 0)
            {
                _log.Winner = _playerName;
                _log.Loser = _enemyName;
                _log.Draw = false;

                // repomanager -> update stats
                _repoManager.UpdateLoserStat(_log.Decks[0].Token);
                _repoManager.UpdateWinnerStat(_log.Decks[1].Token);
            }
            else if (_deckEnemy.Count <= 0)
            {
                _log.Winner = _enemyName;
                _log.Loser = _playerName;
                _log.Draw = false;

                _repoManager.UpdateLoserStat(_log.Decks[1].Token);
                _repoManager.UpdateWinnerStat(_log.Decks[0].Token);
            }
            else
            {
                _log.Winner = "none";
                _log.Loser = "none";
                _log.Draw = true;

                _repoManager.UpdateDrawStat(_log.Decks[0].Token);
                _repoManager.UpdateDrawStat(_log.Decks[1].Token);
            }

            return _log;
        }

        public void CardFight(Player player, Player enemy, Round round)
        {
            if( player.Damage == -1 ||
                player.CardsLeft == -1 ||
                player.ElementMultiplier == -1 ||
                enemy.Damage == -1 ||
                enemy.CardsLeft == -1 ||
                enemy.ElementMultiplier == -1)
            {
                // player / enemy isn't set correctly in StartBattle() - no default val should be still set
                throw new InternalPlayerErrorExcpt();
            }

            /*      SPECIALITIES
             * Goblin vs Dragon, afraid         -> Dragon
             * Dragon vs Goblin, none
             * Wizzard vs Orks, none            -> Wizzard
             * Orks vs Wizzard, controlled
             * Knights vs WaterSpell, drowned   -> Waterspell
             * WaterSpell vs Knight, none
             * Kraken vs Spell, immune          -> Kraken
             * Spell vs Kraken, none
             * FireElves vs Dragon, friend      -> FireElves
             * Dragon vs FireElve, none
             * 
             * 
             * CalculateSpeciality(player.Card, enemy.Card);
             *      if (controlled ||
             *          afraid ||
             *          drowned )
             *          enemy wins;
             *      
             *      if (friend || 
             *          immune )
             *          player wins;
             *      
             *      else // none
             *          
             * CalculateSpeciality(enemy.Card, player.Card);
             *      if (friend ||
             *          immune)
             *          enemy wins
             *      
             *      if (controlled ||
             *          afraid ||
             *          drowned )
             *          player wins;
             *      
             *      else //none
             */

            // consider Specialities

            if (    player.Speciality == Specialities.friend ||
                    player.Speciality == Specialities.immune ||
                    enemy.Speciality == Specialities.afraid ||
                    enemy.Speciality == Specialities.drowned ||
                    enemy.Speciality == Specialities.controlled)
            {
                // set winner -> player, loser -> enemy
                //SetRoundEnding(player, enemy, false, round);
                //ChangeCards(_deckPlayer, _deckEnemy, enemy.Card);
                PlayerWinsRound(player, enemy, round);
                return;
            }
            else if(player.Speciality == Specialities.afraid ||
                    player.Speciality == Specialities.drowned ||
                    player.Speciality == Specialities.controlled ||
                    enemy.Speciality == Specialities.friend ||
                    enemy.Speciality == Specialities.immune)
            {
                // set winner -> enemy, loser -> player
                //SetRoundEnding(enemy, player, false, round);
                //ChangeCards(_deckEnemy, _deckPlayer, player.Card);
                EnemyWinsRound(player, enemy, round);
                return;
            }

            // Damage is already correctly set based on CardType (Monster - Spell) and only needs to be compared
            if (player.Damage > enemy.Damage)
                PlayerWinsRound(player, enemy, round);

            else if (player.Damage < enemy.Damage)
                EnemyWinsRound(player, enemy, round);

            else // draw
                SetRoundEnding(player, enemy, true, round);


            // old approach

            //if (enemy.Speciality == Specialities.friend ||
            //    enemy.Speciality == Specialities.immune)
            //{
            //    // set winner -> enemy, loser -> player
            //    //SetRoundEnding(enemy, player, false, round);
            //    //ChangeCards(_deckEnemy, _deckPlayer, player.Card);
            //    EnemyWinsRound(player, enemy, round);
            //    return;
            //}
            //else if (enemy.Speciality == Specialities.afraid ||
            //         enemy.Speciality == Specialities.drowned ||
            //         enemy.Speciality == Specialities.controlled)
            //{
            //    // set winner -> player, loser -> enemy
            //    //SetRoundEnding(player, enemy, false, round);
            //    //ChangeCards(_deckPlayer, _deckEnemy, enemy.Card);
            //    PlayerWinsRound(player, enemy, round);
            //    return;
            //}

            /*
            if(player.Card.CardType == CardType.monster && enemy.Card.CardType == CardType.monster)
            {
                // monster fight
                if(player.Card.Damage > enemy.Card.Damage) // player wins
                {
                    PlayerWinsRound(player, enemy, round);
                    return;
                }
                else if(player.Card.Damage < enemy.Card.Damage) // enemy wins
                {
                    EnemyWinsRound(player, enemy, round);
                    return;
                }
                else // draw
                {
                    SetRoundEnding(player, enemy, true, round);
                    // Cards don't change holder because of the draw.
                    return;
                }
            }
            else if(player.Card.CardType == CardType.spell && enemy.Card.CardType == CardType.spell)
            {
                // spell fight
                // CalculateElementMultiplicator()
                // compare card dmg
                var tmpDmg1 = CalculateElementMultiplicator(player.Card.Element);
            }
            else
            {
                // mixed fight
                    // CalculateElementMultiplicator()
                    // compare card dmg
            }
            */
        }

        public void PlayerWinsRound(Player player, Player enemy, Round round)
        {
            SetRoundEnding(player, enemy, false, round);
            ChangeCards(_deckPlayer, _deckEnemy, enemy.Card);
            return;
        }

        public void EnemyWinsRound(Player player, Player enemy, Round round)
        {
            SetRoundEnding(enemy, player, false, round);
            ChangeCards(_deckEnemy, _deckPlayer, player.Card);
            return;
        }

        public void SetRoundEnding(Player winner, Player loser, bool draw, Round round)
        {
            if(!draw)
            {
                round.Draw = false;
                round.Loser = loser.Username;
                round.Winner = winner.Username;

                return;
            }

            round.Draw = true;
            round.Loser = "none";
            round.Winner = "none";

            return;
        }

        public void ChangeCards(List<Card> winner, List<Card> loser, Card loserCard)
        {
            winner.Add(loserCard);
            loser.Remove(loserCard);
        }

        public List<Card> CardSetup(Deck deck)
        {
            var cards = new List<Card>();

            foreach (string cardId in deck.CardIds)
                cards.Add(_repoManager.GetCardByIdToken(cardId, deck.Token));

            //Card testCard = _repoManager.GetCardById("845f0dc7-37d0-426e-994e-43fc3ac83c08");
            //cards.Add(testCard);

            return cards;
        }

        public Specialities CalculateSpeciality(Card card1, Card card2)
        {
            switch (card1.Species)
            {
                case Species.elf: // can evade dragons
                    if (card1.Element == Element.fire && card2.Species == Species.dragon)
                        return Specialities.friend; // card1 wins
                    break;

                case Species.ork: // controlled by wizzards
                    if (card2.Species == Species.wizzard)
                        return Specialities.controlled; // card1 loses
                    break;

                //case Species.dragon:
                //    break;

                case Species.goblin: // afraid of dragons
                    if (card2.Species == Species.dragon)
                        return Specialities.afraid; // card1 loses
                    break;

                case Species.knight: // drown
                    if(card2.CardType == CardType.spell && card2.Element == Element.water)
                        return Specialities.drowned; // card1 loses
                    break;

                case Species.kraken: // immune against spells
                    if (card2.CardType == CardType.spell)
                        return Specialities.immune; // card1 wins
                    break;
                //case Species.wizzard:
                //    break;
            }

            return Specialities.none;
        }

        //public double CalculateElementMultiplicator(Element element)
        //{
        //    double multiplier = 1;

        //    switch(element)
        //    {
        //        case Element.normal:
        //            multiplier = CompareElementEffectiveness(Element.water, Element.fire, element);
        //            break;
        //        case Element.fire:
        //            multiplier = CompareElementEffectiveness(Element.normal, Element.water, element);
        //            break;
        //        case Element.water:
        //            multiplier = CompareElementEffectiveness(Element.fire, Element.normal, element);
        //            break;
        //    }

        //    return multiplier;
        //}

        //private double CompareElementEffectiveness(Element effective, Element ineffective, Element element)
        //{
        //    if (element == effective) 
        //        return 2;
        //    else if (element == ineffective) 
        //        return 0.5;
        //    else 
        //        return 1;
        //}
    }
}
