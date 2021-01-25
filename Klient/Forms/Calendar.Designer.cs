using System;
using System.Windows.Forms;

namespace Klient.Forms
{
    partial class Calendar
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.grpAvCalendars = new System.Windows.Forms.GroupBox();
            this.lbNames = new System.Windows.Forms.ListBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssLogin = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssCurCal = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnAddCalendar = new System.Windows.Forms.Button();
            this.btnDelCalendar = new System.Windows.Forms.Button();
            this.grpCalendar = new System.Windows.Forms.GroupBox();
            this.calendarsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.txtAddNew = new System.Windows.Forms.TextBox();
            this.btnAddParticipant = new System.Windows.Forms.Button();
            this.monthCalendar1 = new MyCalendar();
            this.grpAvCalendars.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.grpCalendar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.calendarsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // grpAvCalendars
            // 
            this.grpAvCalendars.Controls.Add(this.lbNames);
            this.grpAvCalendars.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.grpAvCalendars.Location = new System.Drawing.Point(16, 14);
            this.grpAvCalendars.Name = "grpAvCalendars";
            this.grpAvCalendars.Size = new System.Drawing.Size(227, 212);
            this.grpAvCalendars.TabIndex = 0;
            this.grpAvCalendars.TabStop = false;
            this.grpAvCalendars.Text = "Dostępne kalendarze";
            // 
            // lbNames
            // 
            this.lbNames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbNames.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbNames.FormattingEnabled = true;
            this.lbNames.ItemHeight = 17;
            this.lbNames.Items.AddRange(new object[] {
            "(wczytuję dostępne kalendarze..)"});
            this.lbNames.Location = new System.Drawing.Point(3, 31);
            this.lbNames.Name = "lbNames";
            this.lbNames.Size = new System.Drawing.Size(221, 178);
            this.lbNames.TabIndex = 0;
            this.lbNames.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbNames_DbClick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.tssLogin,
            this.tssCurCal});
            this.statusStrip1.Location = new System.Drawing.Point(0, 260);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(560, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "ssLoad";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // tssLogin
            // 
            this.tssLogin.Name = "tssLogin";
            this.tssLogin.Size = new System.Drawing.Size(118, 17);
            this.tssLogin.Text = "toolStripStatusLabel2";
            // 
            // tssCurCal
            // 
            this.tssCurCal.Name = "tssCurCal";
            this.tssCurCal.Size = new System.Drawing.Size(118, 17);
            this.tssCurCal.Text = "toolStripStatusLabel2";
            // 
            // btnAddCalendar
            // 
            this.btnAddCalendar.Location = new System.Drawing.Point(271, 232);
            this.btnAddCalendar.Name = "btnAddCalendar";
            this.btnAddCalendar.Size = new System.Drawing.Size(122, 23);
            this.btnAddCalendar.TabIndex = 2;
            this.btnAddCalendar.Text = "Dodaj nowy kalendarz";
            this.btnAddCalendar.UseVisualStyleBackColor = true;
            this.btnAddCalendar.Click += new System.EventHandler(this.btnAddCalendar_Click);
            // 
            // btnDelCalendar
            // 
            this.btnDelCalendar.Location = new System.Drawing.Point(399, 232);
            this.btnDelCalendar.Name = "btnDelCalendar";
            this.btnDelCalendar.Size = new System.Drawing.Size(132, 23);
            this.btnDelCalendar.TabIndex = 3;
            this.btnDelCalendar.Text = "Usuń wybrany kalendarz";
            this.btnDelCalendar.UseVisualStyleBackColor = true;
            this.btnDelCalendar.Click += new System.EventHandler(this.btnDelCalendar_Click);
            // 
            // grpCalendar
            // 
            this.grpCalendar.Controls.Add(this.monthCalendar1);
            this.grpCalendar.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.grpCalendar.Location = new System.Drawing.Point(250, 14);
            this.grpCalendar.Name = "grpCalendar";
            this.grpCalendar.Size = new System.Drawing.Size(293, 212);
            this.grpCalendar.TabIndex = 4;
            this.grpCalendar.TabStop = false;
            this.grpCalendar.Text = "Kalendarz";
            // 
            // txtAddNew
            // 
            this.txtAddNew.Location = new System.Drawing.Point(16, 232);
            this.txtAddNew.Name = "txtAddNew";
            this.txtAddNew.Size = new System.Drawing.Size(227, 20);
            this.txtAddNew.TabIndex = 5;
            // 
            // btnAddParticipant
            // 
            this.btnAddParticipant.Location = new System.Drawing.Point(431, 6);
            this.btnAddParticipant.Name = "btnAddParticipant";
            this.btnAddParticipant.Size = new System.Drawing.Size(106, 23);
            this.btnAddParticipant.TabIndex = 6;
            this.btnAddParticipant.Text = "Dodaj nową osobę";
            this.btnAddParticipant.UseVisualStyleBackColor = true;
            this.btnAddParticipant.Click += new System.EventHandler(this.btnAddParticipant_Click);
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.Location = new System.Drawing.Point(12, 27);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.ShowToday = false;
            this.monthCalendar1.TabIndex = 0;
            this.monthCalendar1.DoubleClickEx += new System.EventHandler(this.monthCalendar1_Click);
            // 
            // Calendar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 282);
            this.Controls.Add(this.btnAddParticipant);
            this.Controls.Add(this.txtAddNew);
            this.Controls.Add(this.grpCalendar);
            this.Controls.Add(this.btnDelCalendar);
            this.Controls.Add(this.btnAddCalendar);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.grpAvCalendars);
            this.Name = "Calendar";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Calendar";
            this.grpAvCalendars.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.grpCalendar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.calendarsBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.GroupBox grpAvCalendars;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button btnAddCalendar;
        private System.Windows.Forms.Button btnDelCalendar;
        private System.Windows.Forms.GroupBox grpCalendar;
        private MyCalendar monthCalendar1;
        private System.Windows.Forms.BindingSource calendarsBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNames;
        private System.Windows.Forms.ListBox lbNames;
        private System.Windows.Forms.TextBox txtAddNew;
        private ToolStripStatusLabel tssLogin;
        private ToolStripStatusLabel tssCurCal;
        private Button btnAddParticipant;
    }
}