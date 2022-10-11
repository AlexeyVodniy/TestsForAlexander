using System;

namespace Sample1
{
	//enum очень удобная штука, когда нам надо сделать переменную,
	//способную принимать только ряд конкретных значений
	//В нашем случае, это четыре возможных варианта движения дворника
	enum MoveOrderType
	{
		AscendingEven,
		DescendingEven,
		AscendingOdd,
		DescendingOdd
	}

	class Program
	{
		static void Main(string[] args)
		{
			//Первая же строчка одна из самых сложных))
			//Но она важна. Это функция, проверяющая,
			//является ли введённый текст числом. Вернёт true, если да и false, если там написано чё попало
			Func<string, bool> validation = (input) => int.TryParse(input, out var _);

			//Передаём первый вопрос и функцию проверки в метод, внутри которого уже их реаолизуем
			//И оттуда получаем уже сразу количество домов в числовом формате, которое записываем в отдельную переменную
			var housesCount = int.Parse(AskForInput("Какое число домов?", "Вопрос не о чистоте твоей попы, умник", validation));

			//Тут собираем в массив все номера домов, с которых мы теоретически можем начать
			var possibleStartIndexes = new int[] { 1, 2, housesCount - 1, housesCount };

			//Переписываем нашу защитную функцию, чтобы она теперь еще и проверяла, 
			//Тот ли номер дома нам ввели, с которого можно начать, или опять херню
			validation = (input) => {
				if (int.TryParse(input, out var number))
				{
					for (int i = 0; i < possibleStartIndexes.Length; i++)
					{
						if (possibleStartIndexes[i] == number)
						{
							return true;
						}
					}
				}
				return false;
			};

			//Задаём второй вопрос и получаем стартовый индекс
			var startIndex = int.Parse(AskForInput(
				"С какого дома начинаем?",
				$"Стартовым домом может быть только крайний, а их всего четыре: 1, 2, {housesCount - 1}, {housesCount}",
				validation));

			//Получаем из метода готовый ответ, записываем его в сроку и выдаём пользователю
			var result = GetFinalResult(housesCount, startIndex);
			var stringResult = string.Empty;
			for (int i = 0; i < result.Length; i++)
			{
				stringResult += $"{result[i]}, ";
			}
			Console.WriteLine($"Вот такой ответ у нас получился: {stringResult}");
			Console.ReadLine();
		}

		static int[] GetFinalResult(int housesCount, int startIndex)
		{
			//По заветам предков, сперва создаём все переменные, а потом уже их крутим-вертим
			//Вспомогательные значения нужны, чтобы внутри основного кода не мусорить
			var isRoundCount = housesCount % 2 == 0;
			var halfCount = housesCount / 2;

			//Отдельные массивы для чётных и нечетных домов нужны потому
			//Что один из них мы будем проходить в обратном порядке, а другой в обычном
			var evenNumbers = new int[halfCount];
			var oddNumbers = new int[isRoundCount ? halfCount : halfCount + 1];

			//Заполняем эти массивы
			var oddIndex = 0;
			var evenIndex = 0;
			for (int i = 1; i <= housesCount; i++)
			{
				if (i % 2 == 0)
				{
					evenNumbers[evenIndex] = i;
					evenIndex++;
				}
				else
				{
					oddNumbers[oddIndex] = i;
					oddIndex++;
				}
			}

			//Высчитываем порядок прохода
			var moveOrder = GetMoveOrderType(housesCount, startIndex, isRoundCount);

			//Передаём этот порядок и наши массивы для итоговых вычислений
			//и возвращаем ответ в главный метод
			return CalculateFinalResult(moveOrder, evenNumbers, oddNumbers);
		}

		static int[] CalculateFinalResult(MoveOrderType moveOrder, int[] evens, int[] odds)
		{
			//Большой массив с итоговым результатом
			var result = new int[evens.Length + odds.Length];

			//Проверяем значение порядка прохода и в зависимости от него
			//Переворачиваем один из массивов и по-разному складываем их в массив результата
			switch (moveOrder)
			{
				case MoveOrderType.AscendingEven:
					odds.ReverseIntArray();
					for (int i = 0; i < evens.Length; i++)
					{
						result[i] = evens[i];
					}
					for (int i = 0; i < odds.Length; i++)
					{
						result[evens.Length + i] = odds[i];
					}
					break;
				case MoveOrderType.DescendingEven:
					evens.ReverseIntArray();
					for (int i = 0; i < evens.Length; i++)
					{
						result[i] = evens[i];
					}
					for (int i = 0; i < odds.Length; i++)
					{
						result[evens.Length + i] = odds[i];
					}
					break;
				case MoveOrderType.AscendingOdd:
					evens.ReverseIntArray();
					for (int i = 0; i < odds.Length; i++)
					{
						result[i] = odds[i];
					}
					for (int i = 0; i < evens.Length; i++)
					{
						result[odds.Length + i] = evens[i];
					}
					break;
				case MoveOrderType.DescendingOdd:
					odds.ReverseIntArray();
					for (int i = 0; i < odds.Length; i++)
					{
						result[i] = odds[i];
					}
					for (int i = 0; i < evens.Length; i++)
					{
						result[odds.Length + i] = evens[i];
					}
					break;
				default:
					//Обрабатываем ошибку
					throw new ArgumentOutOfRangeException("Кринжовый тип стартового движения, так не бывает");
			}
			//Отдаём результат выше по коду
			return result;
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

			//Сюда мы попадаем уже после всех проверок, так что можно спокойно напрямую парсить текст в число
			//Не волнуясь, что кто-то нам что-то сломает
			return input;
		}

		static MoveOrderType GetMoveOrderType(int housesCount, int startIndex, bool isRoundCount)
		{
			//В зависимости от номера стартового дома, понимаем, какой у нас тип стартового движения
			if (startIndex == 1)
			{
				return MoveOrderType.AscendingOdd;
			}
			else if (startIndex == 2)
			{
				return MoveOrderType.AscendingEven;
			}
			else if (startIndex == housesCount - 1)
			{
				if (isRoundCount)
				{
					return MoveOrderType.DescendingOdd;
				}
				else
				{
					return MoveOrderType.DescendingEven;
				}
			}
			else if (startIndex == housesCount)
			{
				if (isRoundCount)
				{
					return MoveOrderType.DescendingEven;
				}
				else
				{
					return MoveOrderType.DescendingOdd;
				}
			}

			//Сюда мы попадаем только если стартовый номер дома не нашелся в указанных вариантах, такого быть не должно, но учесть надо
			throw new ArgumentOutOfRangeException("Чё-то с номером не так, мб проверка где-то не отработала");
		}
	}

	public static class IntArrayExtensions
	{
		//Пришлось вручную сделать переворачивание массива
		//Ибо без подключения библиотек мы так не умеем
		//А библиотеки подрубать низя(насколько я понял задание)
		public static void ReverseIntArray(this int[] array)
		{
			var temp = array.Clone() as int[];
			for (int i = 0; i < temp.Length; i++)
			{
				array[i] = temp[^(i + 1)];
			}
		}
	}
}
