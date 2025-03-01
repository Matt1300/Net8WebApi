﻿using LearnAPI.Repos;
using LearnAPI.Repos.Models;
using LearnAPI.Service;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace LearnAPI.Container
{
    public class RefreshHandler : IRefreshHandler
    {
        private readonly LearndataContext _context;

        public RefreshHandler(LearndataContext context)
        {
            _context = context;
        }
        public async Task<string> GenerateToken(string username)
        {
            var randomnumber = new byte[32];
            using (var randomnumbergenerator = RandomNumberGenerator.Create())
            {
                randomnumbergenerator.GetBytes(randomnumber);
                string refreshtoken = Convert.ToBase64String(randomnumber);
                var ExistToken = _context.TblRefreshtokens.FirstOrDefaultAsync(item => item.Userid == username).Result;
                if (ExistToken != null)
                {
                    ExistToken.Refreshtoken = refreshtoken;
                }
                else
                {
                    await _context.TblRefreshtokens.AddAsync(new TblRefreshtoken
                    {
                        Userid = username,
                        Tokenid = new Random().Next().ToString(),
                        Refreshtoken = refreshtoken
                    });
                }

                await _context.SaveChangesAsync();

                return refreshtoken;
            }
        }
    }
}
