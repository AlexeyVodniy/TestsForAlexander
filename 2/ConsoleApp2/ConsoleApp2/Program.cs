using System;

namespace ConsoleApp2
{
	class Program
	{
		static void Main(string[] args)
		{
			Func<string, bool> validation = (s) => int.TryParse(s, out var c) && c >= 4 && c <= 26;

			//Т.к. выше мы отважно проверили, что там точно число, тут мы можем бесстрашно парсить
			var coinsCount = int.Parse(
				AskForInput(
					"Дай количество монет:",
					"Количество имеет целочистленный тип, а не вот это вот\nА еще значение между 4 и 26 обещано было",
					validation
				)
			);
			var weighingProtocols = new string[coinsCount * (coinsCount - 1) / 2];

			//Немного позволил себе таки подрубить библиотеку, ибо руками слишком лень,
			//однако за пределы System не выходил, так что норм, наверное
			//Если не в курсе, че это, погугли "Регулярные выражения", полезная штука
			var regexForProtocols = new System.Text.RegularExpressions.Regex(@"^[a-z](>||<)[a-z]$");
			//Переписываем функцию проверки ввода
			validation = (input) => regexForProtocols.IsMatch(input);

			Console.WriteLine($"Теперь давай сюда протоколы сравнения. Всего их должно быть {weighingProtocols.Length}");
			for (int i = 0; i < weighingProtocols.Length; i++)
			{
				weighingProtocols[i] = AskForInput($"Протокол №{i + 1} из {weighingProtocols.Length}", "А обещал, корректные данные, ну-ну", validation);
			}

			Console.WriteLine($"Вот такой ответ у нас получился: {GetResult(coinsCount, weighingProtocols)}");
		}

		static string GetResult(int coinsCount, string[] weighingProtocols)
		{
			var coins = new CoinArray(coinsCount);
			for (int i = 0; i < weighingProtocols.Length; i++)
			{
				if (TryParseProtocolToRatio(weighingProtocols[i], out var lowerCoin, out var highterCoin))
				{
					if (coins[lowerCoin] == null)
					{
						coins.Add(lowerCoin);
					}
					coins[lowerCoin].HeavierCoins += highterCoin;

					if (coins[highterCoin] == null)
					{
						coins.Add(highterCoin);
					}
					coins[highterCoin].LighterCoins += lowerCoin;
				}
			}
			return coins.GetSorted();
		}

		static string AskForInput(string message, string badAnswer, Func<string, bool> validation)
		{
			//Так сказать, категорически приветствуем и спрашиваем, чё нам надо
			Console.WriteLine(message);
			var input = Console.ReadLine();

			//Реализуем защиту, проверяя введённый текст той функцией, которую сюда передали
			while (!validation(input))
			{
				//Будем крутиться в этом цикле до тех пор, пока сраный умник не введёт то, что нам надо
				//Терпения у него точно меньше, чем у бездушной машины
				Console.WriteLine($"{badAnswer}\nДавай еще раз:\n{message}");
				input = Console.ReadLine();
			}
			return input;
		}


		//Тут немного поленился и кринжа вписал, но с учетом гарантии корректности и небольшого объёма входных данных
		static bool TryParseProtocolToRatio(string protocol, out char lesserCoin, out char biggerCoin)
		{
			lesserCoin = new char();
			biggerCoin = new char();
			if (protocol.Contains('>'))
			{
				biggerCoin = protocol[0];
				lesserCoin = protocol[2];
				return true;
			}
			else if (protocol.Contains('<'))
			{
				biggerCoin = protocol[2];
				lesserCoin = protocol[0];
				return true;
			}
			return false;
		}
	}


	public class CoinArray
	{
		public class Coin
		{
			public readonly char Name;
			public string HeavierCoins { get; set; } = string.Empty;
			public string LighterCoins { get; set; } = string.Empty;

			public Coin(char name)
			{
				Name = name;
			}
		}

		public Coin this[char c] => GetCoinByName(c);
		public Coin this[int i] => Coins[i];

		private Coin[] Coins;

		public CoinArray(int count)
		{
			Coins = new Coin[count];
		}


		public void Add(char c)
		{
			for (int i = 0; i < Coins.Length; i++)
			{
				var coin = Coins[i];
				if (coin == null)
				{
					Coins[i] = new Coin(c);
					return;
				}
				else if (Coins[i].Name == c)
				{
					return;
				}
			}
		}

		/// <returns>Может вернуть null</returns>
		private Coin GetCoinByName(char name)
		{
			for (int i = 0; i < Coins.Length; i++)
			{
				var coin = Coins[i];
				if (coin != null && coin.Name == name)
				{
					return Coins[i];
				}
			}
			return null;
		}

		public string GetSorted()
		{
			var coinsRemain = Coins.Length;
			var result = string.Empty;
			while (coinsRemain > 0)
			{
				for (int i = 0; i < Coins.Length; i++)
				{
					var coin = Coins[i];
					if (coin.LighterCoins.Length == result.Length)
					{
						result += coin.Name;
						coinsRemain--;
					}
				}
			}
			return result;
		}
	}
}
