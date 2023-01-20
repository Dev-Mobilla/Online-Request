using MySql.Data.MySqlClient;
using System;

namespace OnlineRequestSystem.Models
{
    public class ORtoMySql
    {
        private MySqlConnection connection;
        private Boolean pool = false;
        private String pathTicket;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ORtoMySql));

        public ORtoMySql()
        {
            ConnDB();
        }

        private void ConnDB()
        {
            try
            {
                pathTicket = "C:\\OnlineRequestConfig\\ORConfig.ini";
                IniFile ini = new IniFile(Path);

                String ServDomestic = ini.IniReadValue("ORtoMySQL", "Server");
                String DBDomestic = ini.IniReadValue("ORtoMySQL", "Database");
                String UIDDomestic = ini.IniReadValue("ORtoMySQL", "UID");
                String PasswordDomestic = ini.IniReadValue("ORtoMySQL", "Password");
                String poolDomestic = ini.IniReadValue("ORtoMySQL", "Pool");
                Int32 DBTimeoutDom = Convert.ToInt32(ini.IniReadValue("ORtoMySQL", "DBTimeOut"));
                Int32 maxconDomestic = Convert.ToInt32(ini.IniReadValue("ORtoMySQL", "MaxCon"));
                Int32 minconDomestic = Convert.ToInt32(ini.IniReadValue("ORtoMySQL", "MinCon"));
                Int32 toutDomestic = Convert.ToInt32(ini.IniReadValue("ORtoMySQL", "Tout"));
                Initialize(ServDomestic, DBDomestic, UIDDomestic, PasswordDomestic, poolDomestic, maxconDomestic, minconDomestic, toutDomestic, DBTimeoutDom);

                log4net.Config.XmlConfigurator.Configure();
            }
            catch (Exception ex)
            {
                log.Fatal(ex.ToString());
                throw new Exception(ex.Message);
            }
        }

        private void Initialize(String Serv, String DB, String UID, String Password, String pooling, Int32 maxcon, Int32 mincon, Int32 tout, Int32 DBTimeOut)
        {
            try
            {
                if (pooling.Equals("1"))
                {
                    pool = true;
                }
                string myconstring = "server = " + Serv + "; database = " + DB + "; uid = " + UID + ";password= " + Password + "; pooling=" + pool + ";min pool size=" + mincon + ";default command timeout=" + DBTimeOut + ";max pool size=" + maxcon + ";connection lifetime=0; connection timeout=" + tout + ";Allow Zero Datetime=true";
                connection = new MySqlConnection(myconstring);
            }
            catch (Exception ex)
            {
                log.Fatal("Unable to connect", ex);
                throw new Exception(ex.Message);
            }
        }

        public String Path
        {
            get { return pathTicket; }
            set { pathTicket = value; }
        }

        public bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
        }

        public bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
        }

        public MySqlConnection getConnection()
        {
            return connection;
        }

        public void dispose()
        {
            connection.Dispose();
        }
    }
}