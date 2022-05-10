﻿using FantasyTeams.Commands;
using FantasyTeams.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FantasyTeams.Repository
{
    public interface IPlayerRepository
    {
        Task<List<Player>> GetAllAsync();
        Task<Player> GetByIdAsync(string id);
        Task CreateAsync(Player player);
        void CreateMany(List<Player> players);
        Task UpdateAsync(string id, Player team);
        Task DeleteAsync(string id);
    }
}