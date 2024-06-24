using Firebase.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WhackAMath
{
    public interface IDataService
    {
        Task SaveGameDataAsync(Dictionary<string, object> data);
        Task<Dictionary<string, object>> LoadGameDataAsync();
        void Logout();
    }
}
