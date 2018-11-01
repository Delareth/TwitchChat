using IrcDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Immutable;

namespace Chat
{
  class MainController
  {
		static private bool bIsChannelJoined = false;
		static StandardIrcClient client = new StandardIrcClient();

		public static void ConnectToTwitch()
		{
			Random rnd = new Random();
			string s = "justinfan" + rnd.Next(100000000);
			IrcRegistrationInfo regInfo = new IrcUserRegistrationInfo()
			{
				NickName = s,
				UserName = s,
				Password = "blah",
				RealName = s
			};

			client.Connect("irc.twitch.tv", 6667, false, regInfo);
			client.RawMessageReceived += RawMessageReceived;
		}

		public static bool CheckConnect()
		{
			if (client.IsConnected && !bIsChannelJoined)
			{
				bIsChannelJoined = true;
				client.Channels.Join("#" + MainWindow.channelName);
				Console.WriteLine("Channel joined");
			}

			return client.IsConnected;
		}

		public static void SendTwitchCommands()
		{
			//client.SendRawMessage("CAP REQ :twitch.tv/commands");
			client.SendRawMessage("CAP REQ :twitch.tv/tags");
			//client.SendRawMessage("CAP REQ :twitch.tv/membership");
		}

		private static void RawMessageReceived(object sender, IrcRawMessageEventArgs e)
		{
			//Console.WriteLine(e.RawContent);

			//string sMess = e.Message.ToString();
			//string[] sType = e.Message.ToString().Split(' ');

			// :exelure!exelure@exelure.tmi.twitch.tv PRIVMSG #scr13m :123
			// :scr13m!scr13m@scr13m.tmi.twitch.tv PRIVMSG #scr13m :eto test message
			// @badges=;color=;display-name=destlany;emotes=;flags=;id=8101b045-c926-4ae5-8dd8-9b2e0ec4471c;mod=0;room-id=54733597;subscriber=0;tmi-sent-ts=1539561451067;turbo=0;user-id=265936266;user-type= :destlany!destlany@destlany.tmi.twitch.tv PRIVMSG #scr13m :234
			// @badges =; color =; display - name = frizzoloooox; emotes =; flags = 72 - 76:P.6; id = 78c459b8 - f72d - 4303 - a54c - 7132bc7151c3; mod = 0; room - id = 54733597; subscriber = 0; tmi - sent - ts = 1539608563178; turbo = 0; user - id = 266082121; user - type = :frizzoloooox!frizzoloooox@frizzoloooox.tmi.twitch.tv PRIVMSG #scr13m :Чтоб получить маверик g1 он с рокет пасса. А рокет пас стоит 10 ключей. Нихуя не пачка сигарет
			// @badges=moderator/1;color=;display-name=A13meOne;emote-only=1;emotes=12:0-1;flags=;id=3d7e2c29-846e-4682-9c8d-e8e39137b5bb;mod=1;room-id=54733597;subscriber=0;tmi-sent-ts=1539614694489;turbo=0;user-id=263194012;user-type=mod :a13meone!a13meone@a13meone.tmi.twitch.tv PRIVMSG #scr13m ::P
			if (e.RawContent[0] == '@')
			{
				if (e.RawContent.Contains("PRIVMSG"))
				{
					string sNick = e.RawContent.Split(';')[2].Split('=')[1];
					if (sNick[0] == ' ')
					{
						Console.WriteLine("NEW PARSING: Removed ' ' from nick: '{0}'", sNick);
						sNick = sNick.Remove(0, 1);
					}

					string searchIndex = "#" + MainWindow.channelName + " :";
					int index = e.RawContent.IndexOf(searchIndex);
					string gettedText = e.RawContent.Remove(0, index + searchIndex.Length);
					//Console.WriteLine("NEW PARSING: " + sNick + ": " + gettedText);

					MainWindow.RunInUiThread(() =>
					{
						MainWindow.ExecuteJS("addMessage('" + sNick + "', '" + gettedText + "')");
					});
				}
			}
		}

