using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

public class NamedPipeClient
{
    private string pipeName;
    private bool isConnected;
    private NamedPipeClientStream pipeClient;

    public NamedPipeClient(string pipeName)
    {
        this.pipeName = pipeName;
        this.isConnected = false;
    }

    public void Connect()
    {
        while (!isConnected)
        {
            try
            {
                pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
                pipeClient.Connect();
                isConnected = true;
                Console.WriteLine("Connected to server.");

                // Start reading data from the server in a separate thread
                ThreadPool.QueueUserWorkItem(ReceiveDataFromServer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection failed. Retrying in 5 seconds...");
                Thread.Sleep(5000);
            }
        }
    }

    public void SendDataToServer(string data)
    {
        if (!isConnected)
        {
            Console.WriteLine("Not connected to server.");
            return;
        }

        try
        {
            StreamWriter writer = new StreamWriter(pipeClient);
            writer.WriteLine(data);
            writer.Flush();
        }
        catch (IOException)
        {
            // If writing to the pipe fails, assume connection is lost
            isConnected = false;
            pipeClient.Close();
            Console.WriteLine("Connection lost. Retrying...");
            Connect(); // Attempt to reconnect
        }
    }

    private void ReceiveDataFromServer(object state)
    {
        try
        {
            StreamReader reader = new StreamReader(pipeClient);
            while (isConnected)
            {
                string data = reader.ReadLine();
                if (data != null)
                {
                    Console.WriteLine("Received data from server: " + data);
                }
            }
        }
        catch (IOException)
        {
            // If reading from the pipe fails, assume connection is lost
            isConnected = false;
            pipeClient.Close();
            Console.WriteLine("Connection lost. Retrying...");
            Connect(); // Attempt to reconnect
        }
    }
}
