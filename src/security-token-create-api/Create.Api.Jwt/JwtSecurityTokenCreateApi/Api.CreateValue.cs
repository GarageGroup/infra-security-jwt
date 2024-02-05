﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace GarageGroup.Infra;

partial class JwtSecurityTokenCreateApi
{
    public SecurityTokenValue CreateTokenValue(ClaimsIdentity claimsIdentity)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Convert.FromBase64String(option.PrivateKeyBase64);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow + option.Ttl,
            SigningCredentials = signingCredentialsApi.GetSigningCredentials(key)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new(
            tokenType: BearerTokenType,
            token: tokenHandler.WriteToken(token),
            ttl: option.Ttl);
    }
}