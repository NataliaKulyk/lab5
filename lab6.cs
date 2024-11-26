using System;
using System.Threading;

class Program
{
    static object locker = new object(); // Синхронізація потоків
    static bool isPart1Completed = false; // Прапор для контролю завершення першого потоку

    // Клас Timer для затримки
    class Timer
    {
        // Статичний метод для затримки в секундах
        public static void Delay(double seconds)
        {
            Thread.Sleep((int)(seconds * 1000)); // Перетворення секунд в мілісекунди
        }
    }

    // Функція для обчислення першої частини f(x) при x >= 2
    static void ComputePart1(double x)
    {
        if (x >= 2)
        {
            double result = 0.6 * x; // Обчислення першої частини функції
            Console.WriteLine($"Thread 1: f(x) = 0.6 * {x} = {result}");
            ProgressBar(100); // Показуємо прогрес
        }

        lock (locker)
        {
            isPart1Completed = true; // Встановлюємо прапор завершення
            Monitor.Pulse(locker); // Сигналізуємо другому потоку, що перший завершився
        }
    }

    // Функція для обчислення другої частини f(x) при x < 2
    static void ComputePart2(double x, int NoB)
    {
        // Затримка перед початком другого потоку
        Timer.Delay(NoB * 3.2);

        // Виконуємо обчислення для x < 2
        if (x < 2)
        {
            double result = 2 / (3 * x); // Обчислення другої частини функції
            Console.WriteLine($"Thread 2: f(x) = 2 / (3 * {x}) = {result}");
        }
        else if (x == 0)
        {
            // Обробка виключення при x = 0 (ділення на нуль)
            Console.WriteLine("Error: Division by zero is not allowed");
        }
        else
        {
            // У випадку, якщо x >= 2, ми все ще хочемо відобразити результат
            lock (locker)
            {
                while (!isPart1Completed)
                {
                    Monitor.Wait(locker);
                }
            }
            Console.WriteLine($"Thread 2: f(x) = x >= 2, no second part to calculate.");
        }

        ProgressBar(100); // Показуємо прогрес
    }

    // Текстовий прогрес-бар
    static void ProgressBar(int progress)
    {
        Console.Write("[");
        for (int i = 0; i < 50; i++)
        {
            if (i < (progress / 2))
            {
                Console.Write("#");
            }
            else
            {
                Console.Write(" ");
            }
        }
        Console.WriteLine($"] {progress}%");
    }

    static void Main(string[] args)
    {
        // Вхідне значення x для тестування
        double x = 2.5; // Ви можете змінити це значення на інше для тестування
        // Значення для змінної NoB
        int NoB = 1;

        // Створюємо потоки
        Thread thread1 = new Thread(() => ComputePart1(x));
        Thread thread2 = new Thread(() => ComputePart2(x, NoB));

        thread1.Start(); // Запуск першого потоку
        thread2.Start(); // Запуск другого потоку

        thread1.Join();  // Чекаємо завершення першого потоку
        thread2.Join();  // Чекаємо завершення другого потоку
    }
}
