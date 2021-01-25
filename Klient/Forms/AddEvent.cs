using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Klient.Forms
{
    //Klasa odpowiedzialna za dodanie nowego zdarzenia do kalendarza
    public partial class AddEvent : Form
    {
        public Calendar.Calendars calendarsStruct; //Struktura kalendarza
        private string processingCalendar; // kalendarz na którym operujemy
        private DateTime processingDate; //aktualna data na której operujemy
        private string adress = "192.168.1.17";
        private string port = "1234";

        public AddEvent(Calendar.Calendars calStruct, string procCal, DateTime date)
        {
            calendarsStruct = calStruct;
            processingCalendar = procCal;
            processingDate = date;
            InitializeComponent();
        }

        //Metoda odpowiedzialna za akcję po kliknięciu dodaj
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtHour.Text.ToString()[1] == ':') //JEŚLI PODANA DATA W FORMACIE np.: 8:00 PRZEKSZTAŁCENIE NA 08:00
                {
                    txtHour.Text = "0" + txtHour.Text.ToString();
                }
                if(Int32.Parse(txtHour.Text.ToString().Substring(0, 2)) >= 0 && Int32.Parse(txtHour.Text.ToString().Substring(0, 2)) < 24)
                {   //DODANIE DO STRUKTURY KALENDARZA NOWEGO ZDARZENIA
                    calendarsStruct.calendarDays.Add(new Calendar.CalendarDay()
                    {
                        day = processingDate.Day,
                        year = processingDate.Year,
                        month = processingDate.Month,
                        hour = txtHour.Text.ToString(),
                        text = txtText.Text.ToString(),
                        topic = txtTopic.Text.ToString()
                    });
                }
                else
                {
                    MessageBox.Show("Podano błędną godzinę!");
                }


                Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd przy dodawaniu nowego wydarzenia.");
            }
        }

        //Metoda odpowiedzialna za akcję po kliknięciu wróć
        private void btnDel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
