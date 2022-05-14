﻿using FantasyTeams.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace FantasyTeams.Commands.Player
{
    public class CreateNewPlayerCommand : IRequest<CommandResponse>
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9]?$",
            ErrorMessage = "Please provide alpha numeric value")]
        public string FirstName { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z0-9]?$",
            ErrorMessage = "Please provide alpha numeric value")]
        public string LastName { get; set; }
        [Required]
        public string Country { get; set; }
    }
}
