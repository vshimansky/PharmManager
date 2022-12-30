using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool ShouldContinue = DbHelper.OpenConnection();
            if (!ShouldContinue)
                ShowDbError(true);
            char input;

            while(ShouldContinue)
            {
                Console.Clear();
                ShowMainPromt();
                input = Console.ReadKey().KeyChar;
                if (input == '0')
                    ShouldContinue = false;
                else if (input == '1')
                    ShouldContinue = ManageGoods();
                else if (input == '2')
                    ShouldContinue = ManagePharms();
                else if (input == '3')
                    ShouldContinue = ManagStockss();
                else if (input == '4')
                    ShouldContinue = ManagBatches();
            }
        }
        static void ShowMainPromt()
        {
            Console.WriteLine("Введите номер для выбора режима");
            Console.WriteLine("1 - работа с товарами");
            Console.WriteLine("2 - работа с аптеками");
            Console.WriteLine("3 - работа со складами");
            Console.WriteLine("4 - работа с партиями товаров");
            Console.WriteLine("0 - выход из приложения");
        }
        #region Товары
        static bool ManageGoods()
        {
            bool ShouldContinue = true;
            Console.Clear();
            char input = '9';
            while (ShouldContinue)
            {
                ShowGoodsPromt();
                input = Console.ReadKey().KeyChar;
                if (input == '0' || input == '4')
                    ShouldContinue = false;
                else if (input == '1')
                    AddGood();
                else if (input == '2')
                    DeleteGood();
                else if (input == '3')
                    ListGood();
            }
            return input != '0';
        }
        static void ShowGoodsPromt()
        {
            Console.WriteLine("Работа с товарами");
            Console.WriteLine("Введите номер для выбора действия");
            Console.WriteLine("1 - добавить товар");
            Console.WriteLine("2 - удалить товар");
            Console.WriteLine("3 - список товаров");
            Console.WriteLine("4 - выход в главное меню");
            Console.WriteLine("0 - выход из программы");
        }
        static void AddGood()
        {
            Console.WriteLine();
            Console.Write("Введите название товара: ");
            string name = Console.ReadLine();
            Console.Clear();
            if (DbHelper.AddGood(name))
                Console.WriteLine("Товар успешно добавлен");
            else
                ShowDbError();
            Console.WriteLine();
        }
        static void DeleteGood()
        {
            Console.WriteLine();
            Console.Write("Введите код товара: ");
            string id = Console.ReadLine();
            id = GetDigitalInput(id, "товара");
            Console.Clear();
            if (DbHelper.DeleteGood(id))
                Console.WriteLine("Товар успешно удален");
            else
                ShowDbError();
            Console.WriteLine();
        }
        static void ListGood()
        {
            Console.Clear();
            Console.Write(DbHelper.ListGoods());
            Console.WriteLine();
        }
        #endregion
        #region Аптеки
        static bool ManagePharms()
        {
            bool ShouldContinue = true;
            Console.Clear();
            char input = '9';
            while (ShouldContinue)
            {
                ShowPharmPromt();
                input = Console.ReadKey().KeyChar;
                if (input == '0' || input == '5')
                    ShouldContinue = false;
                else if (input == '1')
                    AddPharm();
                else if (input == '2')
                    DeletePharm();
                else if (input == '3')
                    ListPharmacies();
                else if (input == '4')
                    ListPharmGoods();
            }
            return input != '0';
        }
        static void ShowPharmPromt()
        {
            Console.WriteLine("Работа с аптеками");
            Console.WriteLine("Введите номер для выбора действия");
            Console.WriteLine("1 - добавить аптеку");
            Console.WriteLine("2 - удалить аптеку");
            Console.WriteLine("3 - список аптек");
            Console.WriteLine("4 - список товаров в аптеке");
            Console.WriteLine("5 - выход в главное меню");
            Console.WriteLine("0 - выход из программы");
        }
        static void AddPharm()
        {
            Console.WriteLine();
            Console.Write("Введите название аптеки: ");
            string name = Console.ReadLine();
            Console.Write("Введите адрес аптеки: ");
            string address = Console.ReadLine();
            Console.Write("Введите телефон аптеки: ");
            string phone = Console.ReadLine();
            Console.Clear();
            if (DbHelper.AddPharmacy(name, address, phone))
                Console.WriteLine("Аптека успешно добавлена");
            else
                ShowDbError();
            Console.WriteLine();
        }
        static void DeletePharm()
        {
            Console.WriteLine();
            Console.Write("Введите код аптеки: ");
            string id = Console.ReadLine();
            id = GetDigitalInput(id, "аптеки");
            Console.Clear();
            if (DbHelper.DeletePharmacy(id))
                Console.WriteLine("Аптека успешно удален");
            else
                ShowDbError();
            Console.WriteLine();
        }
        static void ListPharmacies()
        {
            Console.Clear();
            Console.Write(DbHelper.ListPharmacies());
            Console.WriteLine();
        }
        static void ListPharmGoods()
        {
            Console.WriteLine();
            Console.Write("Введите код аптеки: ");
            string id = Console.ReadLine();
            id = GetDigitalInput(id, "аптеки");
            Console.Clear();
            Console.Write(DbHelper.ListPharmGoods(id));
            Console.WriteLine();
        }
        #endregion
        #region Склады
        static bool ManagStockss()
        {
            bool ShouldContinue = true;
            Console.Clear();
            char input = '9';
            while (ShouldContinue)
            {
                ShowStockPromt();
                input = Console.ReadKey().KeyChar;
                if (input == '0' || input == '4')
                    ShouldContinue = false;
                else if (input == '1')
                    AddStock();
                else if (input == '2')
                    DeleteStock();
                else if (input == '3')
                    ListStocks();
            }
            return input != '0';
        }
        static void ShowStockPromt()
        {
            Console.WriteLine("Работа со складами");
            Console.WriteLine("Введите номер для выбора действия");
            Console.WriteLine("1 - добавить склад");
            Console.WriteLine("2 - удалить склад");
            Console.WriteLine("3 - список всех складов");
            Console.WriteLine("4 - выход в главное меню");
            Console.WriteLine("0 - выход из программы");
        }
        static void AddStock()
        {
            Console.WriteLine();
            Console.Write("Введите название склада: ");
            string name = Console.ReadLine();
            Console.Write("Введите код аптеки для склада: ");
            string id = Console.ReadLine();
            id = GetDigitalInput(id, "аптеки");
            Console.Clear();
            if (DbHelper.AddStockd(name, id))
                Console.WriteLine("Товар успешно добавлен");
            else
                ShowDbError();
            Console.WriteLine();
        }
        static void DeleteStock()
        {
            Console.WriteLine();
            Console.Write("Введите код склада: ");
            string id = Console.ReadLine();
            id = GetDigitalInput(id, "склада");
            Console.Clear();
            if (DbHelper.DeleteStock(id))
                Console.WriteLine("Склад успешно удален");
            else
                ShowDbError();
            Console.WriteLine();
        }
        static void ListStocks()
        {
            Console.Clear();
            Console.Write(DbHelper.ListStocks());
            Console.WriteLine();
        }
        #endregion
        #region Партии
        static bool ManagBatches()
        {
            bool ShouldContinue = true;
            Console.Clear();
            char input = '9';
            while (ShouldContinue)
            {
                ShowBatchPromt();
                input = Console.ReadKey().KeyChar;
                if (input == '0' || input == '4')
                    ShouldContinue = false;
                else if (input == '1')
                    AddBatch();
                else if (input == '2')
                    DeleteBatch();
                else if (input == '3')
                    ListBatches();
            }
            return input != '0';
        }
        static void ShowBatchPromt()
        {
            Console.WriteLine("Работа с партиями товаров");
            Console.WriteLine("Введите номер для выбора действия");
            Console.WriteLine("1 - добавить партию товара");
            Console.WriteLine("2 - удалить партию товара");
            Console.WriteLine("3 - список всех партий");
            Console.WriteLine("4 - выход в главное меню");
            Console.WriteLine("0 - выход из программы");
        }
        static void AddBatch()
        {
            Console.WriteLine();
            Console.Write("Введите код товара в партии: ");
            string goodId = Console.ReadLine();
            goodId = GetDigitalInput(goodId, "товара");
            Console.Write("Введите код склада для партии: ");
            string stockId = Console.ReadLine();
            stockId = GetDigitalInput(stockId, "склада");
            Console.Write("Введите количество товара в партии: ");
            string count = Console.ReadLine();
            count = GetDigitalInput(count, "", "Кол-во должно выражаться целым числом: ");
            Console.Clear();
            if (DbHelper.AddBatch(stockId, goodId, count))
                Console.WriteLine("Партия успешно добавлена");
            else
                ShowDbError();
            Console.WriteLine();
        }
        static void DeleteBatch()
        {
            Console.WriteLine();
            Console.Write("Введите код партии: ");
            string id = Console.ReadLine();
            id = GetDigitalInput(id, "партии");
            Console.Clear();
            if (DbHelper.DeleteBatch(id))
                Console.WriteLine("Партия успешно удалена");
            else
                ShowDbError();
            Console.WriteLine();
        }
        static void ListBatches()
        {
            Console.Clear();
            Console.Write(DbHelper.ListBatches());
            Console.WriteLine();
        }
        #endregion
        #region Вспомогателные методы
        static void ShowDbError(bool waitReact = false)
        {
            Console.WriteLine("В работе приложения произошла ошибка:");
            Console.WriteLine(DbHelper.DbError);
            Console.WriteLine();
            if (waitReact)
            {
                Console.WriteLine("Для завершения работы нажмите любую клавишу");
                Console.Read();
            }
        }
        static string GetDigitalInput(string input, string type, string message = "")
        {
            string result = input;
            while(String.IsNullOrWhiteSpace(result) || result.Any(x => !char.IsDigit(x)))
            {
                if (String.IsNullOrWhiteSpace(message))
                    Console.Write($"Введите код {type} (должен быть числом):");
                else
                    Console.Write(message);
                result = Console.ReadLine().Trim();
            }
            return result;
        }
        #endregion
    }
}
