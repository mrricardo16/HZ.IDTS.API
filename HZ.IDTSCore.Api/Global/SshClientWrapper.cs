using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api
{
    public class SshClientWrapper
    {
        private SshClient sshClient;

        public SshClientWrapper(string host, string username, string password)
        {
            sshClient = new SshClient(host, username, password);
            sshClient.Connect();
        }

        public void RunCommand(string command)
        {
            var sshCommand = sshClient.CreateCommand(command);
            var result = sshCommand.Execute();
            Console.WriteLine(result);
        }

        public void Disconnect()
        {
            sshClient.Disconnect();
        }

        public void DbBackUp()
        {
            var sshClient = new SshClientWrapper("your_pgsql_server_ip", "username", "password");
            sshClient.RunCommand("pg_dump -h localhost -U user -F p -f /path/to/your/output/file.sql dbname");
            sshClient.Disconnect();
        }
    }
}
