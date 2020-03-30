using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cloudSite.Model;
using cloudSite.ViewData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cloudSite.Controllers
{
    [Route("api/[controller]")]
    public class UserInfo : ControllerBase
    {
        private ApplicationDbContext _db;

        public UserInfo(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet, Route("getUsersLogInfo")]
        public async Task<IEnumerable<UserLogInfoViewData>> GetUserLogInfos()
        {
            var usersInfo = await _db.LoggerUserInfos
                .Include(u => u.User)
                .OrderByDescending(u => u.DateTime)
                .Select(u => new UserLogInfoViewData
                {
                    Firstname = u.User.Firstname,
                    Surname = u.User.Surname,
                    Date = u.DateTime
                })
                .ToListAsync();
            return usersInfo;
        }
    }
}