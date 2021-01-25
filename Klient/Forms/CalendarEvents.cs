using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Klient.Forms
{
    public partial class CalendarEvents : Form
    {
        public Calendar.Calendars calendarsStruct;
        private string processingCalendar;
        private DateTime processingDate;
        private string loggedUser;
        private string adress = "192.168.1.17";
        private string port = "1234";

        //Funkcje wysyłające do serwera strukturę kalendarza, aktualizując go
        private void ConnectCallbackText(IAsyncResult ar)
        {
            try
            {
                Socket socketFd = (Socket)ar.AsyncState;

                socketFd.EndConnect(ar);

                SocketStateObject state = new SocketStateObject();
                state.m_SocketFd = socketFd;
                state.sent = 0;

                //WYSŁANIE DO SERVERA OPCJI DZIAŁANIA
                byte[] swOpt = new byte[1];
                swOpt = Encoding.ASCII.GetBytes("u");
                socketFd.Send(swOpt);

                //WYSŁANIE DO SERVERA AKTUALNEGO KALENDARZA
                byte[] curCalendar = Encoding.ASCII.GetBytes(calendarsStruct.calendarName);
                socketFd.Send(curCalendar);

                //STWORZENIE BUFORA DO ODPOWIEDZI OD SERVERA
                byte[] buffer = new byte[256];
                buffer = Encoding.ASCII.GetBytes("");

                //CZEKANIE NA ODPOWIEDŹ OD SERVERA
                socketFd.Receive(buffer);
                while (buffer == Encoding.ASCII.GetBytes(""))
                {
                    socketFd.Receive(buffer);
                }

                byte[] curLine;
                //WYSYŁANIE LINIA PO LINI ZAWARTOŚCI KALENDARZA
                for (int i = 0; i < calendarsStruct.calendarDays.Count; i++)
                {
                    curLine = Encoding.ASCII.GetBytes(calendarsStruct.calendarDays[i].day + "." + calendarsStruct.calendarDays[i].month + "." +
                            calendarsStruct.calendarDays[i].year + ";" + calendarsStruct.calendarDays[i].hour + ";" + calendarsStruct.calendarDays[i].topic + ";" +
                            calendarsStruct.calendarDays[i].text + "\n");
                    socketFd.Send(curLine);

                    //WYCZYSZCZENIE BUFORA
                    buffer = Encoding.ASCII.GetBytes("");
                    socketFd.Receive(buffer);

                    //CZEKANIE NA ODPOWIEDŹ CZY SERVER ODEBRAŁ LINIĘ KALENDARZA
                    while (buffer == Encoding.ASCII.GetBytes(""))
                    {
                        socketFd.Receive(buffer);
                    }
                }
                
                socketFd.Shutdown(SocketShutdown.Both);
                socketFd.Close();

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

                /* complete the DNS query */
                hostEntry = Dns.EndGetHostEntry(ar);
                addresses = hostEntry.AddressList;


                /* create a socket */
                socketFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                /* remote endpoint for the socket */
                endPoint = new IPEndPoint(addresses[0], Int32.Parse(port));


                /* connect to the server */
                socketFd.BeginConnect(endPoint, new AsyncCallback(ConnectCallbackText), socketFd);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd w GetHostEntryCallback: " + ex.Message.ToString());
            }
        }




        //Ustawienie dni które mają jakieś wydarzenia na "bold" w kalendarzu miesięcznym
        private void setCalendarEvents()
        {
            //CONNECTION TO SERVER - UPDATE CALENDAR
            Dns.BeginGetHostByName(adress, new AsyncCallback(GetHostEntryCallbackText), null);
            try
            {
                //this.Invoke((MethodInvoker)delegate
                //{
                lbEvents.Items.Clear();

                DateTime dateTemp;
                //SORTOWANIE WYDARZEŃ TAK ABY TE NAJWCZEŚNIEJ BYLY WYŚWIETLANE NAJWYŻEJ
                calendarsStruct.calendarDays.Sort((x, y) => Int32.Parse(x.hour.Substring(0,2)).CompareTo(Int32.Parse(y.hour.Substring(0, 2))));

                //WYŚWIETLANIE INFORMACJI O KALENDARZU
                for (int i = 0; i < calendarsStruct.calendarDays.Count; i++)
                {
                    dateTemp = new System.DateTime(calendarsStruct.calendarDays[i].year, 
                            calendarsStruct.calendarDays[i].month,
                            calendarsStruct.calendarDays[i].day, 0, 0, 0, 0);
                    if (dateTemp == processingDate)
                        lbEvents.Items.Add(calendarsStruct.calendarDays[i].hour + "\t" 
                            + calendarsStruct.calendarDays[i].topic + "\t" + 
                            calendarsStruct.calendarDays[i].text + "\n");

                }
                //});
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd w setCalendarEvents: " + ex.Message.ToString());
            }
        }

        public CalendarEvents(Calendar.Calendars calStruct, string procCal, DateTime date, String lggdUsr)
        {
            calendarsStruct = calStruct;
            processingCalendar = procCal;
            processingDate = date;
            loggedUser = lggdUsr;
            InitializeComponent();
            setCalendarEvents();
        }

        //Metoda wywołująca klasę odpowiedzialną za dodanie nowego wydarzenia do kalendarza
        private void btnAddEvent_Click(object sender, EventArgs e)
        {
            //WYŚWIETLENIA OKNA ODPOWIEDZIALNEGO ZA DODANIE NOWEGO KALENDARZA
            AddEvent aEvent = new AddEvent(calendarsStruct, processingCalendar, processingDate);
            aEvent.ShowDialog();
            setCalendarEvents();
        }

        //Metoda usuwająca wybrane wydarzenie z kalendarza
        private void btnDelEvent_Click(object sender, EventArgs e)
        {
            try
            {
                int index;
                int delEvent;
                delEvent = lbEvents.SelectedIndex;
                lbEvents.Items.RemoveAt(delEvent);
                calendarsStruct.calendarDays.RemoveAt(delEvent);
                setCalendarEvents();
                //index = calendarsStruct.FindIndex(x => x.calendarName == delCalendar);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd przy usuwaniu kalendarza!");
            }
        }


        public class SocketStateObject
        {
            public const int BUF_SIZE = 1024;
            public byte[] m_DataBuf = new byte[BUF_SIZE];
            public StringBuilder m_StringBuilder = new StringBuilder();
            public Socket m_SocketFd = null;
            public int sent = 0;
        }
    }
}
