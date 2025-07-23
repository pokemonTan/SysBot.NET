using SysBot.Pokemon.WinForms.Properties;

namespace SysBot.Pokemon.WinForms
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            TC_Main = new System.Windows.Forms.TabControl();
            Tab_Bots = new System.Windows.Forms.TabPage();
            CB_Protocol = new System.Windows.Forms.ComboBox();
            FLP_Bots = new System.Windows.Forms.FlowLayoutPanel();
            TB_IP = new System.Windows.Forms.TextBox();
            CB_Routine = new System.Windows.Forms.ComboBox();
            NUD_Port = new System.Windows.Forms.TextBox();
            B_New = new System.Windows.Forms.Button();
            Tab_Hub = new System.Windows.Forms.TabPage();
            PG_Hub = new System.Windows.Forms.PropertyGrid();
            Tab_Logs = new System.Windows.Forms.TabPage();
            RTB_Logs = new System.Windows.Forms.RichTextBox();
            B_Stop = new System.Windows.Forms.Button();
            B_Start = new System.Windows.Forms.Button();
            Tab_Buttons = new System.Windows.Forms.TabPage();
            ResetGame = new System.Windows.Forms.Button();
            CaptureScreenButton = new System.Windows.Forms.Button();
            btn_zr = new System.Windows.Forms.Button();
            btn_zl = new System.Windows.Forms.Button();
            btn_r = new System.Windows.Forms.Button();
            btn_l = new System.Windows.Forms.Button();
            btn_update_robot_info = new System.Windows.Forms.Button();
            btn_plus = new System.Windows.Forms.Button();
            btn_minus = new System.Windows.Forms.Button();
            btn_home = new System.Windows.Forms.Button();
            btn_screen_capture = new System.Windows.Forms.Button();
            btn_b = new System.Windows.Forms.Button();
            btn_a = new System.Windows.Forms.Button();
            btn_y = new System.Windows.Forms.Button();
            btn_x = new System.Windows.Forms.Button();
            btn_arrow_down = new System.Windows.Forms.Button();
            btn_arrow_right = new System.Windows.Forms.Button();
            btn_arrow_left = new System.Windows.Forms.Button();
            btn_arrow_up = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            TC_Main.SuspendLayout();
            Tab_Bots.SuspendLayout();
            Tab_Hub.SuspendLayout();
            Tab_Logs.SuspendLayout();
            Tab_Buttons.SuspendLayout();
            SuspendLayout();
            // 
            // TC_Main
            // 
            TC_Main.Controls.Add(Tab_Bots);
            TC_Main.Controls.Add(Tab_Hub);
            TC_Main.Controls.Add(Tab_Logs);
            TC_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            TC_Main.Location = new System.Drawing.Point(0, 0);
            TC_Main.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TC_Main.Name = "TC_Main";
            TC_Main.SelectedIndex = 0;
            TC_Main.Size = new System.Drawing.Size(533, 357);
            TC_Main.TabIndex = 3;
            // 
            // Tab_Bots
            // 
            Tab_Bots.Controls.Add(CB_Protocol);
            Tab_Bots.Controls.Add(FLP_Bots);
            Tab_Bots.Controls.Add(TB_IP);
            Tab_Bots.Controls.Add(CB_Routine);
            Tab_Bots.Controls.Add(NUD_Port);
            Tab_Bots.Controls.Add(B_New);
            Tab_Bots.Location = new System.Drawing.Point(4, 24);
            Tab_Bots.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_Bots.Name = "Tab_Bots";
            Tab_Bots.Size = new System.Drawing.Size(525, 329);
            Tab_Bots.TabIndex = 0;
            Tab_Bots.Text = "机器人";
            Tab_Bots.UseVisualStyleBackColor = true;
            // 
            // CB_Protocol
            // 
            CB_Protocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Protocol.FormattingEnabled = true;
            CB_Protocol.Location = new System.Drawing.Point(289, 6);
            CB_Protocol.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Protocol.Name = "CB_Protocol";
            CB_Protocol.Size = new System.Drawing.Size(67, 23);
            CB_Protocol.TabIndex = 10;
            CB_Protocol.SelectedIndexChanged += CB_Protocol_SelectedIndexChanged;
            // 
            // FLP_Bots
            // 
            FLP_Bots.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            FLP_Bots.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            FLP_Bots.Location = new System.Drawing.Point(0, 37);
            FLP_Bots.Margin = new System.Windows.Forms.Padding(0);
            FLP_Bots.Name = "FLP_Bots";
            FLP_Bots.Size = new System.Drawing.Size(524, 289);
            FLP_Bots.TabIndex = 9;
            FLP_Bots.Resize += FLP_Bots_Resize;
            // 
            // TB_IP
            // 
            TB_IP.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            TB_IP.Location = new System.Drawing.Point(74, 8);
            TB_IP.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_IP.Name = "TB_IP";
            TB_IP.Size = new System.Drawing.Size(134, 20);
            TB_IP.TabIndex = 8;
            TB_IP.Text = "192.168.0.1";
            // 
            // CB_Routine
            // 
            CB_Routine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Routine.FormattingEnabled = true;
            CB_Routine.Location = new System.Drawing.Point(364, 6);
            CB_Routine.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Routine.Name = "CB_Routine";
            CB_Routine.Size = new System.Drawing.Size(117, 23);
            CB_Routine.TabIndex = 7;
            // 
            // NUD_Port
            // 
            NUD_Port.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            NUD_Port.Location = new System.Drawing.Point(215, 8);
            NUD_Port.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_Port.Name = "NUD_Port";
            NUD_Port.Size = new System.Drawing.Size(67, 20);
            NUD_Port.TabIndex = 6;
            NUD_Port.Text = "6000";
            NUD_Port.ReadOnly = true;
            // 
            // B_New
            // 
            B_New.Location = new System.Drawing.Point(4, 7);
            B_New.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_New.Name = "B_New";
            B_New.Size = new System.Drawing.Size(63, 23);
            B_New.TabIndex = 0;
            B_New.Text = "Add";
            B_New.UseVisualStyleBackColor = true;
            B_New.Click += B_New_Click;
            // 
            // Tab_Hub
            // 
            Tab_Hub.Controls.Add(PG_Hub);
            Tab_Hub.Location = new System.Drawing.Point(4, 24);
            Tab_Hub.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_Hub.Name = "Tab_Hub";
            Tab_Hub.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_Hub.Size = new System.Drawing.Size(525, 329);
            Tab_Hub.TabIndex = 2;
            Tab_Hub.Text = "Hub";
            Tab_Hub.UseVisualStyleBackColor = true;
            // 
            // PG_Hub
            // 
            PG_Hub.BackColor = System.Drawing.SystemColors.Control;
            PG_Hub.Dock = System.Windows.Forms.DockStyle.Fill;
            PG_Hub.Location = new System.Drawing.Point(4, 3);
            PG_Hub.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PG_Hub.Name = "PG_Hub";
            PG_Hub.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            PG_Hub.Size = new System.Drawing.Size(517, 323);
            PG_Hub.TabIndex = 0;
            // 
            // Tab_Logs
            // 
            Tab_Logs.Controls.Add(RTB_Logs);
            Tab_Logs.Location = new System.Drawing.Point(4, 24);
            Tab_Logs.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_Logs.Name = "Tab_Logs";
            Tab_Logs.Size = new System.Drawing.Size(525, 329);
            Tab_Logs.TabIndex = 1;
            Tab_Logs.Text = "Logs";
            Tab_Logs.UseVisualStyleBackColor = true;
            // 
            // RTB_Logs
            // 
            RTB_Logs.Dock = System.Windows.Forms.DockStyle.Fill;
            RTB_Logs.HideSelection = false;
            RTB_Logs.Location = new System.Drawing.Point(0, 0);
            RTB_Logs.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RTB_Logs.Name = "RTB_Logs";
            RTB_Logs.ReadOnly = true;
            RTB_Logs.Size = new System.Drawing.Size(525, 329);
            RTB_Logs.TabIndex = 0;
            RTB_Logs.Text = "";
            // 
            // B_Stop
            // 
            B_Stop.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            B_Stop.Location = new System.Drawing.Point(417, 0);
            B_Stop.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Stop.Name = "B_Stop";
            B_Stop.Size = new System.Drawing.Size(69, 23);
            B_Stop.TabIndex = 4;
            B_Stop.Text = "Stop All";
            B_Stop.UseVisualStyleBackColor = true;
            B_Stop.Click += B_Stop_Click;
            // 
            // B_Start
            // 
            B_Start.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            B_Start.Location = new System.Drawing.Point(341, 0);
            B_Start.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Start.Name = "B_Start";
            B_Start.Size = new System.Drawing.Size(69, 23);
            B_Start.TabIndex = 3;
            B_Start.Text = "Start All";
            B_Start.UseVisualStyleBackColor = true;
            B_Start.Click += B_Start_Click;
            // 
            // Main
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(533, 357);
            Controls.Add(B_Stop);
            Controls.Add(B_Start);
            Controls.Add(TC_Main);
            Icon = Resources.icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "Main";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "SysBot: Pokémon";
            FormClosing += Main_FormClosing;
            TC_Main.ResumeLayout(false);
            Tab_Bots.ResumeLayout(false);
            Tab_Bots.PerformLayout();
            Tab_Hub.ResumeLayout(false);
            Tab_Logs.ResumeLayout(false);
            ResumeLayout(false);
            // 
            // Tab_Buttons
            // 
            Tab_Buttons.Controls.Add(button1);
            Tab_Buttons.Controls.Add(ResetGame);
            Tab_Buttons.Controls.Add(CaptureScreenButton);
            Tab_Buttons.Controls.Add(btn_zr);
            Tab_Buttons.Controls.Add(btn_zl);
            Tab_Buttons.Controls.Add(btn_r);
            Tab_Buttons.Controls.Add(btn_l);
            Tab_Buttons.Controls.Add(btn_update_robot_info);
            Tab_Buttons.Controls.Add(btn_plus);
            Tab_Buttons.Controls.Add(btn_minus);
            Tab_Buttons.Controls.Add(btn_home);
            Tab_Buttons.Controls.Add(btn_screen_capture);
            Tab_Buttons.Controls.Add(btn_b);
            Tab_Buttons.Controls.Add(btn_a);
            Tab_Buttons.Controls.Add(btn_y);
            Tab_Buttons.Controls.Add(btn_x);
            Tab_Buttons.Controls.Add(btn_arrow_down);
            Tab_Buttons.Controls.Add(btn_arrow_right);
            Tab_Buttons.Controls.Add(btn_arrow_left);
            Tab_Buttons.Controls.Add(btn_arrow_up);
            Tab_Buttons.Location = new System.Drawing.Point(4, 26);
            Tab_Buttons.Margin = new System.Windows.Forms.Padding(4);
            Tab_Buttons.Name = "Tab_Buttons";
            Tab_Buttons.Padding = new System.Windows.Forms.Padding(4);
            Tab_Buttons.Size = new System.Drawing.Size(525, 374);
            Tab_Buttons.TabIndex = 3;
            Tab_Buttons.Text = "按键";
            Tab_Buttons.UseVisualStyleBackColor = true;
            // 
            // ResetGame
            // 
            ResetGame.Location = new System.Drawing.Point(216, 220);
            ResetGame.Name = "ResetGame";
            ResetGame.Size = new System.Drawing.Size(75, 23);
            ResetGame.TabIndex = 19;
            ResetGame.Text = "重置模式";
            ResetGame.UseVisualStyleBackColor = true;
            ResetGame.Click += ResetGame_Click;
            // 
            // CaptureScreenButton
            // 
            CaptureScreenButton.Location = new System.Drawing.Point(60, 308);
            CaptureScreenButton.Name = "CaptureScreenButton";
            CaptureScreenButton.Size = new System.Drawing.Size(59, 23);
            CaptureScreenButton.TabIndex = 17;
            CaptureScreenButton.Text = "截图";
            CaptureScreenButton.UseVisualStyleBackColor = true;
            CaptureScreenButton.Click += CaptureScreenButton_Click;
            // 
            // btn_zr
            // 
            btn_zr.Font = new System.Drawing.Font("宋体", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            btn_zr.Location = new System.Drawing.Point(497, 60);
            btn_zr.Margin = new System.Windows.Forms.Padding(4);
            btn_zr.Name = "btn_zr";
            btn_zr.Size = new System.Drawing.Size(27, 30);
            btn_zr.TabIndex = 16;
            btn_zr.Text = "ZR";
            btn_zr.UseVisualStyleBackColor = true;
            btn_zr.Click += btn_zr_Click;
            // 
            // btn_zl
            // 
            btn_zl.Font = new System.Drawing.Font("宋体", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
            btn_zl.Location = new System.Drawing.Point(4, 60);
            btn_zl.Margin = new System.Windows.Forms.Padding(4);
            btn_zl.Name = "btn_zl";
            btn_zl.Size = new System.Drawing.Size(27, 30);
            btn_zl.TabIndex = 15;
            btn_zl.Text = "ZL";
            btn_zl.UseVisualStyleBackColor = true;
            btn_zl.Click += btn_zl_Click;
            // 
            // btn_r
            // 
            btn_r.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 134);
            btn_r.Location = new System.Drawing.Point(467, 21);
            btn_r.Margin = new System.Windows.Forms.Padding(4);
            btn_r.Name = "btn_r";
            btn_r.Size = new System.Drawing.Size(27, 30);
            btn_r.TabIndex = 14;
            btn_r.Text = "R";
            btn_r.UseVisualStyleBackColor = true;
            btn_r.Click += btn_r_Click;
            // 
            // btn_l
            // 
            btn_l.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 134);
            btn_l.Location = new System.Drawing.Point(30, 21);
            btn_l.Margin = new System.Windows.Forms.Padding(4);
            btn_l.Name = "btn_l";
            btn_l.Size = new System.Drawing.Size(27, 30);
            btn_l.TabIndex = 13;
            btn_l.Text = "L";
            btn_l.UseVisualStyleBackColor = true;
            btn_l.Click += btn_l_Click;
            // 
            // btn_update_robot_info
            // 
            btn_update_robot_info.Location = new System.Drawing.Point(415, 301);
            btn_update_robot_info.Margin = new System.Windows.Forms.Padding(4);
            btn_update_robot_info.Name = "btn_update_robot_info";
            btn_update_robot_info.Size = new System.Drawing.Size(88, 30);
            btn_update_robot_info.TabIndex = 12;
            btn_update_robot_info.Text = "更新机器人";
            btn_update_robot_info.UseVisualStyleBackColor = true;
            btn_update_robot_info.Click += btn_update_robot_info_Click;
            // 
            // btn_plus
            // 
            btn_plus.Location = new System.Drawing.Point(405, 21);
            btn_plus.Margin = new System.Windows.Forms.Padding(4);
            btn_plus.Name = "btn_plus";
            btn_plus.Size = new System.Drawing.Size(27, 30);
            btn_plus.TabIndex = 11;
            btn_plus.Text = "✖";
            btn_plus.UseVisualStyleBackColor = true;
            btn_plus.Click += btn_plus_Click;
            // 
            // btn_minus
            // 
            btn_minus.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 134);
            btn_minus.Location = new System.Drawing.Point(92, 21);
            btn_minus.Margin = new System.Windows.Forms.Padding(4);
            btn_minus.Name = "btn_minus";
            btn_minus.Size = new System.Drawing.Size(27, 30);
            btn_minus.TabIndex = 10;
            btn_minus.Text = "-";
            btn_minus.UseVisualStyleBackColor = true;
            btn_minus.Click += btn_minus_Click;
            // 
            // btn_home
            // 
            btn_home.Location = new System.Drawing.Point(405, 229);
            btn_home.Margin = new System.Windows.Forms.Padding(4);
            btn_home.Name = "btn_home";
            btn_home.Size = new System.Drawing.Size(27, 30);
            btn_home.TabIndex = 9;
            btn_home.Text = "●";
            btn_home.UseVisualStyleBackColor = true;
            btn_home.Click += btn_home_Click;
            // 
            // btn_screen_capture
            // 
            btn_screen_capture.Location = new System.Drawing.Point(92, 229);
            btn_screen_capture.Margin = new System.Windows.Forms.Padding(4);
            btn_screen_capture.Name = "btn_screen_capture";
            btn_screen_capture.Size = new System.Drawing.Size(27, 30);
            btn_screen_capture.TabIndex = 8;
            btn_screen_capture.Text = "█";
            btn_screen_capture.UseVisualStyleBackColor = true;
            btn_screen_capture.Click += btn_screen_capture_Click;
            // 
            // btn_b
            // 
            btn_b.Location = new System.Drawing.Point(435, 160);
            btn_b.Margin = new System.Windows.Forms.Padding(4);
            btn_b.Name = "btn_b";
            btn_b.Size = new System.Drawing.Size(27, 30);
            btn_b.TabIndex = 7;
            btn_b.Text = "B";
            btn_b.UseVisualStyleBackColor = true;
            btn_b.Click += btn_b_Click;
            // 
            // btn_a
            // 
            btn_a.Location = new System.Drawing.Point(467, 124);
            btn_a.Margin = new System.Windows.Forms.Padding(4);
            btn_a.Name = "btn_a";
            btn_a.Size = new System.Drawing.Size(27, 30);
            btn_a.TabIndex = 6;
            btn_a.Text = "A";
            btn_a.UseVisualStyleBackColor = true;
            btn_a.Click += btn_a_Click;
            // 
            // btn_y
            // 
            btn_y.Location = new System.Drawing.Point(405, 124);
            btn_y.Margin = new System.Windows.Forms.Padding(4);
            btn_y.Name = "btn_y";
            btn_y.Size = new System.Drawing.Size(27, 30);
            btn_y.TabIndex = 5;
            btn_y.Text = "Y";
            btn_y.UseVisualStyleBackColor = true;
            btn_y.Click += btn_y_Click;
            // 
            // btn_x
            // 
            btn_x.Location = new System.Drawing.Point(435, 93);
            btn_x.Margin = new System.Windows.Forms.Padding(4);
            btn_x.Name = "btn_x";
            btn_x.Size = new System.Drawing.Size(27, 30);
            btn_x.TabIndex = 4;
            btn_x.Text = "X";
            btn_x.UseVisualStyleBackColor = true;
            btn_x.Click += btn_x_Click;
            // 
            // btn_arrow_down
            // 
            btn_arrow_down.Location = new System.Drawing.Point(61, 160);
            btn_arrow_down.Margin = new System.Windows.Forms.Padding(4);
            btn_arrow_down.Name = "btn_arrow_down";
            btn_arrow_down.Size = new System.Drawing.Size(27, 30);
            btn_arrow_down.TabIndex = 3;
            btn_arrow_down.Text = "↓";
            btn_arrow_down.UseVisualStyleBackColor = true;
            btn_arrow_down.Click += btn_arrow_down_Click;
            // 
            // btn_arrow_right
            // 
            btn_arrow_right.Location = new System.Drawing.Point(92, 124);
            btn_arrow_right.Margin = new System.Windows.Forms.Padding(4);
            btn_arrow_right.Name = "btn_arrow_right";
            btn_arrow_right.Size = new System.Drawing.Size(27, 30);
            btn_arrow_right.TabIndex = 2;
            btn_arrow_right.Text = "→";
            btn_arrow_right.UseVisualStyleBackColor = true;
            btn_arrow_right.Click += btn_arrow_right_Click;
            // 
            // btn_arrow_left
            // 
            btn_arrow_left.Location = new System.Drawing.Point(30, 124);
            btn_arrow_left.Margin = new System.Windows.Forms.Padding(4);
            btn_arrow_left.Name = "btn_arrow_left";
            btn_arrow_left.Size = new System.Drawing.Size(27, 30);
            btn_arrow_left.TabIndex = 1;
            btn_arrow_left.Text = "←";
            btn_arrow_left.UseVisualStyleBackColor = true;
            btn_arrow_left.Click += btn_arrow_left_Click;
            // 
            // btn_arrow_up
            // 
            btn_arrow_up.Location = new System.Drawing.Point(61, 93);
            btn_arrow_up.Margin = new System.Windows.Forms.Padding(4);
            btn_arrow_up.Name = "btn_arrow_up";
            btn_arrow_up.Size = new System.Drawing.Size(27, 30);
            btn_arrow_up.TabIndex = 0;
            btn_arrow_up.Text = "↑";
            btn_arrow_up.UseVisualStyleBackColor = true;
            btn_arrow_up.Click += btn_arrow_up_Click;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(216, 160);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(75, 23);
            button1.TabIndex = 20;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;

        }

        #endregion
        private System.Windows.Forms.TabControl TC_Main;
        private System.Windows.Forms.TabPage Tab_Bots;
        private System.Windows.Forms.TabPage Tab_Logs;
        private System.Windows.Forms.RichTextBox RTB_Logs;
        private System.Windows.Forms.TabPage Tab_Hub;
        private System.Windows.Forms.PropertyGrid PG_Hub;
        private System.Windows.Forms.Button B_Stop;
        private System.Windows.Forms.Button B_Start;
        private System.Windows.Forms.TextBox TB_IP;
        private System.Windows.Forms.ComboBox CB_Routine;
        private System.Windows.Forms.TextBox NUD_Port;
        private System.Windows.Forms.Button B_New;
        private System.Windows.Forms.FlowLayoutPanel FLP_Bots;
        private System.Windows.Forms.ComboBox CB_Protocol;
        private System.Windows.Forms.TabPage Tab_Buttons;
        private System.Windows.Forms.Button btn_plus;
        private System.Windows.Forms.Button btn_minus;
        private System.Windows.Forms.Button btn_home;
        private System.Windows.Forms.Button btn_screen_capture;
        private System.Windows.Forms.Button btn_b;
        private System.Windows.Forms.Button btn_a;
        private System.Windows.Forms.Button btn_y;
        private System.Windows.Forms.Button btn_x;
        private System.Windows.Forms.Button btn_arrow_down;
        private System.Windows.Forms.Button btn_arrow_right;
        private System.Windows.Forms.Button btn_arrow_left;
        private System.Windows.Forms.Button btn_arrow_up;
        private System.Windows.Forms.Button btn_update_robot_info;
        private System.Windows.Forms.Button btn_r;
        private System.Windows.Forms.Button btn_l;
        private System.Windows.Forms.Button btn_zr;
        private System.Windows.Forms.Button btn_zl;
        private System.Windows.Forms.Button CaptureScreenButton;
        private System.Windows.Forms.Button ResetGame;
        private System.Windows.Forms.Button button1;
    }
}

