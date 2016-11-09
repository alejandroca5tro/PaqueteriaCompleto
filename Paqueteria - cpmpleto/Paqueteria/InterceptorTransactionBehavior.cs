using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Paqueteria
{
    public class InterceptorTransactionBehavior : IInterceptionBehavior
    {
        public InterceptorTransactionBehavior()
        {
        }

        public virtual IMethodReturn Invoke(IMethodInvocation input,
          GetNextInterceptionBehaviorDelegate getNext)
        {
            IMethodReturn result;
            if (PaqDB.connection == null)
            {
                using (var txScope = new TransactionScope())
                {
                    using (SqlConnection sqlConnection
                        = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PaqDB"].ToString()))
                    {
                        sqlConnection.Open();
                        PaqDB.connection = sqlConnection;
                        result = getNext()(input, getNext);

                        if (result.Exception == null)
                        {
                            txScope.Complete();
                        }
                        else
                        {
                            txScope.Dispose();
                        }
                    }
                }
                PaqDB.connection = null;
            }
            else
            {
                result = getNext()(input, getNext);
            }
            return result;
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
