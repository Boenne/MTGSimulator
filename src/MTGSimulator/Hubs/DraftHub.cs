using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MTGSimulator.Data;
using MTGSimulator.Data.ContextFactory;
using MTGSimulator.Data.Extensions;
using MTGSimulator.Data.Models;
using MTGSimulator.Data.Repositories;
using MTGSimulator.Service;

namespace MTGSimulator.Hubs
{
    public class DraftHub : Hub
    {
        private readonly BoosterCreator boosterCreator;
        private readonly Dictionary<string, string> connections = new Dictionary<string, string>();
        private readonly DraftPlayerRepository draftPlayerRepository;
        private readonly DraftSessionRepository draftSessionRepository;
        private readonly Logger logger;

        public DraftHub()
        {
            var databaseContextFactory = new DatabaseContextFactory();
            logger = new Logger();
            draftSessionRepository = new DraftSessionRepository(databaseContextFactory, logger);
            draftPlayerRepository = new DraftPlayerRepository(databaseContextFactory, logger);
            boosterCreator = new BoosterCreator(new CardParser());
        }

        public async Task CreateDraft(string setCode)
        {
            try
            {
                var draftId = Guid.NewGuid().ToString().GenerateHash();
                var playerId = Guid.NewGuid().ToString().GenerateHash();
                var draftSession = new DraftSession { Id = draftId, SetCode = setCode };
                var draftPlayer = new DraftPlayer { DraftSessionId = draftSession.Id, Id = playerId };
                await draftSessionRepository.Save(draftSession);
                await draftPlayerRepository.Save(draftPlayer);
                await JoinGroup(draftId);
                var boosters = await boosterCreator.CreateBoosters(setCode, 3);
                connections.Add(draftPlayer.Id, Context.ConnectionId);
                Clients.Caller.InitializeGame(draftId, draftPlayer, boosters);
            }
            catch (Exception e)
            {
                logger.Error($"{nameof(CreateDraft)} failed for {nameof(setCode)} '{setCode}'", e);
            }
        }

        public async Task JoinDraft(string draftId)
        {
            try
            {
                var draftSession = await draftSessionRepository.Get(draftId);
                if (draftSession == null) return;
                var playerId = Guid.NewGuid().ToString().GenerateHash();
                var draftPlayer = new DraftPlayer { DraftSessionId = draftSession.Id, Id = playerId };
                await draftPlayerRepository.Save(draftPlayer);
                var boosters = await boosterCreator.CreateBoosters(draftSession.SetCode, 3);
                Clients.Caller.InitializeGame(draftId, draftPlayer, boosters);
            }
            catch (Exception e)
            {
                logger.Error($"{nameof(JoinDraft)} failed for {nameof(draftId)} '{draftId}'", e);
            }
        }

        public async Task PassOnBooster(string playerId, string draftId, dynamic booster)
        {
            try
            {
                if (!await draftSessionRepository.HasStarted(draftId)
                    || !await draftPlayerRepository.PlayerExists(playerId, draftId)) return;

                var nextPlayer = await draftPlayerRepository.GetNextPlayer(playerId, draftId);
                if (nextPlayer == null) return;

                Clients.User(connections[nextPlayer]).ReceiveBooster(booster);
            }
            catch (Exception e)
            {
                logger.Error(
                    $"{nameof(PassOnBooster)} failed for {nameof(playerId)} '{playerId}' and {nameof(draftId)} '{draftId}'",
                    e);
            }
        }

        public async Task StartDraft(string draftId)
        {
            try
            {
                if (await draftPlayerRepository.GetNumberOfPlayers(draftId) < 2) return;
                await draftSessionRepository.Start(draftId);
                Clients.Group(draftId).DraftHasStarted();
            }
            catch (Exception e)
            {
                logger.Error($"{nameof(StartDraft)} failed for {nameof(draftId)} '{draftId}'", e);
            }
        }

        private Task JoinGroup(string group)
        {
            return Groups.Add(Context.ConnectionId, group);
        }
    }
}