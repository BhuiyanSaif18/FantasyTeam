﻿using FantasyTeams.Commands;
using FantasyTeams.Entities;
using FantasyTeams.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FantasyTeams.Repository
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly IMongoCollection<Player> _collection;
        private readonly DbConfiguration _settings;
        public PlayerRepository(IOptions<DbConfiguration> settings)
        {
            _settings = settings.Value;
            var client = new MongoClient(_settings.ConnectionString);
            var database = client.GetDatabase(_settings.DatabaseName);
            _collection = database.GetCollection<Player>("Player");
        }

        public Task<List<Player>> GetAllAsync()
        {
            return _collection.Find(x => true).ToListAsync();
        }
        public Task<Player> GetByIdAsync(string id)
        {
            return _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }
        public async Task CreateAsync(Player player)
        {
            await _collection.InsertOneAsync(player).ConfigureAwait(false);
        }
        public Task UpdateAsync(string id, Player team)
        {
            return _collection.ReplaceOneAsync(x => x.Id == id, team);
        }
        public Task DeleteAsync(string id)
        {
            return _collection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task CreateManyAsync(List<Player> players)
        {
             await _collection.InsertManyAsync(players);
        }

        public async Task DeleteManyAsync(string teamId)
        {
            await _collection.DeleteManyAsync(x => x.TeamId == teamId);
        }

        public async Task<List<Player>> GetAllAsync(string teamId)
        {
            return await _collection.Find(x => x.TeamId == teamId).ToListAsync();
        }

        public async Task<Player> GetByNameAsync(string fullName)
        {
            return await _collection.Find(x => x.FullName == fullName).FirstOrDefaultAsync();
        }
    }
}
