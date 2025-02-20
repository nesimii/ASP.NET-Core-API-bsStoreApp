﻿using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects.UserDtos;

public record UserForAuthenticationDto
{
    [Required(ErrorMessage = "Username is required.")]
    public string UserName { get; init; }

    [Required(ErrorMessage = "Passowrd is required.")]
    public string Password { get; init; }
}
