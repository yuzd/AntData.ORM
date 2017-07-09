using System;

namespace AntData.ORM.DbEngine.ConnectionString
{
    class ConnectionLocatorManager
    {
        private static readonly Object obj = new Object();

        private static IConnectionString instance = null;

        public static IConnectionString Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            try
                            {
                                Type locatorType = DALBootstrap.GetConnectionLocatorType();
                                if (locatorType == null)
                                {
                                    instance = new DefaultConnectionString();
                                }
                                else
                                {
                                    instance = Activator.CreateInstance(locatorType) as IConnectionString;
                                }
                            }
                            catch
                            {
                                instance = new DefaultConnectionString();
                            }
                        }
                    }
                }

                return instance;
            }
        }

    }
}
