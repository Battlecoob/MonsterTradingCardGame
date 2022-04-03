using MonsterTradingCardGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.DAL
{
    public interface IUserRepo
    {
        bool InsertUser(User user);

        //void TruncateAllAndRestartId(string authToken);

        User GetUserByToken(string authToken);
        List<UserStats> GetTop10UserScore();
        User GetUserByCredentials(string username, string password);

        int SelectCoins(string authToken);
        UserStats SelectUserStats(string authToken);
        UserData SelectUserDataByUsername(string username);

        void UpdateCoins(string authToken);
        void UpdateStatsDraw(string authToken);
        void UpdateStatsLoser(string authToken);
        void UpdateStatsWinner(string authToken);
        void UpdateUserData(string username, UserData userData);

        void TruncateTables();
    }
}
