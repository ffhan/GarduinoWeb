﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models;
using GarduinoUniversal;
using Microsoft.EntityFrameworkCore;

namespace Garduino.Data
{
    public class EntryRepository : IMeasureRepository
    {
        private readonly ApplicationDbContext _context;

        public EntryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Measure measure, ApplicationUser user)
        {
            measure.SetUser(user);
            bool tmp = await ContainsAsync(measure, user);
            if (!tmp)
            {
                _context.Measure.Add(measure);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public IEnumerable<Measure> GetAll(ApplicationUser user)
        {
            return _context.Measure.Where(g => StringOperations.IsFromUser(g.User.Id, user.Id)).OrderByDescending(g => g.DateTime);
        }

        public IEnumerable<Measure> GetDevice(string device, ApplicationUser user)
        {
            return _context.Measure.Where(g => g.IsFromDevice(device) && StringOperations.IsFromUser(g.User.Id, user.Id));
        }

        public async Task<Measure> GetAsync(Measure measure, ApplicationUser user)
        {
            return await _context.Measure.FirstOrDefaultAsync(g => g.Equals(measure) && StringOperations.IsFromUser(
                g.User.Id, user.Id));
        }
        public async Task<Measure> GetAsync(Guid id, ApplicationUser user)
        {
            return await _context.Measure.FirstOrDefaultAsync(g => g.Id == id && StringOperations.IsFromUser(
                g.User.Id, user.Id));
        }

        public async Task<Measure> GetAsync(DateTime dateTime, ApplicationUser user)
        {
            var tmp = await _context.Measure.FirstOrDefaultAsync(g => g.DateTime.Equals(dateTime) && StringOperations.IsFromUser(
                g.User.Id, user.Id));
            return tmp;
        }

        public async Task<IEnumerable<Measure>> GetRangeAsync(DateTime dateTime1, DateTime dateTime2, ApplicationUser user)
        {
            return _context.Measure.Where(m => m.DateTime.CompareTo(dateTime1) >= 0 && m.DateTime.CompareTo(dateTime2) <= 0
            && StringOperations.IsFromUser(m.User.Id, user.Id));
        }

        public async Task<bool> UpdateAsync(Guid id, Measure measure, ApplicationUser user)
        {
            Measure mes = await GetAsync(id, user);
            if (mes is null) return false;
            mes.Update(measure);
            _context.Entry(mes).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ContainsAsync(mes, user))
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

        public async Task<bool> ContainsAsync(Measure measure, ApplicationUser user)
        {
            return await _context.Measure.AnyAsync(g => g.EqualsEf(measure) && 
            StringOperations.IsFromUser(g.User.Id, user.Id));
        }

        public async Task<bool> ContainsAsync(Guid id, ApplicationUser user)
        {
            return await _context.Measure.AnyAsync(g => StringOperations.IsFromUser(g.User.Id, user.Id) && g.Id.Equals(id));
        }

        public async Task<bool> DeleteAsync(Guid id, ApplicationUser user)
        {
            try
            {
                _context.Measure.Remove(await GetAsync(id, user));
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            return true;
        }

        public async Task<Guid> GetId(Measure measure, ApplicationUser user)
        {
            Measure mes = await GetAsync(measure.DateTime, user);
            return mes.Id;
        }

        public bool AreEqual(Measure m1, Measure m2)
        {
            return m1.Equals(m2);
        }

        public async Task<bool> DeleteAllAsync()
        {
            try
            {
                await _context.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Measure");
            }
            catch (Exception e)
            {
                return false;
            }
            
            return true;
        }

        public Measure GetLatest(ApplicationUser user)
        {
            return GetAll(user).FirstOrDefault();
        }

        public async Task AddAllAsync(ISet<Measure> all, ApplicationUser user)
        {
            foreach (var measure in all)
            {
                await AddAsync(measure, user);
            }
        }
    }
}
