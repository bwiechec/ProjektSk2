namespace Klient.Forms
{
    partial class CalendarEvents
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
            this.lbEvents = new System.Windows.Forms.ListBox();
            this.btnAddEvent = new System.Windows.Forms.Button();
            this.btnDelEvent = new System.Windows.Forms.Button();
            this.lblHour = new System.Windows.Forms.Label();
            this.lblTopic = new System.Windows.Forms.Label();
            this.lblText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbEvents
            // 
            this.lbEvents.FormattingEnabled = true;
            this.lbEvents.Location = new System.Drawing.Point(12, 25);
            this.lbEvents.Name = "lbEvents";
            this.lbEvents.Size = new System.Drawing.Size(327, 316);
            this.lbEvents.TabIndex = 0;
            // 
            // btnAddEvent
            // 
            this.btnAddEvent.Location = new System.Drawing.Point(157, 347);
            this.btnAddEvent.Name = "btnAddEvent";
            this.btnAddEvent.Size = new System.Drawing.Size(84, 34);
            this.btnAddEvent.TabIndex = 1;
            this.btnAddEvent.Text = "Dodaj wydarzenie";
            this.btnAddEvent.UseVisualStyleBackColor = true;
            this.btnAddEvent.Click += new System.EventHandler(this.btnAddEvent_Click);
            // 
            // btnDelEvent
            // 
            this.btnDelEvent.Location = new System.Drawing.Point(247, 347);
            this.btnDelEvent.Name = "btnDelEvent";
            this.btnDelEvent.Size = new System.Drawing.Size(84, 34);
            this.btnDelEvent.TabIndex = 2;
            this.btnDelEvent.Text = "Usuń wydarzenie";
            this.btnDelEvent.UseVisualStyleBackColor = true;
            this.btnDelEvent.Click += new System.EventHandler(this.btnDelEvent_Click);
            // 
            // lblHour
            // 
            this.lblHour.AutoSize = true;
            this.lblHour.Location = new System.Drawing.Point(9, 10);
            this.lblHour.Name = "lblHour";
            this.lblHour.Size = new System.Drawing.Size(46, 13);
            this.lblHour.TabIndex = 3;
            this.lblHour.Text = "Godzina";
            // 
            // lblTopic
            // 
            this.lblTopic.AutoSize = true;
            this.lblTopic.Location = new System.Drawing.Point(61, 10);
            this.lblTopic.Name = "lblTopic";
            this.lblTopic.Size = new System.Drawing.Size(37, 13);
            this.lblTopic.TabIndex = 4;
            this.lblTopic.Text = "Temat";
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(104, 10);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(34, 13);
            this.lblText.TabIndex = 5;
            this.lblText.Text = "Treść";
            // 
            // CalendarEvents
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 393);
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.lblTopic);
            this.Controls.Add(this.lblHour);
            this.Controls.Add(this.btnDelEvent);
            this.Controls.Add(this.btnAddEvent);
            this.Controls.Add(this.lbEvents);
            this.Name = "CalendarEvents";
            this.Text = "CalendarEvents";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbEvents;
        private System.Windows.Forms.Button btnAddEvent;
        private System.Windows.Forms.Button btnDelEvent;
        private System.Windows.Forms.Label lblHour;
        private System.Windows.Forms.Label lblTopic;
        private System.Windows.Forms.Label lblText;
    }
}