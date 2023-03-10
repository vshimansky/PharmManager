using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmManager
{
    public static class DbHelper
    {
        // SQL запросы для товаров
        private const string _sqlAddGood = @"INSERT INTO dbo.Goods (gdName) VALUES('{0}'); 
                                             SELECT @@ROWCOUNT;";
        private const string _sqlListGoods = @"SELECT gdId Код, gdName Название
                                               FROM dbo.Goods WITH(NOLOCK) ORDER BY gdName";
        private const string _sqlDeleteGood = @"BEGIN TRAN; 
                                                  DELETE FROM dbo.Batches WHERE bhGoodId = {0};
                                                  DELETE FROM dbo.Goods WHERE gdId = {0};
                                                COMMIT;";
        private const string _sqlCheckGood = "SELECT COUNT(*) FROM dbo.Goods WITH(NOLOCK) WHERE gdId={0}";
        // SQL запросы для аптек
        private const string _sqlAddPharmacy = @"INSERT INTO dbo.Pharmacies (phName, phAddress, phPhone) 
                                                 VALUES('{0}','{1}','{2}'); 
                                                 SELECT @@ROWCOUNT;";
        private const string _sqlListPharmacies = @"SELECT phId Код, phName Название, phAddress Адрес, phPhone Телефон
                                                    FROM dbo.Pharmacies WITH(NOLOCK) ORDER BY phName";
        private const string _sqlDeletePharmacy = @"BEGIN TRAN; 
                                                      DELETE FROM dbo.Batches WHERE bhStockId IN 
                                                        (SELECT stId FROM dbo.Stocks WHERE stPharmId = {0});
                                                      DELETE FROM dbo.Stocks WHERE stPharmId = {0};
                                                      DELETE FROM dbo.Pharmacies WHERE phId = {0};
                                                    COMMIT;";
        private const string _sqlListPharmGoods = @"SELECT gd.gdName Название, SUM(bh.bhCount) Количество
                                                    FROM dbo.Batches bh WITH(NOLOCK)
                                                    LEFT JOIN dbo.Goods gd WITH(NOLOCK) ON gd.gdId = bh.bhGoodId
                                                    LEFT JOIN dbo.Stocks st WITH(NOLOCK) ON st.stId = bh.bhStockId
                                                    WHERE st.stPharmId={0}
                                                    GROUP BY gd.gdId, gd.gdName;";
        private const string _sqlCheckPharmacy = "SELECT COUNT(*) FROM dbo.Pharmacies WITH(NOLOCK) WHERE phId={0}";
        // SQL запросы для складов
        private const string _sqlAddStock = @"IF EXISTS(SELECT 1 FROM dbo.Pharmacies WHERE phId = {1})
                                                INSERT INTO dbo.Stocks(stName,stPharmId)
                                                VALUES('{0}', {1});
                                              SELECT @@ROWCOUNT;";
        private const string _sqlListStocks = @"SELECT st.stId Код, st.stName Название, ph.phName Аптека
                                                FROM dbo.Stocks st WITH(NOLOCK)
                                                LEFT JOIN dbo.Pharmacies ph WITH(NOLOCK) ON ph.phId = st.stPharmId
                                                ORDER BY ph.phName, st.stName";
        private const string _sqlDeleteStock = @"BEGIN TRAN; 
                                                    DELETE FROM dbo.Batches WHERE bhStockId = {0};
                                                    DELETE FROM dbo.Stocks WHERE stId = {0};
                                                 COMMIT;";
        private const string _sqlCheckStock = "SELECT COUNT(*) FROM dbo.Stocks WITH(NOLOCK) WHERE stId={0}";
        // SQL запросы для партий
        private const string _sqlAddBatch = @"IF EXISTS(SELECT 1 FROM dbo.Stocks WHERE stId = {0})
                                                 AND EXISTS(SELECT 1 FROM dbo.Goods WHERE gdId = {1})
                                                    INSERT INTO dbo.Batches(bhStockId,bhGoodId,bhCount)
                                                    VALUES({0}, {1}, {2});
                                              SELECT @@ROWCOUNT;";
        private const string _sqlListBatches = @"SELECT bh.bhId Код, st.stName Склад, gd.gdName Товар, bh.bhCount Количествово
                                                FROM dbo.Batches bh WITH(NOLOCK)
                                                LEFT JOIN dbo.Stocks st WITH(NOLOCK) ON st.stId = bh.bhStockId
                                                LEFT JOIN dbo.Goods gd WITH(NOLOCK) ON gd.gdId = bh.bhGoodId
                                                ORDER BY st.stName, gd.gdName";
        private const string _sqlDeleteBatch = @"DELETE FROM dbo.Batches WHERE bhId = {0}";
        private const string _sqlCheckBatch = "SELECT COUNT(*) FROM dbo.Batches WITH(NOLOCK) WHERE bhId={0}";
        
        public static string DbError;
        
        private static SqlConnection _connection;
        private static SqlConnection DbConnection
        {
            get 
            {
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();
                return _connection; 
            }
        }
        #region Вспомогательные метожы
        public static bool CheckConnection()
        {
            bool result = true;
            string connectionStringt = GetConnectionString();
            if (!String.IsNullOrWhiteSpace(connectionStringt))
            {
                _connection = new SqlConnection(connectionStringt);
                try
                {
                    _connection.Open();
                }
                catch (Exception exp)
                {
                    result = false;
                    if (_connection.State == ConnectionState.Open)
                        _connection.Close();
                    DbError = exp.Message;
                }
            }
            else
                DbError = "Соединение с сервером не установлено в конфигурации";
            return result;
        }
        private static string GetConnectionString()
        {
            ConnectionStringSettings settings =
                ConfigurationManager.ConnectionStrings["Pharm"];
            return settings?.ConnectionString;
        }
        public static void CloseConnection()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
                _connection.Close();
        }
        private static bool ExecuteScalarSql(string sqlText, int CheckResult)
        {
            DbError = "";
            bool result = true;
            try
            {
                SqlCommand command = new SqlCommand(sqlText, DbConnection);
                result = ((int)command.ExecuteScalar() == CheckResult);
            }
            catch (Exception exp)
            {
                DbError = exp.Message;
                result = false;
            }
            return result;
        }
        private static bool ExecuteNonQuerySql(string sqlText)
        {
            DbError = "";
            bool result = true;
            try
            {
                SqlCommand command = new SqlCommand(sqlText, DbConnection);
                command.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                DbError = exp.Message;
                result = false;
            }
            return result;
        }
        private static string ExecuteReaderSql(string sqlText, params int[] spaces)
        {
            string result = "";
            try
            {
                SqlCommand command = new SqlCommand(sqlText, DbConnection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        List<int> spaces2 = new List<int>();
                        spaces2.AddRange(spaces);
                        if (spaces.Length < reader.FieldCount)
                            spaces2.AddRange(Enumerable.Repeat<int>(50, reader.FieldCount - spaces.Length));
                        for (int i = 0; i < reader.FieldCount; i++)
                            result += SetCorrectLength(reader.GetName(i), spaces2[i]);
                        result += Environment.NewLine;
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                                result += SetCorrectLength(reader[i].ToString(), spaces2[i]);
                            result += Environment.NewLine;
                        }
                    }
                    else
                        result = "В базе нет записей";
                }
            }
            catch (Exception exp)
            {
                result = "Произошла ошибка в приложении:" + Environment.NewLine;
                result += exp.Message;
            }
            return result;
        }
        private static string SetCorrectLength(string data, int num)
        {
            return data.Length <= num ? data.PadRight(num, ' ') : data.Substring(0, num);
        }
        #endregion
        #region Методы для товаров
        public static bool AddGood(string name) => ExecuteScalarSql(String.Format(_sqlAddGood, name), 1);
        public static string ListGoods() => ExecuteReaderSql(_sqlListGoods, 10, 50);
        public static bool DeleteGood(string id) => ExecuteNonQuerySql(String.Format(_sqlDeleteGood, id));
        public static bool CheckGood(string id) => ExecuteScalarSql(String.Format(_sqlCheckGood, id), 1);
        #endregion
        #region Методы для аптеки
        public static bool AddPharmacy(string name, string address, string phone) => ExecuteScalarSql(String.Format(_sqlAddPharmacy, name, address, phone), 1);
        public static string ListPharmacies() => ExecuteReaderSql(_sqlListPharmacies, 10, 50, 50, 20);
        public static bool DeletePharmacy(string id) => ExecuteNonQuerySql(String.Format(_sqlDeletePharmacy, id));
        public static string ListPharmGoods(string id) => ExecuteReaderSql(String.Format(_sqlListPharmGoods, id), 50, 20);
        public static bool CheckPharmacyd(string id) => ExecuteScalarSql(String.Format(_sqlCheckPharmacy, id), 1);
        #endregion
        #region Методы для складов
        public static bool AddStockd(string name, string pharmId) => ExecuteScalarSql(String.Format(_sqlAddStock, name, pharmId), 1);
        public static string ListStocks() => ExecuteReaderSql(_sqlListStocks, 10, 50, 50);
        public static bool DeleteStock(string id) => ExecuteNonQuerySql(String.Format(_sqlDeleteStock, id));
        public static bool CheckStock(string id) => ExecuteScalarSql(String.Format(_sqlCheckStock, id), 1);
        #endregion
        #region Методы для партий товаров
        public static bool AddBatch(string stockId, string goodId, string count) => ExecuteScalarSql(String.Format(_sqlAddBatch, stockId, goodId, count), 1);
        public static string ListBatches() => ExecuteReaderSql(_sqlListBatches, 10, 50, 50, 20);
        public static bool DeleteBatch(string id) => ExecuteNonQuerySql(String.Format(_sqlDeleteBatch, id));
        public static bool CheckBatch(string id) => ExecuteScalarSql(String.Format(_sqlCheckBatch, id), 1);
        #endregion
    }
}
