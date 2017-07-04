using System;
using AntData.ORM.DbEngine;
using AntData.ORM.DbEngine.RW;

namespace AntData.ORM.Common.Util
{
    public class RWSplittingManager
    {
        private static readonly Object obj = new Object();

        private static IReadWriteSplitting instance = null;

        public static IReadWriteSplitting Instance
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
                               
                                Type rwType = DALBootstrap.Instance.GetRWSplittingType();
                                if (rwType == null)
                                {
                                    instance = new DefaultReadWriteSplitting();
                                }
                                else
                                {
                                    instance = Activator.CreateInstance(rwType) as IReadWriteSplitting;
                                }
                            }
                            catch
                            {
                                instance = new DefaultReadWriteSplitting();
                            }
                        }
                    }
                }

                return instance;
            }
        }

    }
}