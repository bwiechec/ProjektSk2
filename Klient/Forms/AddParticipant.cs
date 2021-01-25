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

    //KLASA SŁUŻĄCA DO DODAWANIA NOWYCH CZŁONKÓW DO KALENDARZA

    public partial class AddParticipant : Form
    {
        string currentCalendar; //Obecnie wybrany kalendarz
        private string adress = "192.168.1.17"; //Adres ip serwera
        private string port = "1234"; //Wykorzystywany port
        public AddParticipant(string curCal) //curCal obecnie wybrany kalendarz
        {
            currentCalendar = curCal;
            InitializeComponent();
        }

        //FUNKCJE ODPOWIEDZIALNE ZA POŁĄCZENIE Z SERWEREM
        private void ConnectCallbackText(IAsyncResult ar)
        {
            try
            {
                Socket socketFd = (Socket)ar.AsyncState;

                socketFd.EndConnect(ar);

                SocketStateObject state = new SocketStateObject();
                state.m_SocketFd = socketFd;

                //Wyślij opcję działania do servera
                byte[] swOpt = new byte[1];
                swOpt = Encoding.ASCII.GetBytes("p");
                socketFd.Send(swOpt);

                //Wyślij obecnie używany kalendarz do serwera
                byte[] curCalendar = Encoding.ASCII.GetBytes(currentCalendar);
                socketFd.Send(curCalendar);

                //Utworzenia bufora do odebrania od serwera informacji, ze odebrał nazwe kalendarza
                byte[] buffer = new byte[256];
                buffer = Encoding.ASCII.GetBytes("");

                //Czekanie na odpowiedź od serwera, czy odebrał nazwę kalendarza
                socketFd.Receive(buffer);
                while (buffer == Encoding.ASCII.GetBytes(""))
                {
                    socketFd.Receive(buffer);
                }

                //Wysłanie do serwera nazwy usera do dodania
                byte[] userToAdd = Encoding.ASCII.GetBytes(txtParticipant.Text.ToString());
                socketFd.Send(userToAdd);


            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd w ConnectCallback: " + ex.Message.ToString());
            }
        }


        private void GetHostEntryCallbackText(IAsyncResult ar)
        {
            try
            {
                IPHostEntry hostEntry = null;
                IPAddress[] addresses = null;
                Socket socketFd = null;
                IPEndPoint endPoint = null;

                hostEntry = Dns.EndGetHostEntry(ar);
                addresses = hostEntry.AddressList;

                socketFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                endPoint = new IPEndPoint(addresses[0], Int32.Parse(port));

                socketFd.BeginConnect(endPoint, new AsyncCallback(ConnectCallbackText), socketFd);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd w GetHostEntryCallback: " + ex.Message.ToString());
            }
        }

        //Funkcja odpowiedzialna za akcję po kliknięciu przycisku "dodaj"
        private void btnAdd_Click(object sender, EventArgs e)
        {
            //txtParticipant.Text.ToString()
            //POLACENIE Z LINUXEM I WYSLANIE NAZWY USERA
            Dns.BeginGetHostByName(adress, new AsyncCallback(GetHostEntryCallbackText), null);

            Close();
        }


        //Funkcja odpowiedzialna za akcję po kliknięciu przycisku "wróć"
        private void btnDel_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        
        public class SocketStateObject
        {
            public const int BUF_SIZE = 1024;
            public byte[] m_DataBuf = new byte[BUF_SIZE];
            public StringBuilder m_StringBuilder = new StringBuilder();
            public Socket m_SocketFd = null;
        }
    }
}
