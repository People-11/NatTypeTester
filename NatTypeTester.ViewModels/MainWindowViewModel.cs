using DynamicData;
using DynamicData.Binding;
using NatTypeTester.Models;
using ReactiveUI;
using STUN.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace NatTypeTester.ViewModels
{
	public class MainWindowViewModel : ReactiveObject, IScreen
	{
		public RoutingState Router { get; } = new();

		public Config Config { get; }

		private readonly IEnumerable<string> _defaultServers = new HashSet<string>
		{
				@"stun.hot-chilli.net",
				@"stun.fitauto.ru",
				@"stun.internetcalls.com",
				@"stun.voip.aebc.com",
				@"stun.mixvoip.com",
				@"stun.uls.co.za",
				@"stun.epygi.com",
				@"stun.voipgate.com",
				@"stun.voipbuster.com",
				@"stun.voipstunt.com"
		};

		private SourceList<string> List { get; } = new();
		public readonly IObservableCollection<string> StunServers = new ObservableCollectionExtended<string>();

		public MainWindowViewModel(Config config)
		{
			Config = config;

			LoadStunServer();
			List.Connect()
				.DistinctValues(x => x)
				.ObserveOn(RxApp.MainThreadScheduler)
				.Bind(StunServers)
				.Subscribe();
		}

		private async void LoadStunServer()
		{
			foreach (var server in _defaultServers)
			{
				List.Add(server);
			}
			Config.StunServer = _defaultServers.First();

			const string path = @"stun.txt";

			if (!File.Exists(path))
			{
				return;
			}

			using var sw = new StreamReader(path);
			string line;
			var stun = new StunServer();
			while ((line = await sw.ReadLineAsync()) != null)
			{
				if (!string.IsNullOrWhiteSpace(line) && stun.Parse(line))
				{
					List.Add(stun.ToString());
				}
			}
		}
	}
}
