using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Klient.Forms
{
    //KLASA ODPOWIEDZIALNA ZA LOGOWANIE DO SERVERA
    public partial class Login : Form
    {
        private string adress = "192.168.1.17";
        private string port = "1234";
        public bool logged = false;
        public string loggedUser = "";

        public Login()
        {
            InitializeComponent();
        }

        //METODY ODPOWIEDZIALNE ZA ZALOGOWANIE DO SERVERA
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                int size = socketFd.EndReceive(ar);

                if (size > 0)
                {
                    state.m_StringBuilder.Append(Encoding.ASCII.GetString(state.m_DataBuf, 0, size));

                    socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    if (state.m_StringBuilder.Length > 1)
                    {
                        if (state.m_StringBuilder.ToString() == "Zalogowano!") //SPRAWDZENIE CZY SERVER ODPOWIEDZIAŁ POZYTYWNIE NA LOGIN
                        {
                            logged = true;
                            loggedUser = txtLogin.Text;

                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Close();
                            });
                        }
                    }
                    socketFd.Shutdown(SocketShutdown.Both);
                    socketFd.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd w ReceiveCallback: " + ex.Message.ToString());
            }
        }

        private void SendCallback(IAsyncResult ar)
        {

        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket socketFd = (Socket)ar.AsyncState;

                socketFd.EndConnect(ar);

                SocketStateObject state = new SocketStateObject();
                state.m_SocketFd = socketFd;

                SocketStateObject stateSend = new SocketStateObject();
                stateSend.m_SocketFd = socketFd;
                stateSend.m_DataBuf = Encoding.ASCII.GetBytes(txtLogin.Text);

                //WYBÓR OPCJI DZIALANIA SERVERA
                byte[] swOpt = new byte[1];
                swOpt = Encoding.ASCII.GetBytes("l");
                socketFd.Send(swOpt);

                //WYSŁANIE LOGINU DO SERVERA
                socketFd.BeginSend(stateSend.m_DataBuf, 0, stateSend.m_DataBuf.Length, 0, new AsyncCallback(SendCallback), stateSend);

                //ODBIERANIE INFORMACJI OD SERVERA
                socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd w ConnectCallback: " + ex.Message.ToString());
            }
        }

        private void GetHostEntryCallback(IAsyncResult ar)
        {
            try
            {
                IPHostEntry hostEntry = null;
                IPAddress[] addresses = null;
                Socket socketFd = null;
                IPEndPoint endPoint = null;

                /* complete the DNS query */
                hostEntry = Dns.EndGetHostEntry(ar);
                addresses = hostEntry.AddressList;


                /* create a socket */
                socketFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                /* remote endpoint for the socket */
                endPoint = new IPEndPoint(addresses[0], Int32.Parse(port));


                /* connect to the server */
                socketFd.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), socketFd);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd w GetHostEntryCallback: " + ex.Message.ToString());
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if(txtLogin.Text.Length > 0)
                {
                    Dns.BeginGetHostByName(adress, new AsyncCallback(GetHostEntryCallback), null);

                    //Close();

                    //MessageBox.Show("Zalogowano jako " + txtLogin.Text + "!");
                }
                else
                {
                    MessageBox.Show("Nie podano loginu!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd: " + ex.Message.ToString());
            }

        }
        //KONIEC METOD ODPOWIEDZIALNYCH ZA LOGOWANIE DO SERVERA

    }

    public class SocketStateObject
    {
        public const int BUF_SIZE = 1024;
        public byte[] m_DataBuf = new byte[BUF_SIZE];
        public StringBuilder m_StringBuilder = new StringBuilder();
        public Socket m_SocketFd = null;
    }

}
