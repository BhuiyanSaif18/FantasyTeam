﻿using FantasyTeams.Commands.Player;
using FantasyTeams.Contracts;
using FantasyTeams.Entities;
using FantasyTeams.Enums;
using FantasyTeams.Models;
using FantasyTeams.Queries.Player;
using FantasyTeams.Repository;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FantasyTeams.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly ILogger<PlayerService> _logger;
        private readonly IRepository<Player> _playerRepository;
        private readonly IRepository<Team> _teamRepository;
        public PlayerService(ILogger<PlayerService> logger,
            IRepository<Player> playerRepository,
            IRepository<Team> teamRepository)
        {
            _logger = logger;
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;
        }
        public async Task<CommandResponse> CreateNewPlayer(CreateNewPlayerCommand createNewPlayerCommand)
        {
            var team = await _teamRepository.GetAsync(x=> x.Id == createNewPlayerCommand.TeamId);
            if(team == null)
            {
                return CommandResponse.Failure(new string[] { "Team doesn't exist to create player" });
            }
            var fullname = createNewPlayerCommand.FirstName + " " + createNewPlayerCommand.LastName;

            var player = await _playerRepository.GetAsync(x => x.FullName == fullname);
            if(player != null)
            {
                return CommandResponse.Failure(new string[] { "Player name already exists" });
            }
            Random rnd = new Random();

            player = new Player();
            player.Id = Guid.NewGuid().ToString();
            player.FirstName = createNewPlayerCommand.FirstName;
            player.LastName = createNewPlayerCommand.LastName;
            player.FullName = createNewPlayerCommand.FirstName + " " + createNewPlayerCommand.LastName;
            player.Country = createNewPlayerCommand.Country;
            player.Value = 1000000;
            player.Age = rnd.Next(18, 40);
            player.ForSale = false;
            player.AskingPrice = 0;
            player.PlayerType = createNewPlayerCommand.PlayerType;
            player.TeamId = team.Id;
            player.TeamName = team.Name;
            await _playerRepository.CreateAsync(player);

            await AddNewPlayerToTeam(team, player);
            return CommandResponse.Success();

        }

        private async Task AddNewPlayerToTeam(Team team, Player playerInfo)
        {
            team.Value += playerInfo.Value;

            if (playerInfo.PlayerType == PlayerType.Defender.ToString())
            {
                var playerTypeList = team.Defenders.ToList();
                playerTypeList.Add(playerInfo.Id);
                team.Defenders = playerTypeList.ToArray();
            }
            else if (playerInfo.PlayerType == PlayerType.Attacker.ToString())
            {
                var playerTypeList = team.Attackers.ToList();
                playerTypeList.Add(playerInfo.Id);
                team.Attackers = playerTypeList.ToArray();
            }
            else if (playerInfo.PlayerType == PlayerType.MidFielder.ToString())
            {
                var playerTypeList = team.MidFielders.ToList();
                playerTypeList.Add(playerInfo.Id);
                team.MidFielders = playerTypeList.ToArray();
            }
            else if (playerInfo.PlayerType == PlayerType.GoalKeeper.ToString())
            {
                var playerTypeList = team.GoalKeepers.ToList();
                playerTypeList.Add(playerInfo.Id);
                team.GoalKeepers = playerTypeList.ToArray();
            }

            await _teamRepository.UpdateAsync(x => x.Id == team.Id, team);
        }

        public async Task<QueryResponse> GetAllPlayer(GetAllPlayerQuery getAllPlayerQuery)
        {
            var players = new List<Player>();
            if (string.IsNullOrEmpty(getAllPlayerQuery.TeamId))
            {
                players = await _playerRepository.GetAllAsync();
                return QueryResponse.Success(players);
            }
            var filter = Builders<Player>.Filter.Eq(x => x.TeamId, getAllPlayerQuery.TeamId);
            
            players =  await _playerRepository.GetAllByFilterAsync(filter);
            return QueryResponse.Success(players);
        }

        //public async Task<List<Player>> CreateNewTeamPlayers(Team team)
        //{
        //    List<Player> players = new List<Player>();
        //    players.AddRange(AddGoalKeepers(team));
        //    players.AddRange(AddDefenders(team));
        //    players.AddRange(AddAttackers(team));
        //    players.AddRange(AddMidFielders(team));

        //    await _playerRepository.CreateManyAsync(players);
        //    return players;
        //}

        //private IEnumerable<Player> AddMidFielders(Team team)
        //{
        //    Random rnd = new Random();
        //    List<Player> players = new List<Player>();
        //    for(int i = 0; i < 6; i++)
        //    {
        //        var player = new Player();
        //        player.Id = Guid.NewGuid().ToString();
        //        player.FirstName = "MidFielder";
        //        player.LastName = Guid.NewGuid().ToString();
        //        player.FullName = player.FirstName + " " + player.LastName;
        //        player.Country = team.Country;
        //        player.Value = 1000000;
        //        player.Age = rnd.Next(18, 40);
        //        player.ForSale = false;
        //        player.AskingPrice = 0;
        //        player.PlayerType = PlayerType.MidFielder.ToString();
        //        player.TeamId = team.Id;
        //        player.TeamName = team.Name;
        //        players.Add(player);
        //    }
        //    return players;
        //}

        //private IEnumerable<Player> AddAttackers(Team team)
        //{
        //    Random rnd = new Random();
        //    List<Player> players = new List<Player>();
        //    for (int i = 0; i < 5; i++)
        //    {
        //        var player = new Player();
        //        player.Id = Guid.NewGuid().ToString();
        //        player.FirstName = "Attacker";
        //        player.LastName = Guid.NewGuid().ToString();
        //        player.FullName = player.FirstName + " " + player.LastName;
        //        player.Country = team.Country;
        //        player.Value = 1000000;
        //        player.Age = rnd.Next(18, 40);
        //        player.ForSale = false;
        //        player.AskingPrice = 0;
        //        player.PlayerType = PlayerType.Attacker.ToString();
        //        player.TeamId = team.Id;
        //        player.TeamName = team.Name;
        //        players.Add(player);
        //    }
        //    return players;
        //}

        //private IEnumerable<Player> AddDefenders(Team team)
        //{
        //    Random rnd = new Random();
        //    List<Player> players = new List<Player>();
        //    for (int i = 0; i < 6; i++)
        //    {
        //        var player = new Player();
        //        player.Id = Guid.NewGuid().ToString();
        //        player.FirstName = "Defender";
        //        player.LastName = Guid.NewGuid().ToString();
        //        player.FullName = player.FirstName + " " + player.LastName;
        //        player.Country = team.Country;
        //        player.Value = 1000000;
        //        player.Age = rnd.Next(18, 40);
        //        player.ForSale = false;
        //        player.AskingPrice = 0;
        //        player.PlayerType = PlayerType.Defender.ToString();
        //        player.TeamId = team.Id;
        //        player.TeamName = team.Name;
        //        players.Add(player);
        //    }
        //    return players;
        //}

        //private IEnumerable<Player> AddGoalKeepers(Team team)
        //{
        //    Random rnd = new Random();
        //    List<Player> players = new List<Player>();
        //    for (int i = 0; i < 3; i++)
        //    {
        //        var player = new Player();
        //        player.Id = Guid.NewGuid().ToString();
        //        player.FirstName = "Keeper";
        //        player.LastName = Guid.NewGuid().ToString();
        //        player.FullName = player.FirstName + " " + player.LastName;
        //        player.Country = team.Country;
        //        player.Value = 1000000;
        //        player.Age = rnd.Next(18, 40);
        //        player.ForSale = false;
        //        player.AskingPrice = 0;
        //        player.PlayerType = PlayerType.GoalKeeper.ToString();
        //        player.TeamId = team.Id;
        //        player.TeamName = team.Name;
        //        players.Add(player);
        //    }
        //    return players;
        //}

        public async Task<CommandResponse> SetPlayerForSale(SetPlayerForSaleCommand setPlayerForSaleCommand)
        {
            var playerInfo  = await _playerRepository.GetAsync(x => x.Id == setPlayerForSaleCommand.PlayerId);
            if(playerInfo == null)
            {
                return CommandResponse.Failure(new string[] { "No player found for update" });
            }
            if(playerInfo.TeamId != setPlayerForSaleCommand.TeamId)
            {
                return CommandResponse.FailureForBidden(new string[] { "Can not update other team player price" });
            }
            playerInfo.ForSale = true;
            playerInfo.AskingPrice = setPlayerForSaleCommand.AskingPrice;
            await _playerRepository.UpdateAsync(x => x.Id == playerInfo.Id, playerInfo);
            return CommandResponse.Success();
        }

        public async Task<CommandResponse> UpdatePlayerInfo(UpdatePlayerCommand updatePlayerCommand)
        {
            var playerInfo = await _playerRepository.GetAsync( x=> x.Id == updatePlayerCommand.PlayerId);
            if (playerInfo == null)
            {
                return CommandResponse.Failure(new string[] { "Player not found for update" });
            }
            if(playerInfo.TeamId != updatePlayerCommand.TeamId && !string.IsNullOrEmpty(updatePlayerCommand.TeamId))
            {
                return CommandResponse.FailureForBidden(new string[] { "You can not update other team" });
            }
            

            playerInfo.FirstName = string.IsNullOrEmpty(updatePlayerCommand.FirstName)?
                playerInfo.FirstName : updatePlayerCommand.FirstName;
            playerInfo.LastName = string.IsNullOrEmpty(updatePlayerCommand.LastName)?
                playerInfo.LastName : updatePlayerCommand.LastName;
            playerInfo.FullName = playerInfo.FirstName + " " + playerInfo.LastName;
            if(!string.IsNullOrEmpty(updatePlayerCommand.FirstName) || !string.IsNullOrEmpty(updatePlayerCommand.LastName))
            {
                var playerAlreadyExistsWithName = await _playerRepository.GetAsync(x=> x.FullName == playerInfo.FullName);

                if(playerAlreadyExistsWithName != null)
                {
                    return CommandResponse.Failure(new string[] {"Player name already exists"});
                }
            }

            playerInfo.Country = string.IsNullOrEmpty(updatePlayerCommand.Country)?
                playerInfo.Country : updatePlayerCommand.Country;

            await _playerRepository.UpdateAsync(x=> x.Id == updatePlayerCommand.PlayerId, playerInfo);
            return CommandResponse.Success();
        }

        public async Task<CommandResponse> DeletePlayer(DeletePlayerCommand deletePlayerCommand)
        {
            var player = await _playerRepository.GetAsync(x=> x.Id == deletePlayerCommand.PlayerId);
            if(player == null)
            {
                return CommandResponse.Failure(new string[] { "Player not found to delete" });
            }
            if (!string.IsNullOrEmpty(player.TeamId))
            {
                var team = await _teamRepository.GetAsync(x=> x.Id == player.TeamId);
                if(team != null)
                {
                    team.Value -= player.Value;
                    await DeletePlayerFromTeam(team, player);
                }
            }
            await _playerRepository.DeleteAsync(x=> x.Id == deletePlayerCommand.PlayerId);
            return CommandResponse.Success();
        }

        private async Task DeletePlayerFromTeam(Team team, Player playerInfo)
        {
            if (playerInfo.PlayerType == PlayerType.Defender.ToString())
            {
                team.Defenders = team.Defenders.Where(e => e != playerInfo.Id).ToArray();
            }
            else if (playerInfo.PlayerType == PlayerType.Attacker.ToString())
            {
                team.Attackers = team.Attackers.Where(e => e != playerInfo.Id).ToArray();
            }
            else if (playerInfo.PlayerType == PlayerType.MidFielder.ToString())
            {
                team.MidFielders = team.MidFielders.Where(e => e != playerInfo.Id).ToArray();
            }
            else if (playerInfo.PlayerType == PlayerType.GoalKeeper.ToString())
            {
                team.GoalKeepers = team.GoalKeepers.Where(e => e != playerInfo.Id).ToArray();
            }

            await _teamRepository.UpdateAsync(x=> x.Id == team.Id, team);
        }

        public async Task<CommandResponse> UpdatePlayerValue(UpdatePlayerValueCommand updatePlayerPriceCommand)
        {
            var player = await _playerRepository.GetAsync(x=> x.Id == updatePlayerPriceCommand.PlayerId);
            if(player == null)
            {
                return CommandResponse.Failure(new string[] {"Player not found for update"});
            }

            var team = await _teamRepository.GetAsync(x=> x.Id == player.TeamId);
            if (team != null)
            {
                team.Value -= player.Value;
                team.Value += updatePlayerPriceCommand.PlayerValue;
                await _teamRepository.UpdateAsync(x=> x.Id == team.Id, team);
            }
            player.Value = updatePlayerPriceCommand.PlayerValue;
            await _playerRepository.UpdateAsync(x=> x.Id == player.Id, player);
            return CommandResponse.Success();
        }
        //public async Task<CommandResponse> DeleteTeamPlayers(string teamId)
        //{
        //    await _playerRepository.DeleteManyAsync(teamId);
        //    return CommandResponse.Success();
        //}

        public async Task<QueryResponse> GetPlayer(GetPlayerQuery request)
        {
            var player = await _playerRepository.GetAsync(x=> x.Id == request.PlayerId);
            if(player == null)
            {
                return QueryResponse.Failure(new string[] { "Player not found" });
            }
            if( player.TeamId == request.TeamId || string.IsNullOrEmpty(request.TeamId))
            {
                return QueryResponse.Success(player);
            }
            return QueryResponse.FailureForBidden(new string[] {"Can not fetch other team player"});
        }
    }
}
