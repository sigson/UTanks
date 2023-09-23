using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UTanksServer.Core;
using UTanksServer.ECS.ECSCore;
//using UTanksServer.Protocol.Commands;
using UTanksServer.Services.Servers.Game.Connection;

namespace UTanksServer.Services.Servers.Game
{
    // public static class PlayerExtensions {
    // 	public static void SharePlayers(this Player player, params Player[] toShare) => SharePlayers(player, (IEnumerable<Player>)toShare);
    // 	public static void SharePlayers(this Player player, IEnumerable<Player> toShare) => player.Player.SharePlayers(toShare);
    //
    // 	public static void UnsharePlayers(this Player player, params Player[] toUnshare) =>
    // 		UnsharePlayers(player, (IEnumerable<Player>)toUnshare);
    //
    // 	public static void UnsharePlayers(this Player player, IEnumerable<Player> toUnshare) => player.Player.UnsharePlayers(toUnshare);
    //
    // 	public static void ShareEntities(this Player player, params Entity[] entities) => ShareEntities(player, (IEnumerable<Entity>)entities);
    // 	public static void ShareEntities(this Player player, IEnumerable<Entity> entities) => player.Player.ShareEntities(entities);
    //
    // 	public static void UnshareEntities(this Player player, params Entity[] entities) =>
    // 		UnshareEntities(player, (IEnumerable<Entity>)entities);
    //
    // 	public static void UnshareEntities(this Player player, IEnumerable<Entity> entities) => player.Player.UnshareEntities(entities);
    //
    // 	public static void SendEvent(this Player player, Event @event, params Entity[] entities) => player.Player.SendEvent(@event, entities);
    // }

    /// <summary>
    /// This class represents connected player and its state
    /// </summary>
    public class Player
    {
        public bool IsActive;
        public PlayerData? Data { get; }
        public bool IsLoggedIn;
        public IPlayerConnection Connection { get; set; }

        public ConcurrentDictionary<long, ECSEntity> Entities { get; }
        public List<Player> SharedPlayers { get; }

        public ECSEntity ClientSession { get; set; }
        public ECSEntity UserEntity { get; set; }

        public Player()
        {
            Entities = new ConcurrentDictionary<long, ECSEntity>();
            SharedPlayers = new List<Player>();
        }

        public Player(Server serverConnection, Socket socket)
        {
            Entities = new ConcurrentDictionary<long, ECSEntity>();
            SharedPlayers = new List<Player>();
        }

        //public void SharePlayers(params Player[] players) => SharePlayers((IEnumerable<Player>)players);

        //public void SharePlayers(IEnumerable<Player> players) {
        //  foreach(Player player in players) {
        //    if(player.UserEntity == UserEntity) throw new ArgumentException("Self player cannot be shared.");

        //    if(player.Connection.IsConnected) {
        //      SharedPlayers.Add(player);
        //      //ShareEntities(player.UserEntity);
        //    }
        //  }
        //}

        //public void UnsharePlayers(params Player[] players) => UnsharePlayers((IEnumerable<Player>)players);

        //public void UnsharePlayers(IEnumerable<Player> players) {
        //  foreach(Player player in players) {
        //    if(player.UserEntity == UserEntity) throw new ArgumentException("Self player cannot be unshared.");

        //    if(SharedPlayers.Remove(player)) {
        //      if(player.Connection.IsConnected) {
        //        //UnshareEntities(player.UserEntity);
        //      }
        //    } else {
        //      throw new InvalidOperationException($"Player {player} is not shared");
        //    }
        //  }
        //}

        //public void SendEvent(IEvent @event, params ECSEntity[] entities) {
        //  Connection.QueueCommands(new SendEventCommand(@event, entities));
        //}

        //public void ShareEntities(params ECSEntity[] entities) => ShareEntities((IEnumerable<ECSEntitys>)entities);

        //public void ShareEntities(IEnumerable<ECSEntity> entities) {
        //  foreach (ECSEntity entity in entities) {
        //    Entities.AddOrUpdate(entity.Id, (_) => entity, (_, existingEntity) => {
        //      if(entity != existingEntity) throw new InvalidOperationException($"Another entity is already shared with ID {entity.Id}");
        //      return entity;
        //    });
        //    entity.PlayerReferences.Add(this);

        //    Connection.QueueCommands(new EntityShareCommand(entity, this));
        //  }
        //}

        //public void UnshareEntities(params ECSEntity[] entities) => UnshareEntities((IEnumerable<ECSEntity>)entities);

        //public void UnshareEntities(IEnumerable<ECSEntity> entities) {
        //  foreach (ECSEntity entity in entities) {
        //    if(!Entities.TryRemove(new KeyValuePair<long, ECSEntity>(entity.Id, entity))) {
        //      throw new InvalidOperationException($"Entity {entity.Id} is not shared");
        //    }
        //    entity.PlayerReferences.Remove(this);

        //    Connection.QueueCommands(new EntityUnshareCommand(entity));
        //  }
        //}
        public void Dispose()
        {

        }

        public string LogDisplay
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                if (Data != null) builder.Append(Data.Username);
                //if(Connection is PlayerSocketConnection socketConnection) {
                //  IPEndPoint endpoint = socketConnection.Endpoint;

                //  if(Data != null) builder.Append(" (");
                //  builder.Append($"{endpoint.Address}:{endpoint.Port}");
                //  if(Data != null) builder.Append(')');
                //}

                return builder.ToString();
            }
        }

        public override string ToString() => $"Player: {LogDisplay}";
    }
}
