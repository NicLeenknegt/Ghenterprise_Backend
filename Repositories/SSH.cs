using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using Renci.SshNet;

namespace Ghenterprise_Backend.Repositories
{
    public class SSH
    {
        public String serverIP { get; set; }
        public int serverPort { get; set; }
        public ConnectionInfo conn { get; set; }


        public SSH()
        {
            string folder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            Debug.WriteLine(folder);

            conn = new ConnectionInfo("ghenterpriseapp.westeurope.cloudapp.azure.com", 22, "jari",
                    new AuthenticationMethod[] {
                        new PrivateKeyAuthenticationMethod("jari", new []
                        {
                            new PrivateKeyFile(@"C:\ssh\Ghenterprise\private", "pazaak")
                        })
                    }
                );

        }

        public T executeQuery<T>(Func<T> executeMySqlQuery)
        {
            T t = default(T);

            try
            {

                using (SshClient client = new SshClient(conn))
                {
                    client.Connect();
                    ForwardedPortLocal port = new ForwardedPortLocal("127.0.0.1", 22, "13.94.138.165", 3306);

                    client.AddForwardedPort(port);
                    port.Start();

                    t = executeMySqlQuery();

                    client.Disconnect();
                }

                return t;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR caught in SSH.cs");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw ex;
            }

        }
    }
}