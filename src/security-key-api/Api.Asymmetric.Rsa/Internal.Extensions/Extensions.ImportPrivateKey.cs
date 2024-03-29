using System;
using System.Security.Cryptography;

namespace GarageGroup.Infra;

partial class RsaExtensions
{
    internal static RSA ImportPrivateKey(this RSA rsa, ReadOnlySpan<byte> privateKey)
    {
        rsa.ImportRSAPrivateKey(privateKey, out var _);
        return rsa;
    }
}