using Microsoft.EntityFrameworkCore;
using RestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Repositories
{
    public class ManageAcc : IManageAcc
    {
        private readonly ApplicationDbContext _context;
        public ManageAcc(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAccs()
        {
            var allAcc = await _context.Users.ToListAsync();
            return allAcc;
        }

        //public async Task Delete(int id)
        //{
        //    var findAcc = await _context.Users.FindAsync(id);
        //    _context.Users.Remove(findAcc);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task<User> GetAcc(int id)
        //{
        //    var findAcc = await _context.Users.FindAsync(id);
        //    return findAcc;
        //}

        public async Task<User> GetAcc(string id)
        {
            var findAcc = await _context.Users.FindAsync(id);
            return findAcc;
        }

        public async Task Delete(string id)
        {
            var findAcc = await _context.Users.FindAsync(id);
            _context.Users.Remove(findAcc);
            await _context.SaveChangesAsync();
        }
    }
}
