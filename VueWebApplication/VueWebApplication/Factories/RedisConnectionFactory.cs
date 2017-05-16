using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace VueWebApplication.Factories
{
    public static class RedisConnectionFactory
    {
        private static readonly Lazy<ConnectionMultiplexer> _conn;

        public static ConnectionMultiplexer GetConnection() => _conn.Value;

        static RedisConnectionFactory()
        {
            try
            {
                var connectionString = System.Configuration.ConfigurationManager.AppSettings["RedisConnection"];
                var options = ConfigurationOptions.Parse(connectionString);
                _conn = new Lazy<ConnectionMultiplexer>(
                    () => ConnectionMultiplexer.Connect(options),
                    LazyThreadSafetyMode.PublicationOnly);

                //conn.ConnectionFailed += Conn_ConnectionFailed;
                //conn.ConnectionRestored += Conn_ConnectionRestored;
                //conn.ErrorMessage += Conn_ErrorMessage;
                //conn.InternalError += Conn_InternalError;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Redis Event Handlers

        private static void Conn_ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            //TODO
            string errorMsg = string.Join(Environment.NewLine, e.ConnectionType, e.EndPoint, e.FailureType, e.Exception);
            throw new Exception(errorMsg);
        }

        private static void Conn_ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            //TODO
            string errorMsg = string.Join(Environment.NewLine, e.ConnectionType, e.EndPoint, e.FailureType, e.Exception);
            //throw new Exception(errorMsg);
        }

        private static void Conn_ErrorMessage(object sender, RedisErrorEventArgs e)
        {
            //TODO
            string errorMsg = string.Join(Environment.NewLine, e.EndPoint, e.Message);
            throw new Exception(errorMsg);
        }

        private static void Conn_InternalError(object sender, InternalErrorEventArgs e)
        {
            //TODO
            string errorMsg = string.Join(Environment.NewLine, e.ConnectionType, e.EndPoint, e.Exception);
            throw new Exception(errorMsg);
        }

        #endregion
    }
}