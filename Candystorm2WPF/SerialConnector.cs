using System;
using System.IO.Ports;
using System.Threading;

namespace RobotUICSharp
{
    //This class wraps a SerialPort Object for easier use 
    internal class SerialConnector
    {
        private SerialPort port;

        public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

        public event MessageReceivedEventHandler MessageReceivedEvent;
        public SerialConnector()
        {
            port = new SerialPort();
            port.BaudRate = 115200;

            port.DtrEnable = true;
            port.RtsEnable = true;
        }

        public String getPort()
        {
            return port.PortName;
        }

        // Sets the COM-Port that is to be used 
        public void setPort(String portName)
        {
            port.PortName = portName;
        }

        //Returns an Array(String) of available serial ports 
        public String[] getSerialPorts()
        {
            return SerialPort.GetPortNames();
        }

        //Closes the SerialPorts connection 
        public void shutdownPort()
        {
            if (port.IsOpen == true)
            {

                port.Close();
            }

        }

        //Opens the Serial Port. Returns true if successful. Returns false if error occurs (Port non-existent, Port in use, or similar) .
        public bool openPort()
        {
            try
            {
                port.DataReceived += new SerialDataReceivedEventHandler(ReceiveData);
                port.Open();
                Console.WriteLine("Opened Port at " + port.PortName);

                //receiveThread.Start();
                return true;
            }
            catch
            (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

        private void ReceiveData(object sender, SerialDataReceivedEventArgs e)
        {

            String received = port.ReadExisting();
            //Write the serial port data to the console.
            MessageReceivedEventArgs toInvoke = new MessageReceivedEventArgs();
            // Console.Write(received);
            toInvoke.Message = received;
            MessageReceivedEvent.Invoke(this, toInvoke);

        }



        //Returns the state of the port as a boolean. 
        public bool isOpen()
        {
            return port.IsOpen;
        }

        //Sends a message via Serial Port 
        public void sendMessage(String message)
        {
            if (port.IsOpen == true)
            {
                try
                {
                    port.WriteLine(message);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    port.Close();
                }
            }
            else
            {
                Console.WriteLine("SerialPort not open");
            }
        }


    }
}
public class MessageReceivedEventArgs : EventArgs
{

    public String Message { get; set; }

}