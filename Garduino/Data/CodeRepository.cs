﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models;
using GarduinoUniversal;
using Microsoft.EntityFrameworkCore;

namespace Garduino.Data
{
    public class CodeRepository : ICodeRepository //TODO: schedule codes
    { //TODO: Register code for reuse, then use them like this.
        private readonly ApplicationDbContext _context;

        public CodeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Code code, ApplicationUser user)
        {
            code.SetUser(user);
            code.DateArrived = DateTime.Now;
            try
            {
                await _context.Code.AddAsync(code);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public IEnumerable<Code> GetAll(ApplicationUser user)
        {
            return _context.Code.Where(g => StringOperations.IsFromUser(g.User.Id, user.Id)).OrderByDescending(g => g);
        }

        public Code GetLatest(ApplicationUser user)
        {
            return GetAll(user)?.FirstOrDefault(g => !g.IsCompleted);
        }

        public IEnumerable<Code> GetActive(ApplicationUser user) => _context.Code.Where(g => StringOperations.IsFromUser(
            g.User.Id, user.Id) && !g.IsCompleted).OrderByDescending(g => g);

        public async Task Complete(Code code, DateTime dateExecuted, ApplicationUser user)
        {
            code.Complete(dateExecuted);
            await UpdateAsync(code.Id, code, user);
        }

        public async Task Complete(Guid id, DateTime dateExecuted, ApplicationUser user)
        {
            Code code = await GetAsync(id, user);
            code.Complete(dateExecuted);
            await UpdateAsync(id, code, user);
        }

        public IEnumerable<Code> GetDeviceFromActiveCodes(string device, ApplicationUser user)
        {
            return _context.Code.Where(g => StringOperations.IsFromUser(
                g.User.Id, user.Id) && g.IsFromDevice(device) && !g.IsCompleted);
        }

        public IEnumerable<Code> GetDevice(string device, ApplicationUser user)
        {
            return _context.Code.Where(g => StringOperations.IsFromUser(g.User.Id, user.Id) && g.IsFromDevice(device));
        }

        public async Task<Code> GetAsync(Guid id, ApplicationUser user)
        {
            return await _context.Code.FirstOrDefaultAsync(g => g.Id.Equals(id) && StringOperations.IsFromUser(
                g.User.Id, user.Id));
        }

        public async Task<Code> GetAsync(DateTime dateTime, ApplicationUser user)
        {
            return await _context.Code.FirstOrDefaultAsync(g =>
                g.DateArrived.Equals(dateTime) && StringOperations.IsFromUser(g.User.Id, user.Id));
        }

        public async Task<Code> GetAsync(Code what, ApplicationUser user)
        {
            return await _context.Code.FirstOrDefaultAsync(g => g.Equals(what) && StringOperations.IsFromUser(
                g.User.Id, user.Id));
        }

        public async Task<IEnumerable<Code>> GetRangeAsync(DateTime dateTime1, DateTime dateTime2, string userId)
        {
            return _context.Code.Where(g => g.DateArrived.CompareTo(dateTime1) >= 0 &&
                                                  g.DateArrived.CompareTo(dateTime2) <= 0);
        }

        public async Task<bool> UpdateAsync(Guid id, Code what, ApplicationUser user)
        {
            Code code = await GetAsync(id, user);
            if (code is null) return false;
            code.Update(what);
            _context.Entry(code).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ContainsAsync(code, user))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            return true;
        }

        public async Task<bool> ContainsAsync(Code what, ApplicationUser user)
        {
            return await _context.Code.AnyAsync(g => g.IsUser(user) && g.Equals(what));
        }

        public async Task<bool> ContainsAsync(Guid id, ApplicationUser user)
        {
            return await _context.Code.AnyAsync(g => g.IsUser(user) && g.Id.Equals(id));
        }

        public async Task<bool> DeleteAsync(Guid id, ApplicationUser user)
        {
            try
            {
                _context.Code.Remove(await GetAsync(id, user));
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            return true;
        }

        public async Task<Guid> GetId(Code what, ApplicationUser user)
        {
            Code mes = await GetAsync(what.DateArrived, user);
            return mes.Id;
        }

        public bool AreEqual(Code m1, Code m2)
        {
            return m1.Equals(m2);
        }

        public async Task<bool> DeleteAllAsync()
        {
            try
            {
                await _context.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Code");
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public async Task AddAllAsync(ISet<Code> all, ApplicationUser user)
        {
            foreach(var code in all)
            {
                await AddAsync(code, user);
            }
        }
    }
}
