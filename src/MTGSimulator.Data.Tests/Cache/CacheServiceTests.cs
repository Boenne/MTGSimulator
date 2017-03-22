using System;
using System.Collections.Generic;
using System.Text;
using MTGSimulator.Data.Cache;
using MTGSimulator.Data.Models;
using Shouldly;
using Xunit;

namespace MTGSimulator.Data.Tests.Cache
{
    public class CacheServiceTests
    {
        [Fact]
        public void Get_InputMatchesCachedItem_ReturnsCachedItem()
        {
            var cacheService = new CacheService();
            var draftSession = new DraftSession();
            var draftPlayer = new DraftPlayer();
            cacheService.Cache("method", "tag", draftPlayer, draftSession, "arg2");

            var response = cacheService.Get<DraftPlayer>("method", draftSession, "arg2");

            response.Hit.ShouldBe(true);
            response.Value.ShouldBe(draftPlayer);
        }

        [Fact]
        public void Get_InputDoesNotMatchCachedItem_ReturnsDefault()
        {
            var cacheService = new CacheService();
            var draftSession = new DraftSession();
            var draftPlayer = new DraftPlayer();
            cacheService.Cache("method", "tag", draftPlayer, draftSession, "arg2");

            var response = cacheService.Get<DraftPlayer>("method", new DraftSession(), "arg2");

            response.Hit.ShouldBe(false);
            response.Value.ShouldBeNull();
        }

        [Fact]
        public void Get_ArgsCountDoesNotMatchCachedItem_ReturnsDefault()
        {
            var cacheService = new CacheService();
            var draftSession = new DraftSession();
            var draftPlayer = new DraftPlayer();
            cacheService.Cache("method", "tag", draftPlayer, draftSession, "arg2");

            var response = cacheService.Get<DraftPlayer>("method", draftSession);

            response.Hit.ShouldBe(false);
            response.Value.ShouldBeNull();
        }
    }
}
