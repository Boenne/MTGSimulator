using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MTGSimulator.Data;
using MTGSimulator.Data.Extensions;
using MTGSimulator.Data.Models;
using MTGSimulator.Data.Repositories;
using MTGSimulator.Service;

namespace MTGSimulator.Hubs
{
    public class DraftHub : Hub
    {
        private readonly IBoosterCreator boosterCreator;
        private readonly ICardParser cardParser;
        private readonly Dictionary<string, string> connections = new Dictionary<string, string>();
        private readonly IDraftPlayerRepository draftPlayerRepository;
        private readonly IDraftSessionRepository draftSessionRepository;
        private readonly ILogger logger;

        public DraftHub(IBoosterCreator boosterCreator, IDraftPlayerRepository draftPlayerRepository,
            IDraftSessionRepository draftSessionRepository, ILogger logger, ICardParser cardParser)
        {
            this.boosterCreator = boosterCreator;
            this.draftPlayerRepository = draftPlayerRepository;
            this.draftSessionRepository = draftSessionRepository;
            this.logger = logger;
            this.cardParser = cardParser;
        }

        public async Task GetAvailableSets()
        {
            try
            {
                var sets = await cardParser.GetSets();
                Clients.Caller.SetSets(sets.Select(x => new {code = x.Key, name = x.Value.Name}));
            }
            catch (Exception e)
            {
                var errorMessage = $"{nameof(GetAvailableSets)} failed";
                logger.Error(errorMessage, e);
                throw new HubException(errorMessage);
            }
        }

        public async Task CreateDraft(string setCode)
        {
            try
            {
                var draftId = Guid.NewGuid().ToString().GenerateHash();
                var playerId = Guid.NewGuid().ToString().GenerateHash();
                var draftSession = new DraftSession {Id = draftId, SetCode = setCode};
                var draftPlayer = new DraftPlayer {DraftSessionId = draftSession.Id, Id = playerId};
                await draftSessionRepository.Save(draftSession);
                await draftPlayerRepository.Save(draftPlayer);
                await JoinGroup(draftId);
                var boosters = await boosterCreator.CreateBoosters(setCode, 3);
                connections.Add(draftPlayer.Id, Context.ConnectionId);
                Clients.Caller.InitializeGame(new {draftId, playerId, boosters});
            }
            catch (Exception e)
            {
                var errorMessage = $"{nameof(CreateDraft)} failed for {nameof(setCode)} '{setCode}'";
                logger.Error(errorMessage, e);
                throw new HubException(errorMessage);
            }
        }

        public async Task JoinDraft(string draftId)
        {
            try
            {
                var draftSession = await draftSessionRepository.Get(draftId);
                if (draftSession == null) return;
                var playerId = Guid.NewGuid().ToString().GenerateHash();
                var draftPlayer = new DraftPlayer {DraftSessionId = draftSession.Id, Id = playerId};
                await draftPlayerRepository.Save(draftPlayer);
                var boosters = await boosterCreator.CreateBoosters(draftSession.SetCode, 3);
                Clients.Caller.InitializeGame(new {draftId, playerId, boosters});
            }
            catch (Exception e)
            {
                var errorMessage = $"{nameof(JoinDraft)} failed for {nameof(draftId)} '{draftId}'";
                logger.Error(errorMessage, e);
                throw new HubException(errorMessage);
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
                var errorMessage =
                    $"{nameof(PassOnBooster)} failed for {nameof(playerId)} '{playerId}' and {nameof(draftId)} '{draftId}'";
                logger.Error(
                    errorMessage,
                    e);
                throw new HubException(errorMessage);
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
                var errorMessage = $"{nameof(StartDraft)} failed for {nameof(draftId)} '{draftId}'";
                logger.Error(errorMessage, e);
                throw new HubException(errorMessage);
            }
        }

        private Task JoinGroup(string group)
        {
            return Groups.Add(Context.ConnectionId, group);
        }
    }
}