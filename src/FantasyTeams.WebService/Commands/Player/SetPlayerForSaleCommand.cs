﻿using FantasyTeams.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FantasyTeams.Commands.Player
{
    public class SetPlayerForSaleCommand : IRequest<CommandResponse>
    {
        [Required]
        [RegularExpression("[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}",
            ErrorMessage = "Please provide correct GUID")]
        public string PlayerId { get; set; }
        [JsonIgnore]
        public string TeamId { get; set; }
        [Required]
        [Range(0, 1000000000, ErrorMessage = "Please provide Asking price between 0 to 1000000000")]
        public double AskingPrice { get; set; }
    }
}
