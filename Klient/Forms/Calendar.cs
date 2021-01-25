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
    public partial class Calendar : Form
    {
        private string loggedUser;
        private string adress = "192.168.1.17";
        private string port = "1234";
        public bool logged = false;
        private List<string> calendars = new List<string>();
        private List<string> calendarsText = new List<string>();
        private List<string> calendarsLine = new List<string>();
        private string curCalendar;
        public List<Calendars> calendarsStruct = new List<Calendars>();

        public Calendar(string log)
        {
            loggedUser = log;
            InitializeComponent();
            //loadCalendarNames();


            loadCalendars();
            tssLogin.Text = "Zalogowany jako: " + log;
        }
        public class CalendarDay
        {
            public int year;
            public int month;
            public int day;
            public string hour;
            public string topic;
            public string text;
        };

        public class Calendars
        {
            public string calendarName;
            public List<CalendarDay> calendarDays = new List<CalendarDay>();
        };

        //Ustawienie obecnie wyświetlanego kalendarza
        private void setCurrentCalendar(string CalendarName)
        {
            try {

                if (!IsHandleCreated)
                {
                    CreateHandle();
                }

                this.Invoke((MethodInvoker)delegate
                {

                    int index = 0;
                    curCalendar = CalendarName;
                    tssCurCal.Text = "Kożystasz z kalendarza: " + curCalendar;

                    index = calendarsStruct.FindIndex(x => x.calendarName == CalendarName);

                    DateTime[] bold = new DateTime[calendarsStruct[index].calendarDays.Count];

                    for (int i = 0; i < calendarsStruct[index].calendarDays.Count; i++)
                    {
                        bold[i] = new System.DateTime(calendarsStruct[index].calendarDays[i].year, calendarsStruct[index].calendarDays[i].month, 
                            calendarsStruct[index].calendarDays[i].day, 0, 0, 0, 0);
                    }
                    monthCalendar1.BoldedDates = bold;

                    if(lbNames.Items.Count == 1 && lbNames.Items[0].ToString() == "(wczytuję dostępne kalendarze..)")
                    {
                        lbNames.Items.RemoveAt(0);

                        for (int i = 0; i < calendarsStruct.Count; i++)
                        {
                            lbNames.Items.Add(calendarsStruct[i].calendarName);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd w setCurrentCalendar: " + ex.Message.ToString());
            }

        }

        //METODY ODPOWIEDZIALNE ZA POBRANIE KALENDARZY Z SERVERA
        private void ReceiveCallbackText(IAsyncResult ar)
        {
            try
            {
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                int size = socketFd.EndReceive(ar);

                string currentReadCalendar = "";
                List<string> date;

                Calendars calTemp = new Calendars();
                CalendarDay calDayTemp = new CalendarDay();

                if (size > 0)
                {
                    state.m_StringBuilder.Append(Encoding.ASCII.GetString(state.m_DataBuf, 0, size));
                    socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallbackText), state);
                }
                else if(state.m_StringBuilder.Length > 1)
                {   //JEŚLI SERVER NIE WYSYŁA JUŻ NIC PRZETWARZAMY DANE WYSŁANE OD SERVERA
                    calendarsText = state.m_StringBuilder.ToString().Split('\n').ToList();  //ODDZIELAMY WYDARZENIA
                    calendarsText.Remove("");
                    socketFd.Shutdown(SocketShutdown.Both);
                    socketFd.Close();

                    for (int i = 0; i < calendarsText.Count; i++)
                    {
                        calendarsLine = calendarsText[i].ToString().Split(';').ToList();
                        for (int j = 0; j < calendarsLine.Count; j++)
                        {
                            if(calendarsLine.Count == 1)
                            {
                                if (calendarsLine[j].ToString() == "!@#") //ZNAK KOŃCA KALENDARZA
                                {
                                    calendarsStruct.Add(new Calendars() { calendarDays = calTemp.calendarDays, calendarName = calTemp.calendarName });
                                    calTemp.calendarDays = new List<CalendarDay>();
                                    break;
                                }
                                if (calendarsLine[j].ToString() != "")
                                {
                                    calTemp.calendarName = calendarsLine[j].ToString();
                                }
                            }
                            else if (calendarsLine.Count > 1)
                            {
                                //INDEX 0 CALENDARSLINE -> DATA
                                //INDEX 1 CALENDARSLINE -> GODZINA
                                //INDEX 2 CALENDARSLINE -> TEMAT
                                //INDEX 3 CALENDARSLINE -> TRESC
                                date = calendarsLine[0].ToString().Split('.').ToList();
                                calDayTemp.year = Int32.Parse(date[2]);
                                calDayTemp.month = Int32.Parse(date[1]);
                                calDayTemp.day = Int32.Parse(date[0]);
                                calDayTemp.hour = calendarsLine[1].ToString();
                                calDayTemp.topic = calendarsLine[2].ToString();
                                calDayTemp.text = calendarsLine[3].ToString();

                                calTemp.calendarDays.Add(new CalendarDay() { year = calDayTemp.year, month = calDayTemp.month, 
                                day = calDayTemp.day, hour = calDayTemp.hour, topic = calDayTemp.topic, text = calDayTemp.text});
                                break;
                            }
                        }
                    }

                    setCurrentCalendar(calendarsStruct[calendarsStruct.Count-1].calendarName);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd w ReceiveCallback calendar123: " + ex.Message.ToString());
            }
        }

        private void ConnectCallbackText(IAsyncResult ar)
        {
            try
            {
                Socket socketFd = (Socket)ar.AsyncState;

                socketFd.EndConnect(ar);

                SocketStateObject state = new SocketStateObject();
                state.m_SocketFd = socketFd;

                //WYBÓR OPCJI DZIALANIA
                byte[] swOpt = new byte[1];
                swOpt = Encoding.ASCII.GetBytes("c");
                socketFd.Send(swOpt);

                //WYSŁANIE ZALOGOWANEGO UŻYTKOWNIKA, ABY ODCZYTAĆ JEGO KALENDARZE
                byte[] loggedUsr = Encoding.ASCII.GetBytes(loggedUser);
                socketFd.Send(loggedUsr);

                socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallbackText), state);
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

        private void loadCalendars()
        {
            try
            {
                Dns.BeginGetHostByName(adress, new AsyncCallback(GetHostEntryCallbackText), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problem z załadowaniem dostępnych kalendarzy");
            }

        }

        //KONIEC METOD ODPOWIEDZIALNYCH ZA POBRANIE KALENDARZY Z SERWERA

        //METODY ODPOWIEDZIALNE ZA USUNIĘCIE KALENDARZA
        private void ConnectCallbackDel(IAsyncResult ar)
        {
            try
            {
                Socket socketFd = (Socket)ar.AsyncState;

                socketFd.EndConnect(ar);

                SocketStateObject state = new SocketStateObject();
                state.m_SocketFd = socketFd;

                //WYBÓR OPCJI DZIAŁANIA
                byte[] swOpt = new byte[1];
                swOpt = Encoding.ASCII.GetBytes("d");
                socketFd.Send(swOpt);

                //WYSŁANIE KALENDARZA DO USUNIECIA
                byte[] calName = new byte[256];
                calName = Encoding.ASCII.GetBytes(txtAddNew.Text.ToString());
                socketFd.Send(calName, 0, calName.Length, 0);

                this.Invoke((MethodInvoker)delegate
                {
                     txtAddNew.Text = "";
                });

                socketFd.Shutdown(SocketShutdown.Both);
                socketFd.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd w ConnectCallbackDel: " + ex.Message.ToString());
            }
        }

        private void GetHostEntryCallbackDel(IAsyncResult ar)
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
                socketFd.BeginConnect(endPoint, new AsyncCallback(ConnectCallbackDel), socketFd);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd w GetHostEntryCallbackDel: " + ex.Message.ToString());
            }
        }

        private void btnDelCalendar_Click(object sender, EventArgs e)
        {
            try
            {
                int index;
                string delCalendar;
                if (lbNames.Items.Count > 0 && lbNames.Items[0].ToString() != "(wczytuję dostępne kalendarze..)")
                {
                    delCalendar = lbNames.SelectedItem.ToString();
                    txtAddNew.Text = delCalendar;
                    lbNames.Items.Remove(delCalendar);
                    index = calendarsStruct.FindIndex(x => x.calendarName == delCalendar);
                    calendarsStruct.RemoveAt(index);
                }
                if (lbNames.Items.Count == 0) curCalendar = "";
                else curCalendar = lbNames.Items[0].ToString();

                setCurrentCalendar(curCalendar);

                //CONNECTION TO SERVER - DELETE CALENDAR
                Dns.BeginGetHostByName(adress, new AsyncCallback(GetHostEntryCallbackDel), null);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd przy usuwaniu kalendarza!");
            }
        }
        //KONIEC METOD ODPOWIEDZIALNYCH ZA USUNIĘCIE KALENDARZA

        //METODY ODPOWIEDZIALNE ZA DODANIE NOWEGO KALENDARZA
        private void ConnectCallbackAdd(IAsyncResult ar)
        {
            try
            {
                Socket socketFd = (Socket)ar.AsyncState;

                socketFd.EndConnect(ar);

                SocketStateObject state = new SocketStateObject();
                state.m_SocketFd = socketFd;

                //WYBRANIE OPCJI DZIAŁANIA
                byte[] swOpt = new byte[1];
                swOpt = Encoding.ASCII.GetBytes("a");
                socketFd.Send(swOpt);

                //WYSŁANIE NAZWY KALENDARZA
                byte[] calName = new byte[256];
                calName = Encoding.ASCII.GetBytes(txtAddNew.Text.ToString());
                socketFd.Send(calName, 0, calName.Length, 0);

                //BUFOR CZEKAJĄCY NA ODPOWIEDŹ OD SERVERA
                byte[] buffer = new byte[256];
                buffer = Encoding.ASCII.GetBytes("");

                //CZEKANIE NA ODP OD SERVERA
                socketFd.Receive(buffer);
                while (buffer == Encoding.ASCII.GetBytes(""))
                {
                    socketFd.Receive(buffer);
                }
                //WYSŁANIE ZALOGOWANEGO UŻYTKOWNIKA DO SERVERA, ABY AUTOMATYCZNIE DODAĆ GO DO OSOB MAJACYCH DOSTEP DO KALENDARZA
                socketFd.Send(Encoding.ASCII.GetBytes(loggedUser));


                this.Invoke((MethodInvoker)delegate
                {
                    txtAddNew.Text = "";
                });

                socketFd.Shutdown(SocketShutdown.Both);
                socketFd.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd w ConnectCallbackDel: " + ex.Message.ToString());
            }
        }

        private void GetHostEntryCallbackAdd(IAsyncResult ar)
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
                socketFd.BeginConnect(endPoint, new AsyncCallback(ConnectCallbackAdd), socketFd);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd w GetHostEntryCallbackDel: " + ex.Message.ToString());
            }
        }

        private void btnAddCalendar_Click(object sender, EventArgs e)
        {
            Calendars calTemp = new Calendars();

            if(lbNames.Items[0].ToString() == "(wczytuję dostępne kalendarze..)" && txtAddNew.Text != "") { 
                lbNames.Items.RemoveAt(0);
                //lbNames.Items.Add(txtAddNew.Text);
                curCalendar = txtAddNew.Text;
                //txtAddNew.Text = "";
            }
            else if(txtAddNew.Text != "")
            {
                //lbNames.Items.Add(txtAddNew.Text);
                curCalendar = txtAddNew.Text;
                //txtAddNew.Text = "";
            }
            calendarsStruct.Add(new Calendars());
            calendarsStruct[calendarsStruct.Count - 1].calendarName = curCalendar;
            lbNames.Items.Add(curCalendar);
            setCurrentCalendar(curCalendar);

            //CONNECTION TO SERVER - ADD NEW CALENDAR
            Dns.BeginGetHostByName(adress, new AsyncCallback(GetHostEntryCallbackAdd), null);

        }
        //KONIEC METOD ODPOWIEDZIALNYCH ZA DODANIE NOWEGO KALENDARZA

        //METODA ODPOWIEDZIALNA ZA AKCJE PO DWUKROTNYM KLIKNIĘCIU NA DZIEŃ
        private void monthCalendar1_Click(object sender, EventArgs e)
        {
            int index;
            index = calendarsStruct.FindIndex(x => x.calendarName == curCalendar);
            CalendarEvents cEvent = new CalendarEvents(calendarsStruct[index], curCalendar,monthCalendar1.SelectionRange.Start, loggedUser);
            cEvent.ShowDialog();
            setCurrentCalendar(curCalendar);
        }

        //METODA ODPOWIEDZIALNA ZA AKCJE PO DWUKROTNYM KLIKNIĘCIU NA NAZWE KALENDARZA
        private void lbNames_DbClick(object sender, EventArgs e)
        {
            curCalendar = lbNames.SelectedItem.ToString();
            setCurrentCalendar(curCalendar);
        }

        public class SocketStateObject
        {
            public const int BUF_SIZE = 1024;
            public byte[] m_DataBuf = new byte[BUF_SIZE];
            public StringBuilder m_StringBuilder = new StringBuilder();
            public Socket m_SocketFd = null;
        }

        //METODA ODPOWIEDZIALNA ZA AKCJE PO KLIKNIĘCIU PRZYCISKU "DODAJ NOWĄ OSOBĘ"
        private void btnAddParticipant_Click(object sender, EventArgs e)
        {
            AddParticipant aPart = new AddParticipant(curCalendar);
            aPart.ShowDialog();
        }

    }
}
