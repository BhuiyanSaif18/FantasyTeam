﻿using FantasyTeams.Commands.Team;
using FantasyTeams.Contracts;
using FantasyTeams.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FantasyTeams.CommandHandlers.Team
{
    public class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand, CommandResponse>
    {
        private readonly ITeamService _teamService;
        public UpdateTeamCommandHandler(ITeamService teamService)
        {
            _teamService = teamService;
        }
        public async Task<CommandResponse> Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
        {
            return await _teamService.UpdateTeamInfo(request);
        }
    }
}