		public static void ConnectToDonate()
		{
			Console.WriteLine("Donate connection started..");
			JObject json = new JObject
			{
				["token"] = "nRpBnf2FsCMP7xRCNKCz",
				["type"] = "minor"
			};

			var socket = IO.Socket("http://socket.donationalerts.ru:3001", new IO.Options
			{
				Transports = ImmutableList.Create<string>().Add("websocket"),
				Reconnection = true,
				ReconnectionDelay = 500L,
				ReconnectionDelayMax = 2000L,
				ReconnectionAttempts = 2147483647
			});

			socket.On(Socket.EVENT_CONNECT, () =>
			{
				Console.WriteLine("Donate try to emit connect..");
				socket.Emit("add-user", JObject.FromObject(json));
				Console.WriteLine("Donate socket connected!");
			});

			socket.On("donation", (data) =>
			{
				dynamic results = JsonConvert.DeserializeObject(data.ToString());
				Console.WriteLine(results);

				if (results["alert_type"] == "1")
				{
					string nickname = results["username"];
					string amount = results["amount"];
					string currency = results["currency"];
					string text = results["message"];

					Console.WriteLine("{0} donated - {1} ({2}): {3}", nickname, amount, currency, text);

					MainWindow.RunInUiThread(() =>
					{
						MainWindow.ExecuteJS("addDonate('" + nickname + "', '" + text + "', '" + amount + "', '" + currency + "')");
					});
				}
			});
		}

		public static void ConnectToFollowers()
		{
			Console.WriteLine("Followers connections started..");
			JObject json = new JObject
			{
				["method"] = "jwt",
				["token"] = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyIjoiNWJhODEzZTVhOThkYWY0Mzg2ZThlOTY4IiwiY2hhbm5lbCI6IjViYTgxM2U1YTk4ZGFmMWYzMGU4ZTk2OSIsInByb3ZpZGVyIjoidHdpdGNoIiwicm9sZSI6Im93bmVyIiwiYXV0aFRva2VuIjoiSy10MHNrVjQtdDRiNUFnbEtTLWdhSlUzSDI2MnBENGdhbVdxYXNpajZkT1NYVXo3IiwiaWF0IjoxNTM5NDQ2MDQ4LCJpc3MiOiJTdHJlYW1FbGVtZW50cyJ9.GWoEXPxmcfasygkGHT3nqYGrgtoI8fVUii0r9x-Fvwk"
			};

			var socket = IO.Socket("https://realtime.streamelements.com", new IO.Options
			{
				Transports = ImmutableList.Create<string>().Add("websocket"),
				Reconnection = true,
				ReconnectionDelay = 500L,
				ReconnectionDelayMax = 2000L,
				ReconnectionAttempts = 2147483647
			});

			socket.On(Socket.EVENT_CONNECT, () =>
			{
				Console.WriteLine("Followers try to emit connect..");
				socket.Emit("authenticate", JObject.FromObject(json));
				Console.WriteLine("Followers socket connected!");
			});

			//{
			//	"provider": "twitch",
			//	"_id": "5bc4afb17b88b529d5424d77",
			//	"channel": "5ba813e5a98daf1f30e8e969",
			//	"providerId": "",
			//	"type": "follow",
			//	"data": {
			//		"username": "ro9ergreed",
			//		"avatar": "https://static-cdn.jtvnw.net/user-default-pictures/bb97f7e6-f11a-4194-9708-52bf5a5125e8-profile_image-300x300.jpg"
			//	},
			//	"createdAt": "2018-10-15T15:18:09.609Z"
			//}

			socket.On("event", (data) =>
			{
				dynamic results = JsonConvert.DeserializeObject(data.ToString());
				Console.WriteLine(results);

				if(results["type"] == "follow")
				{
					string follower = results["data"]["username"];

					MainWindow.RunInUiThread(() =>
					{
						MainWindow.ExecuteJS("addFollower('" + follower + "')");
					});
				}
			});

			socket.On("authenticated", (data) =>
			{
				Console.WriteLine("Successfully connected to:");
				Console.WriteLine(data);
			});
		}
	}
}
