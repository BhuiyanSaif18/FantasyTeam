﻿using FantasyTeams.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace FantasyTeams.Commands.Player
{
    public class DeletePlayerCommand : IRequest<CommandResponse>
    {
        [Required]
        [RegularExpression("[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}",
            ErrorMessage = "Please provide correct GUID")]
        public string PlayerId { get; set; }
    }
}
